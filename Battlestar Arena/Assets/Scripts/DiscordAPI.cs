using System;
using System.Linq;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscordAPI : MonoBehaviour {

	Discord.Discord discord;
	Discord.ActivityManager activityManager;
	Discord.RelationshipManager relationshipManager;
	Discord.ImageManager imageManager;
	Discord.UserManager userManager;
	Discord.LobbyManager lobbyManager;
	Discord.NetworkManager networkManager;
	Discord.ApplicationManager applicationManager;
	Discord.StorageManager storageManager;
	Discord.OverlayManager overlayManager;
	Discord.AchievementManager achievementManager;

	public enum DiscordInstance {
		Default = 0,
		Canary = 1,
		PTB = 2,
	}

	public DiscordInstance discordInstance = DiscordInstance.Default;

	private void Awake () {
		Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", string.Format("{0}", (int)discordInstance));

		DontDestroyOnLoad(gameObject);

		discord = new Discord.Discord(555090239341854739, (UInt64)Discord.CreateFlags.Default);

		discord.SetLogHook(Discord.LogLevel.Debug, (level, message) => {
			Debug.LogFormat("Log[{0}] {1}", level, message);
		});

		activityManager = discord.GetActivityManager();
		relationshipManager = discord.GetRelationshipManager();
		imageManager = discord.GetImageManager();
		userManager = discord.GetUserManager();
		lobbyManager = discord.GetLobbyManager();
		networkManager = discord.GetNetworkManager();
		applicationManager = discord.GetApplicationManager();
		storageManager = discord.GetStorageManager();
		overlayManager = discord.GetOverlayManager();
		achievementManager = discord.GetAchievementManager();
	}

	private void Start () {
		userManager.OnCurrentUserUpdate += () => {
			var currentUser = userManager.GetCurrentUser();
			Conlog(string.Format("Welcome, {0}#{1}", currentUser.Username, currentUser.Discriminator));
		};

		UpdateActivity();

		activityManager.OnActivityJoin += JoinLobby;

		activityManager.OnActivityJoinRequest += (ref Discord.User user) => {
			Conlog(string.Format("User {0}#{1} has requested to join your lobby.", user.Username, user.Discriminator));

			// TODO: Link to an invite manager to display the request.
			// TODO: Ignore if on do-not-disturb mode.
		};

		activityManager.OnActivityInvite += (Discord.ActivityActionType type, ref Discord.User user, ref Discord.Activity activity) => {
			if (type == Discord.ActivityActionType.Join) {
				bool acceptInvite = true;
				// TODO: Link to an invite manager to display the invite.

				// If the user accepts the invitation.
				if (acceptInvite) {
					// Carry out the invite.
					activityManager.AcceptInvite(user.Id, (result) => {
						if (result != Discord.Result.Ok) {
							Conlog("Something went wrong.");
							return;
						}
						Conlog("Success!");
					});
				}
			}
		};
	}

	public void CreateLobby () {
		Conlog("Attempting to create lobby...");

		var transaction = lobbyManager.GetLobbyCreateTransaction();
		transaction.SetType(Discord.LobbyType.Public);
		transaction.SetCapacity(4);
		transaction.SetMetadata("avg_elo", "443");

		lobbyManager.CreateLobby(transaction, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result != Discord.Result.Ok) {
				Conlog(string.Format("Lobby could not be created. Reason: {0}", result));
				return;
			}

			Conlog("Lobby created successfuly");
			UpdateActivity(lobby);
		});
	}

	public void JoinLobby(string secret) {
		Conlog("Attempting to join lobby...");
		lobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result != Discord.Result.Ok) {
				Conlog(string.Format("Lobby could not be joined. Reason: {0}", result));
				return;
			}

			Conlog(string.Format("Lobby {0} joined successfuly", lobby.Id));
			UpdateActivity(lobby);
		});
	}

	private void UpdateActivity() {
		var activity = new Discord.Activity {
			Details = "In Menus",
			Assets = {
				LargeImage = "ba_square",
			},
		};

		activityManager.UpdateActivity(activity, (result) => {
			Conlog(string.Format("Update Activity returned \"{0}\"", result));
		});
	}

	private void UpdateActivity(Discord.Lobby lobby) {
		var activity = new Discord.Activity {
			State = "In a Lobby",
			Details = "<lobby_gamemode>",
			Assets = {
				LargeImage = "ba_square",
				LargeText = "<lobby_mapname>",
				SmallImage = "ba_square",
				SmallText = "<user_competitive_ranking>",
			},
			Party = {
				Id = lobby.Id.ToString(),
				Size = {
					CurrentSize = lobbyManager.MemberCount(lobby.Id),
					MaxSize = (int)lobby.Capacity,
				},
			},
			Secrets = {
				Join = lobbyManager.GetLobbyActivitySecret(lobby.Id),
			},
		};

		activityManager.UpdateActivity(activity, (result) => {
			Conlog(string.Format("Update activity returned \"{0}\"", result));
		});
	}

	private void Update () {
		discord.RunCallbacks();
	}

	private void LateUpdate () {
		lobbyManager.FlushNetwork();
	}

	private void OnApplicationQuit () {
		discord.Dispose();
	}

	private void Conlog (string text) {
		GameObject.FindWithTag("Console").GetComponent<Text>().text += string.Format("{0}\n", text);
		Debug.Log(text);
	}
}
