using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {
	private void Start () {
		playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private Rigidbody2D playerRigidbody;
	[Tooltip("Determines how quickly the player gets up to speed.")]
	public float movementAcceleration = 2f;
	[Tooltip("Determines the maximum speed of the player in units per second.")]
	public float maximumSpeed = 5f;
	[Tooltip("Determines how much force will be applied to the player when they use their movement ability.")]
	public float movementAbilityPushFactor = 500f;
	private bool isMovementAbilityAvailable = true;
	[Tooltip("How long the players movement ability will be on cooldown until they can use it again, in seconds.")]
	public int movementAbilityCooldownTime = 5;

	private void MovementAbility () {
		Vector2 direction = playerRigidbody.velocity.normalized;
		playerRigidbody.AddForce(direction * movementAbilityPushFactor * playerRigidbody.drag);
	}

	private IEnumerator MovementAbilityCooldown () {
		isMovementAbilityAvailable = false;
		yield return new WaitForSeconds(movementAbilityCooldownTime);
		isMovementAbilityAvailable = true;
	}

	private void Update () {
		if (Input.GetButtonDown("Burst")) {
			if (isMovementAbilityAvailable) {
				MovementAbility();
				StartCoroutine("MovementAbilityCooldown");
			}
		}
	}

	private void FixedUpdate () {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		Vector2 direction = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);
		playerRigidbody.drag = movementAcceleration / maximumSpeed;
		playerRigidbody.AddForce(direction * movementAcceleration);
	}
}
