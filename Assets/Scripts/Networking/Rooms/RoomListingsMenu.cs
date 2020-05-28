using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks {

	[SerializeField] private RoomListing roomListing;
	[SerializeField] private Transform content;

	private List<RoomListing> listings = new List<RoomListing>();
	private RoomsCanvases roomsCanvases;
	
	public void FirstInitialize(RoomsCanvases canvases) {
		roomsCanvases = canvases;
	}

	public override void OnJoinedRoom() {
		roomsCanvases.CurrentRoomCanvas.Show();
		content.DestroyChildren();
		listings.Clear();
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList) {
		foreach (RoomInfo roomInfo in roomList) {
			int index = listings.FindIndex(x => x.RoomInfo.Name == roomInfo.Name);
			
			if (roomInfo.RemovedFromList) {
				if (index != -1) {
					Destroy(listings[index].gameObject);
					listings.RemoveAt(index);
				}
			}
			else {
				if (index == -1 && NetworkManager.GetRoomType(roomInfo) == '%') {
					RoomListing listing = Instantiate(roomListing, content);
					if (listing != null) {
						listing.SetRoomInfo(roomInfo);
						listings.Add(listing);
					}
				}
			}
		}
	}
}