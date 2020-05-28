﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour {

	[SerializeField] private CreateRoomMenu createRoomMenu;
	[SerializeField] private RoomListingsMenu roomListingsMenu;
	
	private RoomsCanvases roomsCanvases;
	
	public void FirstInitialize(RoomsCanvases canvases) {
		roomsCanvases = canvases;
		createRoomMenu.FirstInitialize(canvases);
		roomListingsMenu.FirstInitialize(canvases);
	}
	
	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		createRoomMenu.Hide();
		gameObject.SetActive(false);
	}
}