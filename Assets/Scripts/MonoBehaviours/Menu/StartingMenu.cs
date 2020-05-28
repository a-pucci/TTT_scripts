using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour {

	private Rewired.Player player1, player2;
	public float timeBeforeCanChangeScene;
	public float timeTextOff;
	public float timeTextOn;
	private bool canChangeScene;
	public GameObject playText;

	private void Start() {
		player1 = Rewired.ReInput.players.GetPlayer(0);
		player2 = Rewired.ReInput.players.GetPlayer(1);
		playText.SetActive(false);
		StartCoroutine(SceneChange());
	}
	
	private void Update () {
		if ((player1.GetButtonDown("Start") || player2.GetButtonDown("Start"))
			&& canChangeScene) {
			StopAllCoroutines();
			SceneManager.LoadScene("scn_menu");
		}
	}

	private IEnumerator SceneChange() {
		yield return new WaitForSeconds(timeBeforeCanChangeScene);
		canChangeScene = true;
		StartCoroutine(TextBlink());
	}

	private IEnumerator TextBlink() {
		bool isActive = false;
		
		while (true) {
			isActive = !isActive;
			playText.SetActive(isActive);
			if (isActive) {
				yield return new WaitForSeconds(timeTextOn);
			}
			else {
				yield return new WaitForSeconds(timeTextOff);
			}
		}
	}
}