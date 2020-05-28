using System.Collections;
using System.Collections.Generic;
using HeathenEngineering.UIX;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class NicknameSetting : MonoBehaviour {
	public TMP_InputField inputField;
	public GameObject keyboard;
	public GameObject firstKey;
	public GameObject enterKey;

	private string nickname;
	private MainMenu menu;
	
	private void Start() {
		menu = FindObjectOfType<MainMenu>();
	}

	public void OpenKeyboard() {
		menu.typing = true;
		inputField.transform.parent.gameObject.SetActive(true);
		inputField.text = NetworkManager.Instance.Nickname;
		keyboard.SetActive(true);
		menu.SetEventSystemSelectedObject(string.IsNullOrEmpty(NetworkManager.Instance.Nickname) ? firstKey : enterKey);
	}
	
	public void SetNickname() {

		if (!string.IsNullOrEmpty(inputField.text)) {

			NetworkManager.Instance.Nickname = inputField.text;
			Hide();
			menu.CreateOrJoinRoom();
		}
	}

	public void Esc() {
		menu.SetEventSystemSelectedObject(menu.matchTypeButtons[0]);
		Hide();
	}
	
	
	public void Hide() {
		menu.typing = false;
		keyboard.SetActive(false);
		inputField.transform.parent.gameObject.SetActive(false);
	}
}