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
	Discord.LobbyManager lobbyManager;
	Discord.ApplicationManager applicationManager;
	Discord.ActivityManager activityManager;
	Discord.UserManager userManager;

	private void Awake () {
		DontDestroyOnLoad(gameObject);

		discord = new Discord.Discord(555090239341854739, (UInt64)Discord.CreateFlags.Default);

		discord.SetLogHook(Discord.LogLevel.Debug, (level, message) => {
			Debug.LogFormat("Log[{0}] {1}", level, message);
		});

		applicationManager = discord.GetApplicationManager();
		activityManager = discord.GetActivityManager();
		lobbyManager = discord.GetLobbyManager();
	}

	private void Start () {
		Conlog(string.Format("Current Locale: {0}", applicationManager.GetCurrentLocale()));
		Conlog(string.Format("Current Branch: {0}", applicationManager.GetCurrentBranch()));

		var activity = new Discord.Activity {
			Details = "In Menus",
			Assets = {
				LargeImage = "ba_square",
			},
		};

		activityManager.UpdateActivity(activity, (result) => {
			Conlog(string.Format("Update Activity {0}", result));
		});

		activityManager.OnActivityJoin += JoinLobby;
	}

	public void CreateLobby () {
		var user = discord.GetUserManager().GetCurrentUser();

		var transaction = lobbyManager.GetLobbyCreateTransaction();

		lobbyManager.CreateLobby(transaction, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result == Discord.Result.Ok) {
				Conlog(string.Format("Lobby {0} created with secret {1}", lobby.Id, lobby.Secret));

				var activity = new Discord.Activity {
					State = "In a Lobby",
					Details = "Classic",
					Assets = {
						LargeImage = "ba_square",
						LargeText = "cl_asteroid",
						SmallImage = "ba_square",
						SmallText = "Rank Master II",
					},
					Party = {
						Id = lobby.Id.ToString(),
						Size = {
							CurrentSize = lobbyManager.MemberCount(lobby.Id),
							MaxSize = (int)lobby.Capacity,
						},
					},
					Secrets = {
						Join = lobby.Secret,
					},
				};

				activityManager.UpdateActivity(activity, (newResult) => {
					Conlog(string.Format("Update activity {0}", newResult));
				});
			}
		});
	}

	public void JoinLobby(string secret) {
		lobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result == Discord.Result.Ok) {
				Conlog(string.Format("Lobby {0} joined with secret {1}", lobby.Id, lobby.Secret));

				var activity = new Discord.Activity {
					State = "In a Lobby",
					Details = "Classic",
					Assets = {
						LargeImage = "ba_square",
						LargeText = "cl_asteroid",
						SmallImage = "ba_square",
						SmallText = "Rank Master II",
					},
					Party = {
						Id = lobby.Id.ToString(),
						Size = {
							CurrentSize = lobbyManager.MemberCount(lobby.Id),
							MaxSize = (int)lobby.Capacity,
						},
					},
					Secrets = {
						Join = lobby.Secret,
					},
				};

				lobbyManager.ConnectNetwork(lobby.Id);
				lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);

				activityManager.UpdateActivity(activity, (newResult) => {
					Conlog(string.Format("Update activity {0}", newResult));
				});

				for (int i = 0; i < lobbyManager.MemberCount(lobby.Id); i++) {
					var userId = lobbyManager.GetMemberUserId(lobby.Id, i);
					string message = string.Format("Hello, my name is {0}", discord.GetUserManager().GetCurrentUser().Username);
					lobbyManager.SendNetworkMessage(lobby.Id, userId, 0, Encoding.UTF8.GetBytes(message));
				}
			}
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
