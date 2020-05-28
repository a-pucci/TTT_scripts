using Photon.Pun;
using Rewired;
using Rewired.Utils.Platforms.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionState : FSMState<MainMenu> {
	public static CharacterSelectionState Instance { get; } = new CharacterSelectionState();

	public override void Enter (MainMenu m) {
		SetCanvas(m);
		SetInputPlayer(m);
	}

	public override void Execute (MainMenu m) {
		m.characterSelection.HandleInputs();
	}

	public override void Exit(MainMenu m) {
		m.characterSelection.characterSelectGameObject.SetActive(false);
		m.characterSelection.characterSpritesGameObject.SetActive(false);
		m.playerSelectBannerManager.SetActive(false);
		
		m.characterSelection.rightStatBlock.SetActive(false);
		m.characterSelection.leftStatBlock.SetActive(false);
		
		for (int i = 0; i < m.characterSelection.rightStatBlock.transform.childCount; i++) {
			GameObject child = m.characterSelection.rightStatBlock.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(false);
		}
		
		for (int i = 0; i < m.characterSelection.leftStatBlock.transform.childCount; i++) {
			GameObject child = m.characterSelection.leftStatBlock.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(false);
		}
	}
	
	private void SetInputPlayer(MainMenu m) {
		if (PhotonNetwork.InRoom) {
			if (PhotonNetwork.IsMasterClient) {
				m.player1 = ReInput.players.GetPlayer(0);
			}
			else {
				m.player2 = ReInput.players.GetPlayer(0);
			}
		}
	}
		
	private void SetCanvas(MainMenu m) {
		m.characterSelection.characterSelectGameObject.SetActive(true);
		m.characterSelection.characterSpritesGameObject.SetActive(true);
		m.playerSelectBannerManager.SetActive(true);
		foreach (TextMeshProUGUI bannerText in m.playerSelectBanners) {
			bannerText.text = "PLAYER SELECT";
		}

		SetPlayerCanvas(m, "1", m.characterSelection.player1SelectedItems, m.characterSelection.player1CharacterSprite, ref m.player1SelectedCharacter, 
			m.characterSelection.player1CurrentlyHighlighted, m.characterSelection.leftStatBlock, m.leftStatBlockPlayerText);
		SetPlayerCanvas(m, "2", m.characterSelection.player2SelectedItems, m.characterSelection.player2CharacterSprite, ref m.player2SelectedCharacter, 
			m.characterSelection.player2CurrentlyHighlighted, m.characterSelection.rightStatBlock, m.rightStatBlockPlayerText);
	}
	
	private void SetPlayerCanvas(MainMenu m,string player, GameObject[] selectedItems, Image[] characterSprites, 
		ref int selected, int highlighted, GameObject statBlock, GameObject blockText) {
		characterSprites[highlighted].gameObject.SetActive(true);
		foreach (GameObject item in selectedItems) {
			item.SetActive(false);
		}
		
		for (int i = 0; i < characterSprites.Length; i++) {
			characterSprites[i].sprite = m.characterSelection.characterSelectionSprites[i];
		}

		selected = -1;
		m.priority = 0; 
		if(player == "1") m.characterSelection.SetSelectionItem("1", 0, false); 
		else m.characterSelection.SetSelectionItem("2", m.characterSelection.player2SelectionItems.Length - 1, false);
		
		
		statBlock.SetActive(true);
		statBlock.ActivateChildren(true);
		blockText.SetActive(false);
	}
}