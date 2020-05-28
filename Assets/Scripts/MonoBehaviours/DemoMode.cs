using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class DemoMode : MonoBehaviour {

	public float timeToLoadDemo;
	[ShowInInspector, ReadOnly]
	private float timeSinceLastInput;
	private bool playingDemo;
	public static DemoMode instance;

#if PUBLICDEMO

	private void Awake() {
		if (instance != null && instance != this) {
			Destroy(this);
		}
		else {
			instance = this;
			DontDestroyOnLoad(this);
		}
	}

	// Update is called once per frame
	void Update () {
		timeSinceLastInput += Time.deltaTime;
		if (ReadAnyControllerInput("1") || ReadAnyControllerInput("2")) {
			timeSinceLastInput = 0;
			if (playingDemo) {
				SceneManager.LoadScene("00_Menu");
				playingDemo = false;
			}
		}
		if (!playingDemo && timeSinceLastInput > timeToLoadDemo) {
			playingDemo = true;
			SceneManager.LoadScene("DemoScene");
		}
	}

	private bool ReadAnyControllerInput(string number) {
		return Input.GetButtonDown("Dash" + number) ||
			   Input.GetButtonDown("Swing" + number) ||
			   Input.GetButtonDown("Charge" + number) ||
			   Input.GetAxis("Horizontal" + number) > 0.19f ||
			   Input.GetAxis("Vertical" + number) > 0.19f ||
			   Input.GetAxis("Horizontal" + number) < -0.19f ||
			   Input.GetAxis("Vertical" + number) < -0.19f ||
			   Input.anyKeyDown;
	}

#else

	private void Awake() {
		Destroy(this);
	}

#endif

}
