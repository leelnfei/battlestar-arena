using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {
	Classic = 0,
	Competitive = 1,
	BattleRoyale = 2,
	Custom = 3,
}


public class DiscordSecrets {
	public long clientId;
}

public class GameManager : MonoBehaviour {

	Discord.Discord discord;
	Discord.ActivityManager activityManager;
	Discord.RelationshipManager relationshipManager;
	Discord.ImageManager imageManager;
	Discord.UserManager userManager;
	Discord.LobbyManager lobbyManager;

	public Discord.ActivityManager GetActivityManager () {
		return activityManager;
	}

	public Discord.RelationshipManager GetRelationshipManager () {
		return relationshipManager;
	}

	public Discord.ImageManager GetImageManager () {
		return imageManager;
	}

	public Discord.UserManager GetUserManager () {
		return userManager;
	}

	public Discord.LobbyManager GetLobbyManager () {
		return lobbyManager;
	}

	public enum DiscordInstance {
		Default = 0,
		Canary = 1,
		PTB = 2,
	}

	public DiscordInstance discordInstance = DiscordInstance.Default;
	private long clientId;
	public Text console;

	private void Awake () {
		Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", string.Format("{0}", (int)discordInstance));

		DontDestroyOnLoad(gameObject);
		GatherSecrets();

		discord = new Discord.Discord(clientId, (UInt64)Discord.CreateFlags.Default);

		discord.SetLogHook(Discord.LogLevel.Debug, (level, message) => {
			Debug.LogFormat("Log[{0}] {1}", level, message);
		});

		activityManager = discord.GetActivityManager();
		relationshipManager = discord.GetRelationshipManager();
		imageManager = discord.GetImageManager();
		userManager = discord.GetUserManager();
		lobbyManager = discord.GetLobbyManager();
	}

	private void Start () {
		userManager.OnCurrentUserUpdate += () => {
			var currentUser = userManager.GetCurrentUser();
			Conlog(string.Format("Welcome, {0}#{1}", currentUser.Username, currentUser.Discriminator));
		};
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

	public void Conlog (string text) {
		console.text += string.Format("{0}\n", text);
		Debug.Log(text);
	}

	private void GatherSecrets () {
		string propertiesFilePath = Path.Combine(Application.dataPath, "secrets.json");
		if (File.Exists(propertiesFilePath)) {
			string json = File.ReadAllText(propertiesFilePath);
			DiscordSecrets discordSecrets = JsonUtility.FromJson<DiscordSecrets>(json);
			clientId = discordSecrets.clientId;
		} else {
			Debug.LogError("No Discord client ID provided.");
		}
	}
}
