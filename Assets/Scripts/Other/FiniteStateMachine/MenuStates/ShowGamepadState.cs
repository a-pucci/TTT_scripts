using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShowGamepadState : FSMState<MainMenu> {
	public static ShowGamepadState Instance { get; } = new ShowGamepadState();
	private const float ChangeWaitTime = 5f;

	public override void Enter (MainMenu m) {
		m.characterSelection.characterSpritesGameObject.SetActive(true);
		m.stageSelection.sceneSelectGameObject.SetActive(true);
		if (PhotonNetwork.IsConnected) {
			SetupOnline(m);
		}
		else {
			SetupOffline(m);
		}
	}

	public override void Execute (MainMenu m) {
		// Do Nothing
	}

	public override void Exit(MainMenu m) {
		// Do Nothing
	}

	private void SetupOnline(MainMenu m) {
		string arenaToLoad = m.gameSceneName;
		
		if (arenaToLoad == Arena.scn_case95.ToString()) {
			m.arenaLoader.SetPlayers(m.characterSelection.characterTypesPC[m.player2SelectedCharacter], m.priority == 1 ? 1 : 0, 
				m.characterSelection.characterTypesPC[m.player1SelectedCharacter], m.priority == 2 ? 1 : 0);
		}
		else {
			m.arenaLoader.SetPlayers(m.characterSelection.characterTypesOCT[m.player2SelectedCharacter], m.priority == 1 ? 1 : 0, 
				m.characterSelection.characterTypesOCT[m.player1SelectedCharacter], m.priority == 2 ? 1 : 0);
		}
		
		m.StartCoroutine(m.buttonsImageArrivingAnimation.Animate(m.buttonsImageRect, () => {
			m.StartCoroutine(WaitTimeBeforeAction(() => {
				if (PhotonNetwork.IsMasterClient) {
					PhotonNetwork.LoadLevel(arenaToLoad);
				}
			}));
		}));
	}

	private void SetupOffline(MainMenu m) {
		if (m.stageSelection.highlightedStage == 0) {
			m.arenaLoader.SetPlayers(m.characterSelection.characterTypesPC[m.player2SelectedCharacter], m.priority == 1 ? 1 : 0, m.characterSelection.characterTypesPC[m.player1SelectedCharacter], m.priority == 2 ? 1 : 0);
		}
		else {
			m.arenaLoader.SetPlayers(m.characterSelection.characterTypesOCT[m.player2SelectedCharacter], m.priority == 1 ? 1 : 0, m.characterSelection.characterTypesOCT[m.player1SelectedCharacter], m.priority == 2 ? 1 : 0);
		}
		m.StartCoroutine(m.buttonsImageArrivingAnimation.Animate(m.buttonsImageRect, () => {
			m.StartCoroutine(WaitInputBeforeAction(() => {
				SceneManager.LoadScene(m.gameSceneName);
			}));
		}));
	}
		
	private static IEnumerator WaitInputBeforeAction(Action action) {
		while (
			!Input.GetButtonDown("Dash1") &&
			!Input.GetButtonDown("Swing1") &&
			!Input.GetButtonDown("Charge1") &&
			!Input.GetButtonDown("Dash2") &&
			!Input.GetButtonDown("Swing2") &&
			!Input.GetButtonDown("Charge2") &&
			!Input.GetMouseButton(0) &&
			!Input.GetMouseButton(1)) {
			yield return null;
		}
		action?.Invoke();
	}

	private static IEnumerator WaitTimeBeforeAction(Action action) {
		yield return new WaitForSeconds(ChangeWaitTime);
		action?.Invoke();
	}
}