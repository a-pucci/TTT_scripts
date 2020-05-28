using Pada1.BBCore.Actions;
using Photon.Pun;
using UnityEngine;

public class MatchingState : FSMState<MainMenu> {
	public static MatchingState Instance { get; } = new MatchingState();
	
	private const float WaitTime = 0.5f;
	private int counter ;
	private float currentTime;
	private int elapsedTime;
	private string text;

	public override void Enter (MainMenu m) {
		NetworkManager.Instance.roomType = RoomType.Random;
		text = "Creating room";
		m.lobbyCanvas.SetActive(true);
		m.waitingText.transform.parent.gameObject.SetActive(true);
		m.waitingText.text = text;
	}

	public override void Execute (MainMenu m) {
		if (PhotonNetwork.InRoom) {
			m.HandleInput();
			text = "Finding match";
		}
		UpdateText(m);
	}

	public override void Exit(MainMenu m) {
		m.waitingText.transform.parent.gameObject.SetActive(false);
		m.menuGameObject.SetActive(false);
		m.lobbyCanvas.SetActive(false);
	}
	
	private void UpdateText(MainMenu m) {

		if ((Time.time - currentTime) > WaitTime) {
			string points = "";
			for (int i = 0; i < counter; i++) {
				points += ".";
			}
			counter++;
			if (counter >= 4) counter = 0;

			m.waitingText.text = text + points;
			
			currentTime = Time.time;
		}
	}

}