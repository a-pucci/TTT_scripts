using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainState : FSMState<MainMenu> {
	public static MainState Instance { get; } = new MainState();

	public override void Enter (MainMenu m) {
		m.menuGameObject.SetActive(true);
		m.mainButtons[0].SetActive(true);
		
		m.SetEventSystemSelectedObject(m.mainButtons[0].transform.GetChild(0).gameObject);
	}

	public override void Execute (MainMenu m) {
		m.HandleInput();
	}

	public override void Exit(MainMenu m) {
		m.menuGameObject.SetActive(false);
		m.mainButtons[0].SetActive(false);
	}
}