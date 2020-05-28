using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalState : FSMState<MainMenu> {
	public static GlobalState Instance { get; } = new GlobalState();

	public override void Enter (MainMenu m) {
		m.currentSelectedGameObject = m.eventSystem.firstSelectedGameObject;
		m.player1 = Rewired.ReInput.players.GetPlayer(0);
		m.player2 = Rewired.ReInput.players.GetPlayer(1);
		m.player1.controllers.maps.SetAllMapsEnabled(false);
		m.player1.controllers.maps.SetMapsEnabled(true, Rewired.ControllerType.Joystick, RewiredConsts.Category.Default, RewiredConsts.Layout.Joystick.Menu);
		m.player2.controllers.maps.SetAllMapsEnabled(false);
		m.player2.controllers.maps.SetMapsEnabled(true, Rewired.ControllerType.Joystick, RewiredConsts.Category.Default, RewiredConsts.Layout.Joystick.Menu);
	}

	public override void Execute (MainMenu m) {
#if UNITY_EDITOR || INTERNAL_BUILD
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4)) {
			int value = Input.GetKeyDown(KeyCode.Alpha1) ? 0 : Input.GetKeyDown(KeyCode.Alpha2) ? 1 : Input.GetKeyDown(KeyCode.Alpha3) ? 2 : Input.GetKeyDown(KeyCode.Alpha4) ? 3 : -1;
			if (value >= 0) {
				if (m.player1SelectedCharacter < 0) {
					m.characterSelection.SetSelectionItem("1", value);
					m.characterSelection.SetSelectedItem("1", m.characterSelection.player1CurrentlyHighlighted);
				}
				else if (m.player2SelectedCharacter < 0) {
					m.characterSelection.SetSelectionItem("2", value);
					m.characterSelection.SetSelectedItem("2", m.characterSelection.player2CurrentlyHighlighted);
				}
			}
		}
#endif
	}

	public override void Exit(MainMenu m) {Debug.Log("Exit: " + Instance);
		// Do Nothing
	}
}