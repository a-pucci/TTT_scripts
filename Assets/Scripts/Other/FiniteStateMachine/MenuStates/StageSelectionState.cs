using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Rewired;
using TMPro;
using UnityEngine;

public class StageSelectionState : FSMState<MainMenu> {
	public static StageSelectionState Instance { get; } = new StageSelectionState();


	public override void Enter (MainMenu m) {

		SetCanvas(m);
	}

	public override void Execute (MainMenu m) {
		m.stageSelection.HandleInputs();
	}
	
	public override void Exit(MainMenu m) {
		m.characterSelection.characterSpritesGameObject.SetActive(false);
		m.stageSelection.sceneSelectGameObject.SetActive(false);
		m.rightStatBlockPlayerText.SetActive(false);
		m.leftStatBlockPlayerText.SetActive(false);
		m.playerSelectBannerManager.SetActive(false);
	}

	private void SetCanvas(MainMenu m) {
		m.characterSelection.characterSpritesGameObject.SetActive(true);
		m.stageSelection.sceneSelectGameObject.SetActive(true);
		m.stageSelection.highlightedStage = 0;
		m.stageSelection.stages[m.stageSelection.highlightedStage].SetActive(true);

		m.characterSelection.rightStatBlock.SetActive(true);
		m.characterSelection.leftStatBlock.SetActive(true);
		
		m.rightStatBlockPlayerText.SetActive(true);
		m.leftStatBlockPlayerText.SetActive(true);
		
		m.playerSelectBannerManager.SetActive(true);
		foreach (TextMeshProUGUI bannerText in m.playerSelectBanners) {
			bannerText.text = "ARENA SELECT";
		}
		m.stageSelection.arenaNameText.text = m.stageSelection.arenaNames[0];
	}
}