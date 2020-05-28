using System.Collections.Generic;
using Kino;
using Photon.Pun;
using Photon.Realtime;
using Rewired.Utils.Platforms.Windows;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenu : MonoBehaviourPun {

	private FiniteStateMachine<MainMenu> stateMachine;

	public EventSystem eventSystem;
	public StandaloneInputModule inputModule;
	public ArenaLoader arenaLoader;
	public GameObject menuGameObject;
	public GameObject playerSelectBannerManager;
	public List<TextMeshProUGUI> playerSelectBanners;
	public GameObject leftStatBlockPlayerText;
	public GameObject rightStatBlockPlayerText;
	public List<GameObject> mainButtons;

	
	public Rewired.Player player1;
	public Rewired.Player player2;
	
	[FoldoutGroup("Animations")] public AnalogGlitch analogGlitch;
	[FoldoutGroup("Animations")] public AnalogGlitchTransition changeSelection;
	[FoldoutGroup("Animations")] public AnalogGlitchTransition confirmSelection;
	[FoldoutGroup("Animations")] public RectTransform buttonsImageRect;
	[FoldoutGroup("Animations")] public RectAnchorAnimation buttonsImageArrivingAnimation;
	
	
	[FoldoutGroup("Lobby")] public GameObject lobbyCanvas;
	[FoldoutGroup("Lobby")] public List<GameObject> matchTypeButtons;
	[FoldoutGroup("Lobby")] public TextMeshProUGUI waitingText;
	[FoldoutGroup("Lobby")] public GameObject roomCanvas;
	[FoldoutGroup("Lobby")] public GameObject joinRoomButton;
	[FoldoutGroup("Lobby")] public GameObject leaveRoomButton;
	[FoldoutGroup("Lobby")] public Transform roomListParent;
	
	[FoldoutGroup("Sfx")] public AudioClip selectionSfx;
	[FoldoutGroup("Sfx")] public AudioClip cancelSfx;
	[FoldoutGroup("Sfx")] public AudioClip changeSfx;

	[HideInInspector] public string gameSceneName;
	[HideInInspector] public int player1SelectedCharacter;
	[HideInInspector] public int player2SelectedCharacter;
	[HideInInspector] public int frameSkip;
	[HideInInspector] public int priority;
	[HideInInspector] public GameObject currentSelectedGameObject;
	[HideInInspector] public bool canUpdateState = true;
	[HideInInspector] public bool typing;

	public StageSelection stageSelection;
	public CharacterSelection characterSelection;
	public NicknameSetting nicknameSetting;

	private bool waitForAxisReset1;
	private bool waitForAxisReset2;

	private void Start() {
		stateMachine = new FiniteStateMachine<MainMenu>();
		stateMachine.Configure(this, MainState.Instance);
		ChangeState(GlobalState.Instance, true);
		if (NetworkManager.Instance.menu == null) NetworkManager.Instance.menu = this;
	}
	
	private void Update () {
		if (frameSkip <= 0) {
			if(canUpdateState) stateMachine.Update();
		}
		else {
		frameSkip--;
		}
	}
	public void ChangeState(FSMState<MainMenu> state, bool isGlobalState = false) {
		frameSkip = 2;
		if(isGlobalState) stateMachine.ChangeGlobalState(state);
		else stateMachine.ChangeState(state);
	}

	public FSMState<MainMenu> GetCurrentState() {
		return stateMachine.currentState;
	}

	#region RPCs
	
	[PunRPC]
	public void RevertState(int skips = 0) {
		frameSkip = 10;
		AudioManager.instance.PlaySound(cancelSfx);
		stateMachine.RevertToPreviousState(skips);
	}
	
	[PunRPC]
	public void GoToCharacterSelection() {
		if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
		ChangeState(CharacterSelectionState.Instance);
	}
	
	[PunRPC]
	public void GoToStageSelection() {
		ChangeState(StageSelectionState.Instance);
	}

	[PunRPC]
	public void GoToLobby() {
		ChangeState(GlobalState.Instance, true);
		ChangeState(GameSelectionState.Instance);
		ChangeState(ConnectingState.Instance);
	}
	
	#endregion
	
	#region Buttons
	
	public void GameSelectionMenu() {
		frameSkip = 2;
		AudioManager.instance.PlaySound(selectionSfx);
		ChangeState(GameSelectionState.Instance);	
	}
	
	public void PlayOffline() {
		NetworkManager.Instance.Disconnect();
		NetworkManager.Instance.roomType = RoomType.None;
		frameSkip = 2;
		AudioManager.instance.PlaySound(selectionSfx);
		ChangeState(GlobalState.Instance, true);
		ChangeState(CharacterSelectionState.Instance);
	}
	
	public void PlayOnline() {
		frameSkip = 2;
		AudioManager.instance.PlaySound(selectionSfx);
		ChangeState(ConnectingState.Instance);
	}

	public void JoinRandomRoom() {
		frameSkip = 4;
		AudioManager.instance.PlaySound(selectionSfx);
		ChangeState(MatchingState.Instance);

		NetworkManager.Instance.JoinRandomRoom();
	}

	public void CreateOrJoinRoom() {
		frameSkip = 4;
		AudioManager.instance.PlaySound(selectionSfx);
		ChangeState(JoinOrCreateRoomState.Instance);
	}

	public void JoinRoom() {
		if(roomListParent.childCount > 0) SetEventSystemSelectedObject(roomListParent.GetChild(0).gameObject);
	}


	public void Exit() {
		eventSystem.enabled = false;
		StartCoroutine(confirmSelection.Animate(analogGlitch));
		confirmSelection.AnimationFinished = Application.Quit;
	}
	
	#endregion

	#region Methods

	public void SetEventSystemSelectedObject(GameObject obj) {
		eventSystem.firstSelectedGameObject = obj;
		eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
		currentSelectedGameObject = eventSystem.firstSelectedGameObject;
		if(eventSystem.firstSelectedGameObject != null && eventSystem.firstSelectedGameObject.GetComponent<Animator>())
		eventSystem.firstSelectedGameObject.GetComponent<Animator>().SetTrigger("Highlighted");
	}

	private bool ReadAnyControllerInput(string number) {
		return player1.GetAnyButton() || player2.GetAnyButton();
	}

	public void HandleInput() {
		if (eventSystem.currentSelectedGameObject != currentSelectedGameObject && eventSystem.currentSelectedGameObject != null) {
			currentSelectedGameObject = eventSystem.currentSelectedGameObject;
			if(!typing)StartCoroutine(changeSelection.Animate(analogGlitch));
		}
		if (!eventSystem.currentSelectedGameObject && (ReadAnyControllerInput("1") || ReadAnyControllerInput("2"))) {
			eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
		}
		//if (ReadAnyControllerInput("1")) {
		//	inputModule.verticalAxis = "Vertical1";
		//	inputModule.horizontalAxis = "Horizontal1";
		//	inputModule.submitButton = "Swing1";
		//	inputModule.cancelButton = "Dash1";
		//}
		//else if (ReadAnyControllerInput("2")) {
		//	inputModule.verticalAxis = "Vertical2";
		//	inputModule.horizontalAxis = "Horizontal2";
		//	inputModule.submitButton = "Swing2";
		//	inputModule.cancelButton = "Dash2";
		//}

		if (player1.GetButtonDown(RewiredConsts.Action.Back) || player2.GetButtonDown(RewiredConsts.Action.Back)) {
			int skips = 0;
			
			if (stateMachine.currentState == LobbyState.Instance || stateMachine.currentState == MatchingState.Instance) skips = 1;
			if (PhotonNetwork.InRoom && stateMachine.currentState == JoinOrCreateRoomState.Instance) return;
			
			RevertState(skips);
		}
	}
	
	#endregion
}