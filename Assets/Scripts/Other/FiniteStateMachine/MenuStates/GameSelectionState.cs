using UnityEngine;

public class GameSelectionState : FSMState<MainMenu> {
	public static GameSelectionState Instance { get; } = new GameSelectionState();

	public override void Enter (MainMenu m) {
		m.menuGameObject.SetActive(true);
		m.mainButtons[1].SetActive(true);
		
		m.SetEventSystemSelectedObject(m.mainButtons[1].transform.GetChild(0).gameObject);
	}

	public override void Execute (MainMenu m) {
		m.HandleInput();
	}

	public override void Exit(MainMenu m) {
		m.menuGameObject.SetActive(false);
		m.mainButtons[1].SetActive(false);
	}
}