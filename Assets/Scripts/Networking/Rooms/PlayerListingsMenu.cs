using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks {

	[SerializeField] private PlayerListing playerListing;
	[SerializeField] private Transform content;

	private bool ready;

	private List<PlayerListing> listings = new List<PlayerListing>();
	
	private RoomsCanvases roomsCanvases;
	
	public void FirstInitialize(RoomsCanvases canvases) {
		roomsCanvases = canvases;
	}

	public override void OnEnable() {
		base.OnEnable();
		GetCurrentRoomPlayers();
	}

	public override void OnDisable() {
		base.OnDisable();
		foreach (PlayerListing listing in listings) {
			Destroy(listing.gameObject);
		}
		listings.Clear();
	}

	private void GetCurrentRoomPlayers() {
		if (!PhotonNetwork.IsConnected) {
			return;
		}
		if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null) {
			return;
		}
		
		foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players) {
			AddPlayerListing(playerInfo.Value);
		}
	}

	private void AddPlayerListing(Player player) {
		int index = listings.FindIndex(x => x.Player == player);
		if (index != -1) {
			listings[index].SetPlayerInfo(player);
		}
		else {
			PlayerListing listing = Instantiate(playerListing, content);
		
			if (listing != null) {
				listing.SetPlayerInfo(player);
				listings.Add(listing);
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer) {
		AddPlayerListing(newPlayer);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer) {
		int index = listings.FindIndex(x => x.Player == otherPlayer);
		if (index != -1) {
			Destroy(listings[index].gameObject);
			listings.RemoveAt(index);
		}
	}
}