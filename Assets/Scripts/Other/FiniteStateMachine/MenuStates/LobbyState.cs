using UnityEngine;

public class LobbyState : FSMState<MainMenu> {
	public static LobbyState Instance { get; } = new LobbyState();

	public override void Enter (MainMenu m) {
		m.lobbyCanvas.SetActive(true);
		m.menuGameObject.SetActive(true);
		m.matchTypeButtons[0].SetActive(true);
		m.matchTypeButtons[1].SetActive(true);
		
		m.SetEventSystemSelectedObject(m.matchTypeButtons[0]);
		NetworkManager.Instance.LeaveRoom();
	}

	public override void Execute (MainMenu m) {
		 m.HandleInput();
	}

	public override void Exit(MainMenu m) {
		m.nicknameSetting.Esc();
		m.matchTypeButtons[0].SetActive(false);
		m.matchTypeButtons[1].SetActive(false);
		m.lobbyCanvas.SetActive(false);
	}
}