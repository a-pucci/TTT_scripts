using System.Collections;
using UnityEngine;

public class PillarSurface : Surface {
	public PillarSettings settings;
	
	public override void TriggerHit(BallPhysics ball) {
		Vector3 ballVelocity = ball.velocity;
		ball.gameObject.SetActive(false);

		var newDirection = new Vector3();
		var newVelocity = new Vector3();
		
		// direction
		if (settings.fixedOutputDirections) {
			int random = Random.Range(0, settings.outputDirections.Count);
			Vector3 randomDirection = settings.outputDirections[random];
			newDirection = new Vector3(randomDirection.x, ballVelocity.normalized.y, randomDirection.z).normalized;
		}
		else {
			Vector3 normalizedDirection = ballVelocity.normalized;
			newDirection = new Vector3(-normalizedDirection.x, normalizedDirection.y, normalizedDirection.z);
		}
		
		// offset
		if (settings.addRandomOffset) {
			newDirection = AddOffset(newDirection);
		}
		
		// strength
		if (settings.fixedOutputStrength) {
			newVelocity = newDirection * settings.outputStrength;
		}
		else {
			newVelocity = newDirection * ballVelocity.magnitude;
		}
		
		// spawn point
		float offsetZ;
		var spawnPoint = new Vector3();
		float radius = ball.GetComponent<SphereCollider>().radius;
		if (ball.transform.position.z > transform.position.z) {
			offsetZ = (ball.transform.position.z - transform.position.z) + radius/2;
			spawnPoint = new Vector3(ball.transform.position.x, ball.transform.position.y, transform.position.z - offsetZ);
		}
		else {
			offsetZ = (transform.position.z - ball.transform.position.z) + radius/2;
			spawnPoint = new Vector3(ball.transform.position.x, ball.transform.position.y, transform.position.z + offsetZ);
		}

		Vector3 ballOutputVelocity = newVelocity;
		Debug.Log("ball pos: " + ball.transform.position + " | radius: " + radius);
		Debug.Log("offset: " + offsetZ);
		Debug.Log("spawn point: " + spawnPoint);
		Debug.Log("ball velocity: " + ball.velocity + " | magnitude: " + ball.velocity.magnitude);
		Debug.Log("ball out velocity: " + ballOutputVelocity + " | magnitude: " + ballOutputVelocity.magnitude);
		StartCoroutine(SpawnBall(ball,spawnPoint, ballOutputVelocity));
	}

	private Vector3 AddOffset(Vector3 direction) {
		Vector3 offset = settings.directionOffset;
		direction = new Vector3(direction.x + Random.Range(-offset.x, offset.x), direction.x, direction.z + Random.Range(-offset.z, offset.z));
		return direction.normalized;
	}

	private IEnumerator SpawnBall(BallPhysics ball,Vector3 position, Vector3 velocity) {
		yield return new WaitForSeconds(settings.outputWaitTime);
		ball.gameObject.SetActive(true);
		ball.transform.position = position;
		ball.velocity = velocity;
	}

}
