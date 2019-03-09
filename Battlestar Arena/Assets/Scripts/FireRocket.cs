using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireRocket : MonoBehaviour {

	// Called when the object enters a scene.
	void Start () {
		// Set the player rigidbody component accordingly.
		playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	[Tooltip("The rocket prefab that the player will be shooting.")]
	public GameObject rocketPrefab;
	[Tooltip("The amount of time a player must wait before shooting another rocket.")]
	public float rocketCooldownTime = 5f;
	[Tooltip("The amount of time, in seconds, that a rocket will live before exploding.")]
	public float rocketLifespan = 5f;
	// A private boolean to know whether or not the player can shoot a rocket.
	private bool canShootRocket = true;
	// A private reference to the players ridigbody component.
	private Rigidbody2D playerRigidbody;

	// The method for shooting a rocket.
	private void ShootRocket () {
		// Gather an angle using trigonometry, based on the current velocity of the player.
		float angle = Mathf.Atan2(playerRigidbody.velocity.y, playerRigidbody.velocity.x) * Mathf.Rad2Deg - 90f;
		// Instantiate a new instance of the rocket, rotated at the angle we calculated earlier.
		GameObject instance = Instantiate(rocketPrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		// Get the rocket data component of the instance.
		// Rocket data is a custom script to store information regarding the rocket,
		// most importantly it's sender.
		RocketData rocketData = instance.GetComponent<RocketData>();
		// Set the rocket data sender to be this gameobject.
		rocketData.sender = this.gameObject;
		// Set the rocket data death timer to be the value we provided in this script.
		rocketData.deathTimer = rocketLifespan;
		// Start the rocket death timer,
		// so that it will explode after a given amount of time without contact.
		rocketData.StartDeathTimer();
		// Please note that once the rocket is destroyed, either by contact or by death timer,
		// it must call the Reset method in this componenet of it's sender. See RocketData.cs,
		// line 11.
	}

	// A coroutine for the fire cooldown.
	private IEnumerator ShootingCooldown () {
		// Wait for the given amount of time.
		yield return new WaitForSeconds(rocketCooldownTime);
		// Let the player shoot rockets again.
		canShootRocket = true;
	}

	// Reset method, called from a fired projectile on contact or death.
	public void Reset () {
		// Start the shooting cooldown in a coroutine.
		StartCoroutine("ShootingCooldown");
	}

	// Called once per frame.
	void Update () {
		// If the fire button is pressed.
		if (Input.GetButtonDown("Fire")) {
			// If the player is allowed to shoot a rocket.
			if (canShootRocket) {
				// Shoot a rocket.
				ShootRocket();
				// Don't let the player shoot a rocket until the existing one has been destroyed,
				// and the player has waited their cooldown.
				canShootRocket = false;
			}
		}
	}
}
