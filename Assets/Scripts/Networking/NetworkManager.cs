using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RoomType {
	None,
	Random,
	Custom
}

public class NetworkManager : MonoBehaviourPunCallbacks {
	public static NetworkManager Instance;
	public MainMenu menu;
	public TypedLobby customLobby = new TypedLobby("Custom", LobbyType.SqlLobby);
	public RoomType roomType = RoomType.None;

	[SerializeField] private NetworkSettings settings;

	public string Nickname {
		get { return settings.Nickname; }
		set {
			settings.Nickname = value;
			PhotonNetwork.NickName = value;
		}
	}
	
	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
		}
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	public void Connect() {
		
		print("Connecting to server.");
	
		PhotonNetwork.AutomaticallySyncScene = true;

		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.SendRate = settings.sendRate;
			PhotonNetwork.SerializationRate = settings.serializationRate;
			
			PhotonNetwork.NickName = settings.Nickname;
			PhotonNetwork.GameVersion = settings.GameVersion;
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public void Disconnect() {
		if (PhotonNetwork.IsConnected) {
			PhotonNetwork.Disconnect();
		}
	}

	public void JoinRandomRoom() {
		PhotonNetwork.JoinRandomRoom();
	}

	public void LeaveRoom() {
		if (PhotonNetwork.InRoom) {
			PhotonNetwork.LeaveRoom();
		}

		if (PhotonNetwork.InLobby) {
			PhotonNetwork.LeaveLobby();
		}
	}

	public void JoinLobby() {
		if (!PhotonNetwork.InLobby) {
			PhotonNetwork.JoinLobby(customLobby);
		}
	}
	
	public static char GetRoomType(RoomInfo room) {
		return room.Name[0];
	}

	public void CreateRoom(RoomType type, string roomName = null) {
		switch (type) {
			case RoomType.Custom:
				roomName = "%" + roomName; // PhotonNetwork.NickName + "'s Room";
				var options = new RoomOptions { MaxPlayers = 2, PublishUserId = true, PlayerTtl = 0};
				PhotonNetwork.CreateRoom(roomName, options, customLobby);
				break;
			
			case RoomType.Random:
				var roomOptions = new RoomOptions {MaxPlayers = 2, PlayerTtl = 0};
				PhotonNetwork.CreateRoom(null, roomOptions);
				break;
			
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}
	
	public void ReturnToMenu() {
		StopMessageQueue();
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
		string currentScene = SceneManager.GetActiveScene().name;
		if (currentScene == "scn_menu") menu = null;
		SceneManager.LoadScene("scn_menu", LoadSceneMode.Additive);
		StartCoroutine(GoToLobby(currentScene));
	}

	private IEnumerator GoToLobby(string currentScene) {
		yield return new WaitUntil(() => menu != null);
		yield return null;
		menu.GoToLobby();
		SceneManager.UnloadSceneAsync(currentScene);
	}

	public void StopMessageQueue() {
		PhotonNetwork.IsMessageQueueRunning = false;
	}
	
	#region Photon CallBacks
	
	public override void OnConnectedToMaster() {
		print("Connected to master."); 
		
		menu.canUpdateState = true;
		if(menu.GetCurrentState() != LobbyState.Instance && menu.GetCurrentState() != JoinOrCreateRoomState.Instance) menu.ChangeState(LobbyState.Instance);
		if(roomType == RoomType.Custom) JoinLobby(); 
	}

	public override void OnDisconnected(DisconnectCause cause) {
		print("Disconnected from server for reason " + cause);
	}

	public override void OnJoinedLobby() {
		menu.canUpdateState = true;
		print("Joined lobby, rooms: " + PhotonNetwork.CountOfRooms);
	}
	
	public override void OnLeftLobby() {
		print("Lobby left successfully");
	}

	public override void OnJoinedRoom() {
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
	}

	public override void OnLeftRoom() {
		menu.canUpdateState = false;
		print("Room left successfully.");
	}
	
	public override void OnRoomListUpdate(List<RoomInfo> roomList) {
		foreach (RoomInfo roomInfo in roomList) {
			print("room in lobby: " + roomInfo.Name);
		}
	}

	public override void OnPlayerEnteredRoom(Player other)
	{
		Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

		if (menu.GetCurrentState() == MatchingState.Instance) {
			GoToCharacterSelection();
		}
	}

	public void GoToCharacterSelection() {
		if (PhotonNetwork.IsMasterClient) {
			PhotonNetwork.CurrentRoom.IsOpen = false;
			PhotonNetwork.CurrentRoom.IsVisible = false;
			menu.photonView.RPC("GoToCharacterSelection", RpcTarget.All);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer) {
		if(menu == null || menu.GetCurrentState() != JoinOrCreateRoomState.Instance) ReturnToMenu();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
		
		CreateRoom(RoomType.Random);
	}

	#endregion

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		PhotonNetwork.IsMessageQueueRunning = true;
	}
}