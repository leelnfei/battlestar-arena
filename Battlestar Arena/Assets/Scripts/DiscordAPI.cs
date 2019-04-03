using System;
using System.Linq;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordAPI : MonoBehaviour {

	Discord.Discord discord;
	Discord.LobbyManager lobbyManager;

	private void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start () {
		var clientID = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");

		if (clientID == null) {
			Debug.Log("Couldn't get Client ID from environment variables.");
			clientID = "555090239341854739";
		}

		discord = new Discord.Discord(Int64.Parse(clientID), (UInt64)Discord.CreateFlags.Default);

		discord.SetLogHook(Discord.LogLevel.Debug, (level, message) => {
			Debug.LogFormat("Log[{0}] {1}", level, message);
		});

		var applicationManager = discord.GetApplicationManager();

		Debug.LogFormat("Current Locale: {0}", applicationManager.GetCurrentLocale());
		Debug.LogFormat("Current Branch: {0}", applicationManager.GetCurrentBranch());

		var activityManager = discord.GetActivityManager();
		lobbyManager = discord.GetLobbyManager();

		var activity = new Discord.Activity {
			Details = "In Menus",
			Assets = {
				LargeImage = "ba_square",
			},
		};

		activityManager.UpdateActivity(activity, (result) => {
			Debug.LogFormat("Update Activity {0}", result);
		});

		// When someone accepts a request to join or an invite.
		activityManager.OnActivityJoin += (secret) => {
			Debug.LogFormat("OnJoin {0}", secret);
			lobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) => {
				Debug.LogFormat("Conneceted to Lobby {0}", lobby.Id);
				lobbyManager.ConnectNetwork(lobby.Id);
				lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);
				foreach (var user in lobbyManager.GetMemberUsers(lobby.Id)) {
					lobbyManager.SendNetworkMessage(lobby.Id, user.Id, 0, Encoding.UTF8.GetBytes(String.Format("Hello, {0}!", user.Username)));
				}
				UpdateActivity(discord, lobby);
			});
		};

		// Someone has requested to spectate the game.
		activityManager.OnActivitySpectate += (secret) => {
			Debug.LogFormat("OnSpectate {0}", secret);
		};

		// A user has requested to join the game. UI elements get triggered here.
		activityManager.OnActivityJoinRequest += (ref Discord.User user) => {
			Debug.LogFormat("OnJoinRequest {0} {1}", user.Id, user.Username);
		};

		// An invite has been received.
		activityManager.OnActivityInvite += (Discord.ActivityActionType Type, ref Discord.User user, ref Discord.Activity activity2) => {
			Debug.LogFormat("OnInvite {0} {1} {2}", Type, user.Username, activity2.Name);
		};

		/*
		var imageManager = discord.GetImageManager();

		var userManager = discord.GetUserManager();

		userManager.OnCurrentUserUpdate += () => {
			var currentUser = userManager.GetCurrentUser();
			Debug.Log(currentUser.Username);
			Debug.Log(currentUser.Id);
		};

		userManager.GetUser(166263936839188480, (Discord.Result result, ref Discord.User user) => {
			if (result == Discord.Result.Ok) {
				Debug.LogFormat("User fetched: {0}", user.Username);
			} else {
				Debug.LogFormat("User fetch error: {0}", result);
			}
		});

		var relationshipManager = discord.GetRelationshipManager();

		relationshipManager.OnRefresh += () => {
			relationshipManager.Filter((ref Discord.Relationship relationship) => {
				return relationship.Type == Discord.RelationshipType.Friend;
			});

			Debug.LogFormat("Relationships update: {0}", relationshipManager.Count());

			for (var i = 0; i < Math.Min(relationshipManager.Count(), 10); i++) {
				var r = relationshipManager.GetAt((uint)i);
				Debug.LogFormat("Relationships: {0} {1} {2} {3}", r.Type, r.User.Username, r.Presence.Status, r.Presence.Activity.Name);

				FetchAvatar(imageManager, r.User.Id);
			}
		};

		relationshipManager.OnRelationshipUpdate += (ref Discord.Relationship r) => {
			Debug.LogFormat("Relationship updated: {0} {1} {2} {3}", r.Type, r.User.Username, r.Presence.Status, r.Presence.Activity.Name);
		};
		*/

		/*
		lobbyManager.OnLobbyMessage += (lobbyId, userId, data) => {
			Debug.LogFormat("Lobby message: {0} {1}", lobbyId, Encoding.UTF8.GetString(data));
		};

		lobbyManager.OnNetworkMessage += (lobbyId, userId, channelId, data) => {
			Debug.LogFormat("Network message: {0} {1} {2} {3}", lobbyId, userId, channelId, Encoding.UTF8.GetString(data));
		};

		lobbyManager.OnSpeaking += (lobbyId, userId, speaking) => {
			Debug.LogFormat("Lobby speaking: {0} {1} {2}", lobbyId, userId, speaking);
		};
		*/

		Discord.LobbyTransaction transaction = lobbyManager.GetLobbyCreateTransaction();
		transaction.SetType(Discord.LobbyType.Public);
		transaction.SetMetadata("a", "123");
		transaction.SetMetadata("b", "456");
		transaction.SetMetadata("c", "111");
		transaction.SetMetadata("d", "222");

		lobbyManager.CreateLobby(transaction, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result != Discord.Result.Ok) {
				return;
			}

			Debug.LogFormat("Lobby {0} with capacity {1} and secret {2}", lobby.Id, lobby.Capacity, lobby.Secret);

			foreach (var key in new string[] {"a", "b", "c"}) {
				Debug.LogFormat("{0} = {1}", key, lobbyManager.GetLobbyMetadataValue(lobby.Id, key));
			}

			foreach (var user in lobbyManager.GetMemberUsers(lobby.Id)) {
				Debug.LogFormat("Lobby member: {0}", user.Username);
			}

			lobbyManager.SendLobbyMessage(lobby.Id, "Hello from C#!", (_) => {
				Debug.Log("Sent message");
			});

			var lobbyTransaction = lobbyManager.GetLobbyUpdateTransaction(lobby.Id);
			lobbyTransaction.SetMetadata("d", "e");
			lobbyTransaction.SetCapacity(16);
			lobbyManager.UpdateLobby(lobby.Id, lobbyTransaction, (_) => {
				Debug.Log("Lobby has been updated");
			});

			var lobbyID = lobby.Id;
			var userID = lobby.OwnerId;
			var memberTransaction = lobbyManager.GetMemberUpdateTransaction(lobbyID, userID);
			memberTransaction.SetMetadata("hello", "there");
			lobbyManager.UpdateMember(lobbyID, userID, memberTransaction, (_) => {
				Debug.LogFormat("Lobby member has been updated: {0}", lobbyManager.GetMemberMetadataValue(lobbyID, userID, "hello"));
			});

			var query = lobbyManager.GetSearchQuery();
			query.Filter("metadata.a", Discord.LobbySearchComparison.GreaterThan, Discord.LobbySearchCast.Number, "455");
			query.Sort("metadata.a", Discord.LobbySearchCast.Number, "0");

			query.Limit(1);
			lobbyManager.Search(query, (_) => {
				Debug.LogFormat("Search returned {0} lobbies", lobbyManager.LobbyCount());
				if (lobbyManager.LobbyCount() == 1) {
					Debug.LogFormat("First lobby secret: {0}", lobbyManager.GetLobby(lobbyManager.GetLobbyId(0)).Secret);
				}
			});

			lobbyManager.ConnectVoice(lobby.Id, (_) => {
				Debug.LogFormat("Connected to voice chat!");
			});

			lobbyManager.ConnectNetwork(lobby.Id);
			lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);

			UpdateActivity(discord, lobby);
		});

		Debug.Log("Got through it");
	}

	private static void UpdateActivity(Discord.Discord discord, Discord.Lobby lobby) {
		var activityManager = discord.GetActivityManager();
		var activityLobbyManager = discord.GetLobbyManager();

		var activity = new Discord.Activity {
			State = "olleh",
			Details = "foo details",
			Timestamps = {
				Start = 5,
				End = 6,
			},
			Assets = {
				LargeImage = "foo largeImageKey",
				LargeText = "foo largeImageText",
				SmallImage = "foo smallImageKey",
				SmallText = "foo smallImageText",
			},
			Party = {
				Id = lobby.Id.ToString(),
				Size = {
					CurrentSize = activityLobbyManager.MemberCount(lobby.Id),
					MaxSize = (int)lobby.Capacity,
				}
			},
			Secrets = {
				Join = activityLobbyManager.GetLobbyActivitySecret(lobby.Id),
			},
			Instance = true,
		};

		activityManager.UpdateActivity(activity, (result) => {
			Debug.LogFormat("Update Activity {0}", result);
		});
	}

	static void FetchAvatar(Discord.ImageManager imageManager, Int64 userID) {
		imageManager.Fetch(Discord.ImageHandle.User(userID), (result, handle) => {
			if (result == Discord.Result.Ok) {
				var data = imageManager.GetData(handle);
				Debug.LogFormat("Image updated {0} {1}", handle.Id, data.Length);
			} else {
				Debug.LogFormat("Image error {0}", handle.Id);
			}
		});
	}

	private void Update () {
		discord.RunCallbacks();
		lobbyManager.FlushNetwork();
	}

	private void OnApplicationQuit () {
		discord.Dispose();
	}
}
