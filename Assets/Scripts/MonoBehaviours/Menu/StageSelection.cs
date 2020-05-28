using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Rewired;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class StageSelection : MonoBehaviourPun {

	public MainMenu menu;
	
	[FoldoutGroup("Stage Selection")]public GameObject[] stages;
	[FoldoutGroup("Stage Selection")] public GameObject sceneSelectGameObject;
	[FoldoutGroup("Stage Selection")] public GameObject rightArrow;
	[FoldoutGroup("Stage Selection")] public GameObject leftArrow;

	[FoldoutGroup("Stage Selection")] public List<string> arenaNames;
	[FoldoutGroup("Stage Selection")] public TextMeshProUGUI arenaNameText;
	
	[HideInInspector] public int highlightedStage;
	
	public void HandleInputs() {
		if (PhotonNetwork.InRoom) {
			HandleInputsOnline();
		}
		else {
			HandleInputsOffline();
		}
	}
	
	private void HandleInputsOnline() {
		Player player = menu.player1 ?? menu.player2;

		if (player.GetButtonRepeating(RewiredConsts.Action.MenuRight) && highlightedStage < stages.Length-1) {
			photonView.RPC("ScrollArenaRight", RpcTarget.All);
		}
		if (player.GetButtonRepeating(RewiredConsts.Action.MenuLeft) && highlightedStage > 0) {
			photonView.RPC("ScrollArenaLeft", RpcTarget.All);
		}
		if (player.GetButtonDown(RewiredConsts.Action.Select)) {
			photonView.RPC("SelectArena", RpcTarget.All);
		}
		if (player.GetButtonDown(RewiredConsts.Action.Back)) {
			menu.photonView.RPC("RevertState", RpcTarget.All, 3);
		}
	}

	private void HandleInputsOffline() {
		if ((menu.player1.GetButtonRepeating(RewiredConsts.Action.MenuRight) || menu.player2.GetButtonRepeating(RewiredConsts.Action.MenuRight)) && highlightedStage < stages.Length-1) {
			ScrollArenaRight();
		}
		if ((menu.player1.GetButtonRepeating(RewiredConsts.Action.MenuLeft) || menu.player2.GetButtonRepeating(RewiredConsts.Action.MenuLeft)) && highlightedStage > 0) {
			ScrollArenaLeft();
		}
		if (menu.player1.GetButtonDown(RewiredConsts.Action.Select) || menu.player2.GetButtonDown(RewiredConsts.Action.Select)) {
			SelectArena();
		}
		if (menu.player1.GetButtonDown(RewiredConsts.Action.Back) || menu.player2.GetButtonDown(RewiredConsts.Action.Back)) {
			menu.RevertState();
		}
	}
	
	[PunRPC]
	public void ScrollArenaLeft() {
		AudioManager.instance.PlaySound(menu.changeSfx);
		stages[highlightedStage--].SetActive(false);
		stages[highlightedStage].SetActive(true);
		leftArrow.SetActive(highlightedStage > 0);
		rightArrow.SetActive(highlightedStage < stages.Length - 1);
		arenaNameText.text = arenaNames[highlightedStage];
		menu.frameSkip = 5;
	}

	[PunRPC]
	public void ScrollArenaRight() {
		AudioManager.instance.PlaySound(menu.changeSfx);
		stages[highlightedStage++].SetActive(false);
		stages[highlightedStage].SetActive(true);
		leftArrow.SetActive(highlightedStage > 0);
		rightArrow.SetActive(highlightedStage < stages.Length - 1);
		arenaNameText.text = arenaNames[highlightedStage];
	}

	[PunRPC]
	public void SelectArena() {
		AudioManager.instance.PlaySound(menu.selectionSfx);
		menu.gameSceneName = stages[highlightedStage].name;
		menu.eventSystem.enabled = false;
		menu.StartCoroutine(menu.confirmSelection.Animate(menu.analogGlitch));
		menu.ChangeState(ShowGamepadState.Instance);
	}
}