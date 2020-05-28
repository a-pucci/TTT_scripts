using System.Linq;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class Framerate : MonoBehaviour {
	public static Framerate instance;

#if UNITY_EDITOR || INTERNAL_BUILD

	private void Awake() {
		if (instance != null && instance != this) {
			Destroy(this);
		}
		else {
			instance = this;
			DontDestroyOnLoad(this);
		}
	}

	float deltaTime = 0.0f;

	void Update() {
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI() {
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		text += " | Ping: " + PhotonNetwork.GetPing();
		GUI.Label(rect, text, style);

	}

#else

	private void Awake() {
		Destroy(this);
	}

#endif

}
