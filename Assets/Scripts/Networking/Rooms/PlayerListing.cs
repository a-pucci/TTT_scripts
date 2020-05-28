using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;

	public Player Player { get; private set; }

	public bool ready;

	public void SetPlayerInfo(Player player) {
		Player = player;
		text.text = player.NickName;
	}

}