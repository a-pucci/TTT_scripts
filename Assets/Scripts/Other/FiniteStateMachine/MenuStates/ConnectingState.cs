using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ConnectingState : FSMState<MainMenu> {
	public static ConnectingState Instance { get; } = new ConnectingState();
	
	private const float WaitTime = 0.5f;
	
	private int counter;
	private float currentTime;
	private int elapsedTime;

	public override void Enter (MainMenu m) {
		
		m.menuGameObject.SetActive(true);
		m.lobbyCanvas.SetActive(true);
		m.waitingText.transform.parent.gameObject.SetActive(true);
		m.waitingText.text = "Connecting";
		
		if (PhotonNetwork.IsConnected) {
			m.ChangeState(LobbyState.Instance);
			return;
		}
		NetworkManager.Instance.Connect();
	}

	public override void Execute (MainMenu m) {
		m.HandleInput();
		UpdateText(m);
	}

	public override void Exit(MainMenu m) {
		m.waitingText.transform.parent.gameObject.SetActive(false);
		m.lobbyCanvas.SetActive(false);
	}
	
	private void UpdateText(MainMenu m) {
		if ((Time.time - currentTime) > WaitTime) {

			if (counter < 3) {
				m.waitingText.text += ".";
				counter++;
			}
			else {
				m.waitingText.text = "Connecting";
				counter = 0;
			}
			
			currentTime = Time.time;
		}
	}
}