using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviourPun {
	public MainMenu menu;
	
	[FoldoutGroup("Character Selection")] public GameObject[] player1SelectionItems;
	[FoldoutGroup("Character Selection")] public GameObject[] player2SelectionItems;
	[FoldoutGroup("Character Selection")] public GameObject[] player1SelectedItems;
	[FoldoutGroup("Character Selection")] public GameObject[] player2SelectedItems;
	[FoldoutGroup("Character Selection")] public Image[] player1CharacterSprite;
	[FoldoutGroup("Character Selection")] public Image[] player2CharacterSprite;
	[FoldoutGroup("Character Selection")] public PlayerType[] characterTypesPC;
	[FoldoutGroup("Character Selection")] public PlayerType[] characterTypesOCT;
	[FoldoutGroup("Character Selection")] public Sprite[] characterSelectionSprites;
	[FoldoutGroup("Character Selection")] public GameObject characterSelectGameObject;
	[FoldoutGroup("Character Selection")] public GameObject characterSpritesGameObject;
	

	[FoldoutGroup("Stats")] public GameObject rightStatBlock;
	[FoldoutGroup("Stats")] public GameObject leftStatBlock;
	[FoldoutGroup("Stats")] public Sprite disabledStat;
	[FoldoutGroup("Stats")] public Sprite enabledStat;
	[FoldoutGroup("Stats")] public List<Image> leftStatBlockStat1;
	[FoldoutGroup("Stats")] public List<Image> leftStatBlockStat2;
	[FoldoutGroup("Stats")] public List<Image> leftStatBlockStat3;
	[FoldoutGroup("Stats")] public List<Image> rightStatBlockStat1;
	[FoldoutGroup("Stats")] public List<Image> rightStatBlockStat2;
	[FoldoutGroup("Stats")] public List<Image> rightStatBlockStat3;
	
	[HideInInspector] public int player1CurrentlyHighlighted;
	[HideInInspector] public int player2CurrentlyHighlighted;

	[PunRPC]
	public void SetSelectionItem(string player, int selection, bool playSfx = true) {
		switch (player) {
			case "1":
				var statBlocks1 = new List<List<Image>> {leftStatBlockStat1, leftStatBlockStat2, leftStatBlockStat3};
				SetSelection(ref selection, player1SelectionItems, player1CharacterSprite, ref player1CurrentlyHighlighted, statBlocks1, playSfx);
				break;
			
			case "2":
				var statBlocks2 = new List<List<Image>> {rightStatBlockStat1, rightStatBlockStat2, rightStatBlockStat3};
				SetSelection(ref selection, player2SelectionItems, player2CharacterSprite, ref player2CurrentlyHighlighted, statBlocks2, playSfx);
				break;
		}
	}

	private void SetSelection(ref int selection, GameObject[] playerItems, Image[] sprites, ref int highlighted, 
		List<List<Image>> blocksStats, bool playSfx) {
		
		if (selection > playerItems.Length - 1) {
			selection = playerItems.Length - 1;
		}
		else if (selection < 0) {
			selection = 0;
		}
		else if (playSfx) {
			AudioManager.instance.PlaySound(menu.changeSfx);
		}
		sprites[highlighted].gameObject.SetActive(false);
		playerItems[highlighted].SetActive(false);
		sprites[selection].gameObject.SetActive(true);
		playerItems[selection].SetActive(true);

		highlighted = selection;

		SetStatsBlock(blocksStats[0], characterTypesPC[selection].stat1 - 1);
		SetStatsBlock(blocksStats[1], characterTypesPC[selection].stat2 - 1);
		SetStatsBlock(blocksStats[2], characterTypesPC[selection].stat3 - 1);
	}

	private void SetStatsBlock(List<Image> block, int stat) {
		
		if (stat < 0) { stat = 0; }
		if (stat > 4) { stat = 4; }
		for (int i = 0; i < stat; i++) {
			block[i].sprite = enabledStat;
		}
		for (int i = stat; i < 4; i++) {
			block[i].sprite = disabledStat;
		}
	}
	
	[PunRPC]
	public void SetSelectedItem(string player, int selection) {
		switch (player) {
			case "1": 
				SetSelected(selection, player1SelectedItems, ref menu.player2SelectedCharacter, player2CharacterSprite, ref menu.player1SelectedCharacter, player1CurrentlyHighlighted);
				break;
			
			case "2": 
				SetSelected(selection, player2SelectedItems, ref menu.player1SelectedCharacter, player1CharacterSprite, ref menu.player2SelectedCharacter, player2CurrentlyHighlighted);
				break;
		}
	}

	private void SetSelected(int selection, GameObject[] playerItems, ref int opponentSelection, Image[] opponentSprites, ref int selectedCharacter, int currentlyHighlighted) {
		if (selection >= 0) {
			AudioManager.instance.PlaySound(menu.selectionSfx);
			playerItems[selection].gameObject.SetActive(true);
			
			if (opponentSelection == -1) {
				opponentSprites[selection].sprite = characterSelectionSprites[selection + characterSelectionSprites.Length/2];
			}
			selectedCharacter = selection;
			menu.priority = selection == opponentSelection ? 2 : 0;
		}
		else {
			AudioManager.instance.PlaySound(menu.cancelSfx);
			if (selectedCharacter < 0) {
				NetworkManager.Instance.LeaveRoom();
				menu.RevertState();
			}
			else {
				playerItems[currentlyHighlighted].gameObject.SetActive(false);
				opponentSprites[currentlyHighlighted].sprite = characterSelectionSprites[currentlyHighlighted];
				selectedCharacter = -1;
			}
		}
	}

	public void HandleInputs() {
		if (PhotonNetwork.IsConnected) {
			HandleInputsOnline();
		}
		else {
			HandleInputsOffline();
		}
	}
	
	private void HandleInputsOnline() {

		if (PhotonNetwork.IsMasterClient) {
			HandlePlayerOnline(menu.player1, "1", player1CurrentlyHighlighted, ref menu.player1SelectedCharacter);
			
			if (menu.player1SelectedCharacter >= 0 && menu.player2SelectedCharacter >= 0) {
				menu.photonView.RPC("GoToStageSelection", RpcTarget.All);
			}
		}
		else {
			HandlePlayerOnline(menu.player2, "2", player2CurrentlyHighlighted, ref menu.player2SelectedCharacter);
		}
	}

	private void HandlePlayerOnline(Player player, string playerIndex, int currentHighlight, ref int selectedCharacter) {
		if (selectedCharacter < 0) {
			if (player.GetButtonDown(RewiredConsts.Action.Select)) {
				photonView.RPC("SetSelectedItem", RpcTarget.All, playerIndex, currentHighlight);
			}
			if (player.GetButtonRepeating(RewiredConsts.Action.MenuDown)) {
				int temp1 = currentHighlight + 1;
				photonView.RPC("SetSelectionItem", RpcTarget.All, playerIndex, temp1, true);
			}
			if (player.GetButtonRepeating(RewiredConsts.Action.MenuUp)) {
				int temp2 = currentHighlight - 1;
				photonView.RPC("SetSelectionItem", RpcTarget.All, playerIndex, temp2, true);
			}
		}
		if (player.GetButtonDown(RewiredConsts.Action.Back)) {
			photonView.RPC("SetSelectedItem", RpcTarget.All, playerIndex, -1);
			selectedCharacter = -1;
		}
	}

	private void HandleInputsOffline() {
		HandlePlayerOffline(menu.player1, "1", player1CurrentlyHighlighted, ref menu.player1SelectedCharacter);
		HandlePlayerOffline(menu.player2, "2", player2CurrentlyHighlighted, ref menu.player2SelectedCharacter);

		if (menu.player1SelectedCharacter >= 0 && menu.player2SelectedCharacter >= 0) {
			menu.ChangeState(StageSelectionState.Instance);
		}
	}
	
	private void HandlePlayerOffline(Player player, string playerIndex, int currentHighlight, ref int selectedCharacter) {
		if (selectedCharacter < 0) {
			if (player.GetButtonDown(RewiredConsts.Action.Select)) {
				SetSelectedItem(playerIndex, currentHighlight);
			}
			if (player.GetButtonDown(RewiredConsts.Action.MenuDown)) {
				SetSelectionItem(playerIndex, currentHighlight + 1);
			}
			if (player.GetButtonDown(RewiredConsts.Action.MenuUp)) {
				SetSelectionItem(playerIndex, currentHighlight - 1);
			}
		}
		if (player.GetButtonDown(RewiredConsts.Action.Back)) {
			SetSelectedItem(playerIndex, -1);
			selectedCharacter = -1;
		}
	}
}