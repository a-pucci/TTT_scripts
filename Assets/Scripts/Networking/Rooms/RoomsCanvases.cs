using UnityEngine;

public class RoomsCanvases : MonoBehaviour {

	[SerializeField] private CreateOrJoinRoomCanvas createOrJoinRoomCanvas;
	
	public CreateOrJoinRoomCanvas CreateOrJoinRoomCanvas => createOrJoinRoomCanvas;

	[SerializeField] private CurrentRoomCanvas currentRoomCanvas;

	public CurrentRoomCanvas CurrentRoomCanvas => currentRoomCanvas;

	private void Awake() {
		FirstInitialize();
	}

	private void FirstInitialize() {
		createOrJoinRoomCanvas.FirstInitialize(this);
		createOrJoinRoomCanvas.Show();
		currentRoomCanvas.FirstInitialize(this);
		currentRoomCanvas.Hide();
	}
}