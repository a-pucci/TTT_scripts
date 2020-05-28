using System;
using System.Collections;
using System.Collections.Generic;
using Kino;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	public AnalogGlitch analogGlitch;
	public EventSystem eventSystem;
	public StandaloneInputModule inputModule;
	public AnalogGlitchTransition changeSelection;
	public AnalogGlitchTransition confirmSelection;
	public AnalogGlitchTransition resetTransition;
	public RectTransform buttonsImageRect;
	public RectAnchorAnimation buttonsImageArrivingAnimation;
	public string gameSceneName;
	public GameObject[] player1SelectionItems;
	public GameObject[] player2SelectionItems;
	public GameObject[] player1SelectedItems;
	public GameObject[] player2SelectedItems;
	public Image[] player1CharacterSprite;
	public Image[] player2CharacterSprite;
	public PlayerType[] characterTypesPC;
	public PlayerType[] characterTypesOCT;
	public GameObject[] stages;
	public Sprite[] characterSelectionSprites;
	public GameObject menuGameObject;
	public GameObject characterSelectGameObject;
	public GameObject characterSpritesGameObject;
	public GameObject sceneSelectGameObject;
	public GameObject rightArrow;
	public GameObject leftArrow;
	public GameObject rightStatBlock;
	public GameObject rightStatBlockPlayerText;
	public GameObject leftStatBlock;
	public GameObject leftStatBlockPlayerText;
	public Sprite disabledStat;
	public Sprite enabledStat;
	public List<Image> leftStatBlockStat1;
	public List<Image> leftStatBlockStat2;
	public List<Image> leftStatBlockStat3;
	public List<Image> rightStatBlockStat1;
	public List<Image> rightStatBlockStat2;
	public List<Image> rightStatBlockStat3;
	public List<string> arenaNames;
	public TextMeshProUGUI arenaNameText;
	public GameObject playerSelectBannerManager;
	public List<TextMeshProUGUI> playerSelectBanners;
	public ArenaLoader arenaLoader;
	private int player1CurrentlyHighlighted;
	private int player2CurrentlyHighlighted;
	private int highlightedStage;
	public int player1SelectedCharacter;
	public int player2SelectedCharacter;
	private bool characterSelectMode;
	private bool stageSelectMode;
	private int frameSkip;
	public int priority;
	private bool waitForAxisReset1;
	private bool waitForAxisReset2;
	//public Coroutine currentChangeTransition;
	public AudioClip selectionSfx;
	public AudioClip cancelSfx;
	public AudioClip changeSfx;

	private GameObject currentSelectedGameObject;
	private Rewired.Player player1, player2;

	private void Start() {
		currentSelectedGameObject = eventSystem.firstSelectedGameObject;
		player1 = Rewired.ReInput.players.GetPlayer(0);
		player2 = Rewired.ReInput.players.GetPlayer(1);
		player1.controllers.maps.SetAllMapsEnabled(false);
		player1.controllers.maps.SetMapsEnabled(true, Rewired.ControllerType.Joystick, RewiredConsts.Category.Default, RewiredConsts.Layout.Joystick.Menu);
		player2.controllers.maps.SetAllMapsEnabled(false);
		player2.controllers.maps.SetMapsEnabled(true, Rewired.ControllerType.Joystick, RewiredConsts.Category.Default, RewiredConsts.Layout.Joystick.Menu);
	}

	// Update is called once per frame
	private void Update () {
#if UNITY_EDITOR || INTERNAL_BUILD
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4)) {
			int value = Input.GetKeyDown(KeyCode.Alpha1) ? 0 : Input.GetKeyDown(KeyCode.Alpha2) ? 1 : Input.GetKeyDown(KeyCode.Alpha3) ? 2 : Input.GetKeyDown(KeyCode.Alpha4) ? 3 : -1;
			if (value >= 0) {
				if (player1SelectedCharacter < 0) {
					player1CurrentlyHighlighted = SetSelectionItem("1", value);
					SetSelectedItem("1", player1CurrentlyHighlighted);
				}
				else if (player2SelectedCharacter < 0) {
					player2CurrentlyHighlighted = SetSelectionItem("2", value);
					SetSelectedItem("2", player2CurrentlyHighlighted);
				}
			}
		}
