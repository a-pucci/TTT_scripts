using System.Collections;
using UnityEngine;

public class KillerPoint : MonoBehaviour {

	public KillerPointSettings killerPointSettings;

	public void StartCountdown() {
		StartCoroutine(ExplosionCountdown());
	}

	private IEnumerator ExplosionCountdown() {
		yield return new WaitForSeconds(killerPointSettings.killerPointTimer);
		Explode();
	}

	private void Explode() {
		//Check the ball's Z to determine whose side of the field you're on and determine the winner using the GameManager
	}
}
