using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CreateRoomMenu : MonoBehaviourPunCallbacks {

	private RoomsCanvases roomsCanvases;
	private MainMenu menu;
	public CurrentRoomCanvas currentRoomCanvas;
	public TMP_InputField inputField;
	public GameObject keyboard;
	public GameObject firstKey;

	private string roomName;
	
	public void FirstInitialize(RoomsCanvases canvases) {
		roomsCanvases = canvases;
		menu = FindObjectOfType<MainMenu>();
	}

	public void SetRoomName() {
		menu.typing = true;
		inputField.transform.parent.gameObject.SetActive(true);
		keyboard.SetActive(true);
		menu.SetEventSystemSelectedObject(firstKey);
	}

	public void CreateRoom() {
		if (!PhotonNetwork.IsConnected) {
			return;
		}
		if (!string.IsNullOrEmpty(inputField.text)) {

			NetworkManager.Instance.CreateRoom(RoomType.Custom, inputField.text);

		}
	}

	public override void OnJoinedRoom() {
		print("Joined room successfully.");
		roomsCanvases.CurrentRoomCanvas.Show();
		roomsCanvases.CreateOrJoinRoomCanvas.Hide();
		menu.SetEventSystemSelectedObject(menu.leaveRoomButton);
 
		Hide();
	}

	public void Esc() {
		menu.SetEventSystemSelectedObject(menu.joinRoomButton);
		Hide();
	}

	public void Hide() {
		menu.typing = false;
		keyboard.SetActive(false);
		inputField.text = string.Empty;
		inputField.transform.parent.gameObject.SetActive(false);
	}
}