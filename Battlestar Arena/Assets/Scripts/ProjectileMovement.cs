using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileMovement : MonoBehaviour {
	void Start () {
		projectileRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	public float projectileAcceleration = 1f;
	public float projectileSpeed = 5f;
	public float projectileRotationSensitivity = 10f;
	private float movement;
	private Rigidbody2D projectileRigidbody;

	void Update () {
		movement = Input.GetAxis("Rocket");
	}

	private void FixedUpdate () {
		//transform.Rotate(new Vector3(0, 0, movement * projectileRotationSensitivity));
		projectileRigidbody.drag = projectileAcceleration / projectileSpeed;
		projectileRigidbody.AddForce(transform.rotation * Vector2.up * projectileAcceleration);
		projectileRigidbody.MoveRotation(projectileRigidbody.rotation + movement * projectileRotationSensitivity);
	}
}
