using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}

	private Rigidbody2D rb;
	public float movementSpeed = 5f;
	
	void Update () {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		rb.velocity = Vector2.ClampMagnitude(
			new Vector2(
				horizontal,
				vertical),
			1f) * movementSpeed;
	}
}
