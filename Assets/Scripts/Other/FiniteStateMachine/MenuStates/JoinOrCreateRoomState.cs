using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinOrCreateRoomState : FSMState<MainMenu> {

	public static JoinOrCreateRoomState Instance { get; } = new JoinOrCreateRoomState();
	private RoomsCanvases roomsCanvases;

	public override void Enter (MainMenu m) {
		NetworkManager.Instance.roomType = RoomType.Custom;
		roomsCanvases = m.roomCanvas.GetComponent<RoomsCanvases>();
		m.menuGameObject.SetActive(true);
		m.roomCanvas.SetActive(true);
		roomsCanvases.CurrentRoomCanvas.Hide();
		roomsCanvases.CreateOrJoinRoomCanvas.Show();
		m.SetEventSystemSelectedObject(m.joinRoomButton);
		NetworkManager.Instance.JoinLobby();
	}

	public override void Execute (MainMenu m) {
		m.HandleInput();
	}

	public override void Exit(MainMenu m) {
		m.menuGameObject.SetActive(false);
		roomsCanvases.CreateOrJoinRoomCanvas.Hide();
		m.roomCanvas.SetActive(false);
	}

}