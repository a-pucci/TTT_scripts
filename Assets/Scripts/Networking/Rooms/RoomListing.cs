using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListing : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;
	
	public RoomInfo RoomInfo { get; private set; }

	public void SetRoomInfo(RoomInfo roomInfo) {
		RoomInfo = roomInfo;
		text.text = roomInfo.Name.Substring(1);
	}

	public void OnClick_Button() {
		PhotonNetwork.JoinRoom(RoomInfo.Name);
	}

}