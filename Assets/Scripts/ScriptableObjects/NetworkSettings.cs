using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkSettings", menuName = "ScriptableObjects/Network Settings")]
public class NetworkSettings : ScriptableObject {
	[SerializeField] private string gameVersion = "1";
	public string GameVersion => gameVersion;

	[SerializeField] private string nickname;
	public string Nickname { 
		get {
			return nickname;
		}
		set {
			nickname = value;
		}
	}

	[InfoBox("Default: 20")] public int sendRate;
	[InfoBox("Default: 10")] public int serializationRate;
}