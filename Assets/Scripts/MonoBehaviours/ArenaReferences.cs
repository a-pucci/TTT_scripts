using System;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Image = UnityEngine.UI.Image;

public class ArenaReferences : MonoBehaviour {
	[OnValueChanged("UpdateValues")]public Arena arena;
	
	[BoxGroup("Top Player References")]
	public PlayerReferences topReferences;
	
	[BoxGroup("Bottom Player References")]
	public PlayerReferences bottomReferences;

	private void UpdateValues() {
		topReferences.arena = arena;
		bottomReferences.arena = arena;
	}
}

[Serializable]
public class PlayerReferences {
	[HideInInspector] public Arena arena;
	[BoxGroup("Player")] public Animator animator;
	[BoxGroup("Player")] public CharacterManager player;
	[BoxGroup("UI")] public Image icon;
	[BoxGroup("UI")] public TextMeshProUGUI characterName;
	[BoxGroup("UI")] public TextMeshProUGUI characterScoreNumber;
	[BoxGroup("UI")] public TextMeshProUGUI characterIntroName;
	[BoxGroup("UI")] public Image characterIntroImage;
	[ShowIf("arena", Arena.scn_case95), BoxGroup("Arena")] public Animator wallController;
	[ShowIf("arena", Arena.scn_case95), BoxGroup("Arena")] public SpriteRenderer[] netPoints;
	[ShowIf("arena", Arena.scn_case95), BoxGroup("Arena")] public ScorePcArena scorePcArena;
	[ShowIf("arena", Arena.scn_case95), BoxGroup("Arena")] public Image iconBackgroundPcArena;
	[ShowIf("arena", Arena.scn_magicCity), BoxGroup("Arena")] public BackWallsManager backWalls;
	[ShowIf("arena", Arena.scn_magicCity), BoxGroup("Arena")] public ScoreCityArena scoreArena;
	[ShowIf("arena", Arena.scn_magicCity), BoxGroup("UI")] public Image iconName;
	[ShowIf("arena", Arena.scn_temple), BoxGroup("Arena")] public ScoreGreekArena scoreGreekArena;
	

}