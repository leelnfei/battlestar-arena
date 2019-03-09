using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketData : MonoBehaviour {

	// The amount of time the rocket can stay alive without contact.
	// This value is directly set by the players FireRocket component, on instantiation.
	public float deathTimer = 1f;
	// A reference to the player who fired this given instance of the rocket.
	public GameObject sender;

	// Called when the rocket must be destroyed.
	private void Death () {
		// Run the Reset method on the sender.
		sender.GetComponent<FireRocket>().Reset();
		// TODO: Animate.
		// TODO: Play sounds.
		// Destroy the gameobject.
		Destroy(this.gameObject);
	}

	// A coroutine for timing the rockets life span.
	private IEnumerator WaitForDeath () {
		// Wait for the given amount of time.
		yield return new WaitForSeconds(deathTimer);
		// Run the death method.
		Death();
	}

	// A public method to begin counting down the rockets life span.
	public void StartDeathTimer() {
		// Start the coroutine.
		StartCoroutine("WaitForDeath");
	}
}
