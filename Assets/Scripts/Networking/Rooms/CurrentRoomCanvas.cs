using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviourPunCallbacks {

	[SerializeField] private PlayerListingsMenu playerListingsMenu;

	public GameObject startButton;
	public GameObject kickButton;
	private RoomsCanvases roomsCanvases;
	
	public void FirstInitialize(RoomsCanvases canvases) {
		roomsCanvases = canvases;
		playerListingsMenu.FirstInitialize(canvases);
	}
	
	public void Show() {
		gameObject.SetActive(true);
		if (PhotonNetwork.IsMasterClient) {
			startButton.SetActive(true);
			kickButton.SetActive(true);
		}
	}
	
	[PunRPC]
	public void LeaveRoom() {
		NetworkManager.Instance.LeaveRoom();
		roomsCanvases.CreateOrJoinRoomCanvas.Show();
		NetworkManager.Instance.menu.SetEventSystemSelectedObject(NetworkManager.Instance.menu.joinRoomButton);
		Hide();
	}

	public void Hide() {
		startButton.SetActive(false);
		kickButton.SetActive(false);
		gameObject.SetActive(false);
	}

	public void KickPlayer() {
		photonView.RPC(nameof(LeaveRoom), RpcTarget.Others);
	}
 
	public void StartGame() {
		if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
		NetworkManager.Instance.GoToCharacterSelection();
	}

	public override void OnPlayerLeftRoom(Player otherPlayer) {
		Show();
	}
}