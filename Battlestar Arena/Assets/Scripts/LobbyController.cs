using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour {

	// Global game manager.
	public GameManager gameManager;
	// Discord SDK managers.
	Discord.ActivityManager activityManager;
	Discord.ImageManager imageManager;
	Discord.UserManager userManager;
	Discord.LobbyManager lobbyManager;
	// Lobby lists.
	List<Discord.Lobby> connectedLobby = new List<Discord.Lobby>();
	List<Discord.User> lobbyPeers = new List<Discord.User>();
	// UI Elements.
	public Texture2D defaultLobbyMemberTexture;
	public GameObject lobbyGraphicPanel;
	public GameObject createLobbyButton;
	public GameObject leaveLobbyButton;
	public GameObject queueButton;
	public RawImage[] lobbyMemberImages;
	// Queueing.
	public bool queueing = false;

	void Start () {

		/* Step 1. Initialize the Discord SDK managers.
		 * 
		 * This is so that we can do things like invite friends,
		 * check user profiles, create and edit lobbies, and load
		 * user images from Discord.
		 */

		InitializeDiscordManagers();

		/* Step 2. Update Rich Presence and set some events.
		 * 
		 * The events we set will allow users to invite each other
		 * through the Discord client properly, and for that information
		 * to be visible within the game client as well.
		 */

		UpdateActivity();
		InitializeActivityEvents();
	}

	void InitializeDiscordManagers () {
		activityManager = gameManager.GetActivityManager();
		imageManager = gameManager.GetImageManager();
		userManager = gameManager.GetUserManager();
		lobbyManager = gameManager.GetLobbyManager();
	}

	void InitializeActivityEvents () {
		activityManager.OnActivityJoin += JoinLobby;
		activityManager.OnActivityJoinRequest += JoinRequest;
		activityManager.OnActivityInvite += InviteRequest;
	}

	public void JoinRequest (ref Discord.User user) {

		/* TODO: Check if the current user is in a game. If they are,
		 * automatically reject the invitation. Otherwise, continue and
		 * display it.	 
		 */

		gameManager.Conlog(string.Format("{0}#{1} has requested to join your lobby.", user.Username, user.Discriminator));

		bool userIsBusy = false;
		if (userIsBusy) { // This will be replaced with an actual condition.
			return;
		} else {
			StartCoroutine(DialogPrompt(string.Format("{0}#{1} has requested to join your lobby.", user.Username, user.Discriminator), user.Id, 10));
		}
	}

	public void InviteRequest (Discord.ActivityActionType type, ref Discord.User user, ref Discord.Activity activity) {
		gameManager.Conlog(string.Format("{0}#{1} has invited you to join their lobby.", user.Username, user.Discriminator));
		StartCoroutine(DialogPrompt(string.Format("{0}#{1} has invited you to join their lobby.", user.Username, user.Discriminator), user.Id, 10));
	}

	IEnumerator DialogPrompt (string message, long userId, float dialogTimeout) {

		/* TODO: Set the callback function of the dialog prompt to trigger
		 * an AcceptInvite method with the proper user ID. Set the body text
		 * of the dialog to show the appropriate message. Then, display
		 * the dialog prompt.
		 */

		// here we are logging to the debugger, because the
		// dialog functionality has not been implemented yet.
		Debug.LogFormat("\"{0}\", {1}", message, userId);
		yield return new WaitForSeconds(dialogTimeout);
		Debug.LogFormat("Dialog has disappeared after {0} seconds", dialogTimeout);

		// TODO: Hide the dialog prompt.
	}

	public void InviteUser (Discord.User user) {
		activityManager.SendInvite(user.Id, Discord.ActivityActionType.Join, string.Format("Hey {0}, join my lobby?", user.Username), (result) => {
			if (result == Discord.Result.Ok) {
				gameManager.Conlog(string.Format("{0}#{1} accepted your invite.", user.Username, user.Discriminator));
			} else {
				gameManager.Conlog(string.Format("{0}#{1} refused your invite.", user.Username, user.Discriminator));
			}
		});
	}

	public void InviteUser (Discord.User user, string message) {
		activityManager.SendInvite(user.Id, Discord.ActivityActionType.Join, string.Format("Hey {0}, {1}", user.Username, message), (result) => {
			if (result == Discord.Result.Ok) {
				gameManager.Conlog(string.Format("{0}#{1} accepted your invite.", user.Username, user.Discriminator));
			} else {
				gameManager.Conlog(string.Format("{0}#{1} refused your invite.", user.Username, user.Discriminator));
			}
		});
	}

	public void CreateLobby () {

		/* First things first, check that we're not already within a lobby. If we for some reason are,
		 * then we need to leave it. Then, we can create the lobby.	
		 */

		if (connectedLobby.Count > 0) {
			gameManager.Conlog("Already in lobby. Leaving...");
			LeaveLobby();
		}

		gameManager.Conlog("Attempting to create lobby...");

		var transaction = lobbyManager.GetLobbyCreateTransaction();
		transaction.SetType(Discord.LobbyType.Public);
		transaction.SetCapacity(4);
		transaction.SetMetadata("avg_elo", "443");
		transaction.SetMetadata("gamemode", GameMode.Classic.ToString());

		lobbyManager.CreateLobby(transaction, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result != Discord.Result.Ok) {
				gameManager.Conlog(string.Format("Lobby could not be created. Reason: {0}", result));
				return;
			}

			gameManager.Conlog("Lobby created successfuly");
			InitializeLobby(lobby);
		});
	}

	public void JoinLobby (string secret) {

		/* First things first, check that we're not already within a lobby. If we for some reason are,
		 * then we need to leave it. Then, we can create the lobby.	
		 */

		if (connectedLobby.Count > 0) {
			gameManager.Conlog("Already in lobby. Leaving...");
			LeaveLobby();
		}

		gameManager.Conlog("Attempting to join lobby...");

		lobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) => {
			if (result != Discord.Result.Ok) {
				gameManager.Conlog(string.Format("Lobby could not be joined. Reason: {0}", result));
				return;
			}

			gameManager.Conlog("Lobby joined successfuly");
			InitializeLobby(lobby);
		});
	}

	public void LeaveLobby () {
		gameManager.Conlog("Leaving lobby...");
		lobbyManager.DisconnectLobby(connectedLobby[0].Id, (result) => {
			if (result == Discord.Result.Ok) {
				DeinitializeLobby();
				gameManager.Conlog("Left lobby");
			}
		});
	}

	void InitializeLobby (Discord.Lobby lobby) {
		connectedLobby.Add(lobby);
		UpdateActivity(lobby);

		// Trigger the necessary UI.
		ToggleUIElements(true);

		/* TODO: Set up event methods so that when new members join or leave the lobby, things will happen.
		 * Additionally, initialize the user interface here, and begin gathering data on player images.
		 */

		foreach (Discord.User user in lobbyManager.GetMemberUsers(lobby.Id)) {
			lobbyPeers.Add(user);
			SetImage(user);
		}

		lobbyManager.OnMemberConnect += (lobbyId, userId) => {
			/* Double check that the lobby matches our current lobby,
			 * then fetch and apply the users image.		
			 * 
			 * Add the user to our lobby peers list.
			 */

			if (connectedLobby[0].Id == lobbyId) {
				userManager.GetUser(userId, (Discord.Result result, ref Discord.User user) => {
					if (result == Discord.Result.Ok) {
						lobbyPeers.Add(user);
						SetImage(user);
						gameManager.Conlog(string.Format("User {0}#{1} has connected to your lobby.", user.Username, user.Discriminator));
						UpdateActivity(lobby);
					}
				});
			} else {
				gameManager.Conlog("Your client is connected to multiple lobbies. Please report this error and restart your game.");
			}
		};

		lobbyManager.OnMemberDisconnect += (lobbyId, userId) => {
			/* Double check that the lobby matches our current lobby,
			 * then remove the users image.
			 * 
			 * Remove the user from our lobby peers list.
			 */

			if (connectedLobby[0].Id == lobbyId) {
				userManager.GetUser(userId, (Discord.Result result, ref Discord.User user) => {
					if (result == Discord.Result.Ok) {
						UnsetImage(user);
						lobbyPeers.Remove(user);
						gameManager.Conlog(string.Format("User {0}#{1} has disconnected from your lobby.", user.Username, user.Discriminator));
						UpdateActivity(lobby);
					}
				});
			} else {
				gameManager.Conlog("Your client is connected to multiple lobbies. Please report this error and restart your game.");
			}
		};
	}

	public void FindMatch () {
		var searchQuery = lobbyManager.GetSearchQuery();
		// Filter the results by game mode.
		string lobbyGameMode = lobbyManager.GetLobbyMetadataValue(connectedLobby[0].Id, "gamemode");
		searchQuery.Filter("metadata.gamemode", Discord.LobbySearchComparison.Equal, Discord.LobbySearchCast.String, lobbyGameMode);
		// Sort the results by nearest average ELO rating.
		string lobbyAverageELO = lobbyManager.GetLobbyMetadataValue(connectedLobby[0].Id, "avg_elo");
		searchQuery.Sort("meteadata.avg_elo", Discord.LobbySearchCast.Number, lobbyAverageELO);
		searchQuery.Distance(Discord.LobbySearchDistance.Extended);
		StartCoroutine(Queue(searchQuery));
	}

	IEnumerator Queue (Discord.LobbySearchQuery searchQuery) {
		gameManager.Conlog("Queueing...");
		queueing = true;
		yield return new WaitUntil(() => {
			uint resultsToGet = 1;
			while (queueing) {
				searchQuery.Limit(resultsToGet);
				lobbyManager.Search(searchQuery, (result) => {
					if (result == Discord.Result.Ok) {
						var resultCount = lobbyManager.LobbyCount();
						gameManager.Conlog(string.Format("{0} lobbies were found", resultCount));
						if (resultCount > 0) {
							var bestLobbyId = lobbyManager.GetLobbyId(0);
							gameManager.Conlog(string.Format("{0}: Your lobby ID", connectedLobby[0]));
							gameManager.Conlog(string.Format("{0}: Found lobby ID", bestLobbyId));
							// TODO: Connect to the lobby.
							queueing = false;
						}
					}
				});
				gameManager.Conlog("Couldn't find any lobbies, expanding search.");
				resultsToGet = resultsToGet * 2;
			}
			queueing = false;
			return true;
		});
	}

	void DeinitializeLobby () {
		foreach (Discord.User user in lobbyManager.GetMemberUsers(connectedLobby[0].Id)) {
			UnsetImage(user);
		}

		// Trigger the necessary UI.
		ToggleUIElements(false);

		lobbyPeers.Clear();
		connectedLobby.Clear();
		UpdateActivity();

		/* TODO: Tear down any constructions set up previously, clear the data on player images and
		 * deinitialize the user interface.
		 */
	}

	void ToggleUIElements (bool on) {
		lobbyGraphicPanel.SetActive(on);
		createLobbyButton.SetActive(!on);
		leaveLobbyButton.SetActive(on);
		queueButton.SetActive(on);
	}

	void SetImage(Discord.User user) {
		imageManager.Fetch(Discord.ImageHandle.User(user.Id), (Discord.Result result, Discord.ImageHandle handle) => {
			if (result == Discord.Result.Ok) {
				var dimensions = imageManager.GetDimensions(handle);
				Texture2D userAvatar = new Texture2D((int)dimensions.Width, (int)dimensions.Height, TextureFormat.RGBA32, false);
				byte[] data = imageManager.GetData(handle);
				userAvatar.LoadRawTextureData(data);
				userAvatar.Apply();
				lobbyMemberImages[lobbyPeers.IndexOf(user)].texture = userAvatar;
			}
		});
	}

	void UnsetImage(Discord.User user) {
		lobbyMemberImages[lobbyPeers.IndexOf(user)].texture = defaultLobbyMemberTexture;
	}

	void UpdateActivity () {
		var activity = new Discord.Activity {
			Details = "In Menus",
			Assets = {
				LargeImage = "ba_square",
			},
		};

		activityManager.UpdateActivity(activity, (result) => {
			gameManager.Conlog(string.Format("Update Activity returned \"{0}\"", result));
		});
	}

	void UpdateActivity (Discord.Lobby lobby) {
		var activity = new Discord.Activity {
			State = "In a Lobby",
			Details = lobbyManager.GetLobbyMetadataValue(lobby.Id, "gamemode"),
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
			gameManager.Conlog(string.Format("Update activity returned \"{0}\"", result));
		});
	}
}
