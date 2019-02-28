using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckForServer : MonoBehaviour {
	private void Start () {
		if (Application.isBatchMode) {
			Debug.Log("Application is running in Batch mode.");
			Debug.Log("Starting server...");
			gameObject.GetComponent<NetworkManager>().StartServer();
			/* 
			 * In the future, we should be using a more customized configuration depending on an associated file,
			 * similarly to how Minecraft uses a server.properties file to load configuration data. In there we can
			 * store public IP (if necessary), and port information as well as server password.
			 */
			Debug.Log("Server is running.");
		} else {
			Debug.Log("Application is running normally.");
		}
	}
}
