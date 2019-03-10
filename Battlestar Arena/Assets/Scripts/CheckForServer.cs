using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// An enumerator of game modes.
public enum Gamemode {
	Classic,
	Competitive,
	BattleRoyale
}

// Object representing server settings.
public class Settings {
	// The IP address that the server will bind to.
	public string ip = "127.0.0.1";
	// The port that the server will listen on.
	public int port = 7777;
	// The maximum number of players allowed.
	public int players = 4;
	// The game mode that will be played.
	public Gamemode gamemode = Gamemode.Classic;
}

// Class for checking whether or not we're running a dedicated server.
public class CheckForServer : MonoBehaviour {
	// On start.
	private void Start () {
		// If the application is running in batch mode...
		if (Application.isBatchMode) {
			// Store a reference to the game network manager.
			NetworkManager serverNetworkManager = gameObject.GetComponent<NetworkManager>();
			// Generate a file path relative to the applications data directory, looking for server.properties.
			string propertiesFilePath = Path.Combine(Application.dataPath, "server.properties");
			// If a file exists at that path...
			if (File.Exists(propertiesFilePath)) {
				// Load it into a string.
				string json = File.ReadAllText(propertiesFilePath);
				// Serialize an object using the json data.
				Settings serverSettings = JsonUtility.FromJson<Settings>(json);
				// Adjust the settings of our network manager accordingly.
				serverNetworkManager.networkAddress = serverSettings.ip;
				serverNetworkManager.networkPort = serverSettings.port;
			// If no server.properties file was found...
			} else {
				// Let the user know the configuration is missing.
				Debug.LogWarning("server.properties file not found, using default settings.");
			}
			// Start the server.
			serverNetworkManager.StartServer();
		}
	}
}
