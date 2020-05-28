using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState : FSMState<MainMenu> {
	public static RoomState Instance { get; } = new RoomState();

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