#endif
		if (frameSkip <= 0) {
			if (characterSelectMode) {
				if (player1SelectedCharacter < 0) {
					if (player1.GetButtonDown(RewiredConsts.Action.Select)) {
						SetSelectedItem("1", player1CurrentlyHighlighted);
					}
					if (player1.GetButtonRepeating(RewiredConsts.Action.MenuDown)) {
						player1CurrentlyHighlighted = SetSelectionItem("1", player1CurrentlyHighlighted + 1);
					}
					if (player1.GetButtonRepeating(RewiredConsts.Action.MenuUp)) {
						player1CurrentlyHighlighted = SetSelectionItem("1", player1CurrentlyHighlighted - 1);
					}
				}
				if (player1.GetButtonDown(RewiredConsts.Action.Back)) {
					SetSelectedItem("1", -1);
					player1SelectedCharacter = -1;
				}
				if (player2SelectedCharacter < 0) {
					if (player2.GetButtonDown(RewiredConsts.Action.Select)) {
						SetSelectedItem("2", player2CurrentlyHighlighted);
					}
					if (player2.GetButtonRepeating(RewiredConsts.Action.MenuDown)) {
						player2CurrentlyHighlighted = SetSelectionItem("2", player2CurrentlyHighlighted + 1);
					}
					if (player2.GetButtonRepeating(RewiredConsts.Action.MenuUp)) {
						player2CurrentlyHighlighted = SetSelectionItem("2", player2CurrentlyHighlighted - 1);
					}
				}
				if (player2.GetButtonDown(RewiredConsts.Action.Back)) {
					SetSelectedItem("2", -1);
					player2SelectedCharacter = -1;
				}
				if (player1SelectedCharacter >= 0 && player2SelectedCharacter >= 0) {
					SetupStageSelectMode();
				}
			}
			else if (stageSelectMode) {
				if ((player1.GetButtonRepeating(RewiredConsts.Action.MenuRight) || player2.GetButtonRepeating(RewiredConsts.Action.MenuRight)) && highlightedStage < stages.Length-1) {
					AudioManager.instance.PlaySound(changeSfx);
					stages[highlightedStage++].SetActive(false);
					stages[highlightedStage].SetActive(true);
					leftArrow.SetActive(highlightedStage > 0);
					rightArrow.SetActive(highlightedStage < stages.Length - 1);
					arenaNameText.text = arenaNames[highlightedStage];
				}
				if ((player1.GetButtonRepeating(RewiredConsts.Action.MenuLeft) || player2.GetButtonRepeating(RewiredConsts.Action.MenuLeft)) && highlightedStage > 0) {
					AudioManager.instance.PlaySound(changeSfx);
					stages[highlightedStage--].SetActive(false);
					stages[highlightedStage].SetActive(true);
					leftArrow.SetActive(highlightedStage > 0);
					rightArrow.SetActive(highlightedStage < stages.Length - 1);
					arenaNameText.text = arenaNames[highlightedStage];
					frameSkip = 5;
				}
				if (player1.GetButtonDown(RewiredConsts.Action.Select) || player2.GetButtonDown(RewiredConsts.Action.Select)) {
					AudioManager.instance.PlaySound(selectionSfx);
					characterSelectMode = false;
					stageSelectMode = false;
					gameSceneName = stages[highlightedStage].name;
					eventSystem.enabled = false;
					StartCoroutine(confirmSelection.Animate(analogGlitch));
					confirmSelection.AnimationFinished = ShowGamepadCard;
				}
				if (player1.GetButtonDown(RewiredConsts.Action.Back) || player2.GetButtonDown(RewiredConsts.Action.Back)) {
					AudioManager.instance.PlaySound(cancelSfx);
					SetupCharacterSelectMode();
				}
			}
			else {
				if (eventSystem.currentSelectedGameObject != currentSelectedGameObject && eventSystem.currentSelectedGameObject != null) {
					currentSelectedGameObject = eventSystem.currentSelectedGameObject;
					StartCoroutine(changeSelection.Animate(analogGlitch));
				}
				if (!eventSystem.currentSelectedGameObject && (ReadAnyControllerInput("1") || ReadAnyControllerInput("2"))) {
					eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
				}
				if (ReadAnyControllerInput("1")) {
					inputModule.verticalAxis = "Vertical1";
					inputModule.horizontalAxis = "Horizontal1";
					inputModule.submitButton = "Swing1";
				}
				else if (ReadAnyControllerInput("2")) {
					inputModule.verticalAxis = "Vertical2";
					inputModule.horizontalAxis = "Horizontal2";
					inputModule.submitButton = "Swing2";
				}
			}
		}
		else {
			frameSkip--;
		}
	}

	public void Play(string sceneName) {
		//eventSystem.enabled = false;
		//StartCoroutine(confirmSelection.Animate(analogGlitch));
		//confirmSelection.AnimationFinished = SetupCharacterSelectMode;
		frameSkip = 2;
		AudioManager.instance.PlaySound(selectionSfx);
		SetupCharacterSelectMode();
	}

	public void Exit() {
		eventSystem.enabled = false;
		StartCoroutine(confirmSelection.Animate(analogGlitch));
		confirmSelection.AnimationFinished = Application.Quit;
	}

	public void ShowGamepadCard() {
		if (highlightedStage == 0) {
			arenaLoader.SetPlayers(characterTypesPC[player2SelectedCharacter], priority == 1 ? 1 : 0, characterTypesPC[player1SelectedCharacter], priority == 2 ? 1 : 0);
		}
		else {
			arenaLoader.SetPlayers(characterTypesOCT[player2SelectedCharacter], priority == 1 ? 1 : 0, characterTypesOCT[player1SelectedCharacter], priority == 2 ? 1 : 0);
		}
		StartCoroutine(buttonsImageArrivingAnimation.Animate(buttonsImageRect, () => {
			StartCoroutine(ReadAffermativeInput(() => {
				SceneManager.LoadScene(gameSceneName);
			}));
		}));
	}

	private IEnumerator ReadAffermativeInput(Action inputAction) {
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
		inputAction?.Invoke();
	}
	
	private bool ReadAnyControllerInput(string number) {
		return Input.GetButtonDown("Dash" + number) ||
		       Input.GetButtonDown("Swing" + number) ||
		       Input.GetButtonDown("Charge" + number) ||
		       Input.GetAxis("Horizontal" + number) > 0.19f ||
		       Input.GetAxis("Vertical" + number) > 0.19f ||
		       Input.GetAxis("Horizontal" + number) < -0.19f ||
		       Input.GetAxis("Vertical" + number) < -0.19f;

	}

	private void SetupStageSelectMode() {
		characterSelectMode = false;
		stageSelectMode = true;
		menuGameObject.SetActive(false);
		characterSelectGameObject.SetActive(false);
		sceneSelectGameObject.SetActive(true);
		highlightedStage = 0;
		stages[highlightedStage].SetActive(true);

		for (int i = 0; i < rightStatBlock.transform.childCount; i++) {
			var child = rightStatBlock.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(false);
		}
		rightStatBlockPlayerText.SetActive(true);

		for (int i = 0; i < leftStatBlock.transform.childCount; i++) {
			var child = leftStatBlock.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(false);
		}
		leftStatBlockPlayerText.SetActive(true);
		playerSelectBannerManager.SetActive(true);
		foreach (TextMeshProUGUI bannerText in playerSelectBanners) {
			bannerText.text = "ARENA SELECT";
		}
		arenaNameText.text = arenaNames[0];
	}

	private void SetupMainMenuMode() {
		characterSelectMode = false;
		stageSelectMode = false;
		menuGameObject.SetActive(true);
		sceneSelectGameObject.SetActive(false);
		characterSelectGameObject.SetActive(false);
		characterSpritesGameObject.SetActive(false);
		eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
		currentSelectedGameObject = eventSystem.firstSelectedGameObject;
		eventSystem.firstSelectedGameObject.GetComponent<Animator>().SetTrigger("Highlighted");
		playerSelectBannerManager.SetActive(false);
		rightStatBlock.SetActive(false);
		leftStatBlock.SetActive(false);
	}

	private void SetupCharacterSelectMode() {
		characterSelectMode = true;
		stageSelectMode = false;
		menuGameObject.SetActive(false);
		sceneSelectGameObject.SetActive(false);
		characterSelectGameObject.SetActive(true);
		characterSpritesGameObject.SetActive(true);
		foreach (GameObject item in player1SelectedItems) {
			item.SetActive(false);
		}
		foreach (GameObject item in player2SelectedItems) {
			item.SetActive(false);
		}
		for (int i = 0; i < player1CharacterSprite.Length; i++) {
			player1CharacterSprite[i].sprite = characterSelectionSprites[i];
		}
		for (int i = 0; i < player2CharacterSprite.Length; i++) {
			player2CharacterSprite[i].sprite = characterSelectionSprites[i];
		}
		player1SelectedCharacter = -1;
		player2SelectedCharacter = -1;
		priority = 0; 
		player1CurrentlyHighlighted = SetSelectionItem("1", 0, false);
		player2CurrentlyHighlighted = SetSelectionItem("2", player2SelectionItems.Length - 1, false);
		player1CharacterSprite[player1CurrentlyHighlighted].gameObject.SetActive(true);
		player2CharacterSprite[player2CurrentlyHighlighted].gameObject.SetActive(true);
		rightStatBlock.SetActive(true);
		for (int i = 0; i < rightStatBlock.transform.childCount; i++) {
			var child = rightStatBlock.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(true);
		}
		rightStatBlockPlayerText.SetActive(false);
		leftStatBlock.SetActive(true);
		for (int i = 0; i < leftStatBlock.transform.childCount; i++) {
			var child = leftStatBlock.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(true);
		}
		leftStatBlockPlayerText.SetActive(false);
		playerSelectBannerManager.SetActive(true);
		foreach (TextMeshProUGUI bannerText in playerSelectBanners) {
			bannerText.text = "PLAYER SELECT";
		}
	}

	private int SetSelectionItem(string player, int selection, bool playSFX = true) {
		if (player == "1") {
			if (selection > player1SelectionItems.Length - 1) {
				selection = player1SelectionItems.Length - 1;
			}
			else if (selection < 0) {
				selection = 0;
			}
			else if (playSFX) {
				AudioManager.instance.PlaySound(changeSfx);
			}
			player1CharacterSprite[player1CurrentlyHighlighted].gameObject.SetActive(false);
			player1SelectionItems[player1CurrentlyHighlighted].SetActive(false);
			player1CharacterSprite[selection].gameObject.SetActive(true);
			player1SelectionItems[selection].SetActive(true);

			byte stat1 = characterTypesPC[selection].stat1;
			byte stat2 = characterTypesPC[selection].stat2;
			byte stat3 = characterTypesPC[selection].stat3;

			stat1 -= 1;
			stat2 -= 1;
			stat3 -= 1;

			if (stat1 < 0) { stat1 = 0; }
			if (stat1 > 4) { stat1 = 4; }
			for (int i = 0; i < stat1; i++) {
				leftStatBlockStat1[i].sprite = enabledStat;
			}
			for (int i = stat1; i < 4; i++) {
				leftStatBlockStat1[i].sprite = disabledStat;
			}

			if (stat2 < 0) { stat2 = 0; }
			if (stat2 > 4) { stat2 = 4; }
			for (int i = 0; i < stat2; i++) {
				leftStatBlockStat2[i].sprite = enabledStat;
			}
			for (int i = stat2; i < 4; i++) {
				leftStatBlockStat2[i].sprite = disabledStat;
			}

			if (stat3 < 0) { stat3 = 0; }
			if (stat3 > 4) { stat3 = 4; }
			for (int i = 0; i < stat3; i++) {
				leftStatBlockStat3[i].sprite = enabledStat;
			}
			for (int i = stat3; i < 4; i++) {
				leftStatBlockStat3[i].sprite = disabledStat;
			}
		}
		else if (player == "2") {
			if (selection > player2SelectionItems.Length - 1) {
				selection = player2SelectionItems.Length - 1;
			}
			else if (selection < 0) {
				selection = 0;
			}
			else if (playSFX) {
				AudioManager.instance.PlaySound(changeSfx);
			}
			player2SelectionItems[player2CurrentlyHighlighted].SetActive(false);
			player2SelectionItems[selection].SetActive(true);
			player2CharacterSprite[player2CurrentlyHighlighted].gameObject.SetActive(false);
			player2CharacterSprite[selection].gameObject.SetActive(true);

			byte stat1 = characterTypesPC[selection].stat1;
			byte stat2 = characterTypesPC[selection].stat2;
			byte stat3 = characterTypesPC[selection].stat3;

			stat1 -= 1;
			stat2 -= 1;
			stat3 -= 1;

			if (stat1 < 0) { stat1 = 0; }
			if (stat1 > 4) { stat1 = 4; }
			for (int i = 0; i < stat1; i++) {
				rightStatBlockStat1[i].sprite = enabledStat;
			}
			for (int i = stat1; i < 4; i++) {
				rightStatBlockStat1[i].sprite = disabledStat;
			}

			if (stat2 < 0) { stat2 = 0; }
			if (stat2 > 4) { stat2 = 4; }
			for (int i = 0; i < stat2; i++) {
				rightStatBlockStat2[i].sprite = enabledStat;
			}
			for (int i = stat2; i < 4; i++) {
				rightStatBlockStat2[i].sprite = disabledStat;
			}

			if (stat3 < 0) { stat3 = 0; }
			if (stat3 > 4) { stat3 = 4; }
			for (int i = 0; i < stat3; i++) {
				rightStatBlockStat3[i].sprite = enabledStat;
			}
			for (int i = stat3; i < 4; i++) {
				rightStatBlockStat3[i].sprite = disabledStat;
			}
		}
		return selection;
	}

	private int SetSelectedItem(string player, int selection) {
		if (player == "1") {
			if (selection >= 0) {
				AudioManager.instance.PlaySound(selectionSfx);
				player1SelectedItems[selection].gameObject.SetActive(true);
				if (player2SelectedCharacter == -1) {
					player2CharacterSprite[selection].sprite = characterSelectionSprites[selection + characterSelectionSprites.Length/2];
				}
				player1SelectedCharacter = selection;
				priority = selection == player2SelectedCharacter ? 2 : 0;
			}
			else {
				AudioManager.instance.PlaySound(cancelSfx);
				if (player1SelectedCharacter < 0) {
					SetupMainMenuMode();
				}
				else {
					player1SelectedItems[player1CurrentlyHighlighted].gameObject.SetActive(false);
					player2CharacterSprite[player1CurrentlyHighlighted].sprite = characterSelectionSprites[player1CurrentlyHighlighted];
					player1SelectedCharacter = -1;
				}
			}
		}
		else if (player == "2") {
			if (selection >= 0) {
				AudioManager.instance.PlaySound(selectionSfx);
				player2SelectedItems[selection].gameObject.SetActive(true);
				if (player1SelectedCharacter == -1) {
					player1CharacterSprite[selection].sprite = characterSelectionSprites[selection + characterSelectionSprites.Length / 2];
				}
				player2SelectedCharacter = selection;
				priority = selection == player1SelectedCharacter ? 1 : 0;
			}
			else {
				AudioManager.instance.PlaySound(cancelSfx);
				if (player2SelectedCharacter < 0) {
					SetupMainMenuMode();
				}
				else {
					player2SelectedItems[player2CurrentlyHighlighted].gameObject.SetActive(false);
					player1CharacterSprite[player2CurrentlyHighlighted].sprite = characterSelectionSprites[player2CurrentlyHighlighted];
					player2SelectedCharacter = -1;
				}
			}
		}
		return selection;
	}
}
