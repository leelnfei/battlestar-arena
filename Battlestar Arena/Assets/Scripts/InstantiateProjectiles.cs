using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateProjectiles : MonoBehaviour {

  public GameObject Projectile;
  // canFire determines if projectile can be fired (made false when projectile is fired)
  // timeOfluanch holds the time of button pressed
  // realTime keeps constant track of real time (defined in Update)
  // lengthOfTime used to set how long projectile will last
  public bool canFire = true;
  float timeOfLaunch = 0.0f;
  public float lengthOfTime = 0;
  private Rigidbody2D playerRigidbody;
  GameObject clone;

  //spawns projectile at players location
  void FireRocket() {
    clone = Instantiate(Projectile, transform.position, Quaternion.Euler(new Vector3(0, 0, checkDirection())));
  }

  void Start() {
    playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
  }

	// Update is called once per frame
	void Update () {
    float realTime = Time.time;
    Debug.Log(playerRigidbody.velocity.normalized);
    // checks if player has shot
    if (Input.GetKeyDown("space") && canFire == true) {
        FireRocket();
        canFire = false;
        timeOfLaunch = Time.time;
    }

    // determines if correct amount of time has passed before another projectile may be shot
    if (realTime > timeOfLaunch + lengthOfTime) {
      canFire = true;
      Destroy(this.clone);
    }

    checkDirection();

	}

  //checks direction player is facing, returns a float that affects the angle at which the projectile is shot
  int checkDirection() {
    int angle = 0;

    if (playerRigidbody.velocity.normalized.y > 0) {
      angle = 0;
    }

    if (playerRigidbody.velocity.normalized.y < 0) {
      angle = 180;
    }

    if (playerRigidbody.velocity.normalized.x > 0) {
      angle = 270;
    }

    if (playerRigidbody.velocity.normalized.x < 0) {
      angle = 90;
    }

    return angle;
  }
}
