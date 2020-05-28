using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Arena {
	scn_case95,
	scn_magicCity,
	scn_temple
}

public class ArenaLoader : MonoBehaviour {

	private PlayerType topPlayer;
	private int topSkin;
	private PlayerType bottomPlayer;
	private int bottomSkin;
	
	public static ArenaLoader Instance { get; private set; }

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(this);
			throw new Exception("An instance of this singleton already exists.");
		}
		else {
			Instance = this;
		}
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
		if (arg1 == LoadSceneMode.Additive) return;
		LoadArena();
	}

	public void SetPlayers(PlayerType top, int topSkinIndex, PlayerType bottom, int bottomSkinIndex) {
		topPlayer = top;
		topSkin = topSkinIndex;
		bottomPlayer = bottom;
		bottomSkin = bottomSkinIndex;
	}

	public void LoadArena() {
		var references = FindObjectOfType<ArenaReferences>();
		if (references != null) {
			Arena arena = references.arena;

			LoadPlayer(references.topReferences, topPlayer, topSkin, PlayerPosition.Top, arena);
			LoadPlayer(references.bottomReferences, bottomPlayer, bottomSkin, PlayerPosition.Bottom, arena);

			switch (arena) {
				case Arena.scn_case95:
					LoadPcArena(references.topReferences, topPlayer, topSkin, PlayerPosition.Top);
					LoadPcArena(references.bottomReferences, bottomPlayer, bottomSkin, PlayerPosition.Bottom);
					break;

				case Arena.scn_magicCity:
					LoadCityArena(references.topReferences, topPlayer, PlayerPosition.Top);
					LoadCityArena(references.bottomReferences, bottomPlayer, PlayerPosition.Bottom);
					break;
				
				case Arena.scn_temple:
					LoadGreekArena(references.topReferences, topPlayer, PlayerPosition.Top);
					LoadGreekArena(references.bottomReferences, bottomPlayer, PlayerPosition.Bottom);
					break;
			}
		}	
	}

	private void LoadPcArena(PlayerReferences playerRef, PlayerType playerType, int skinIndex, PlayerPosition position) {
		if (playerRef.wallController != null) {
			playerRef.wallController.runtimeAnimatorController = playerType.skins[skinIndex].wallController;
		}
		if (playerRef.iconBackgroundPcArena != null) {
			playerRef.iconBackgroundPcArena.sprite = playerType.skins[skinIndex].iconBackgroundPcArena;
		}
		foreach (SpriteRenderer point in playerRef.netPoints) {
			point.sprite = playerType.skins[skinIndex].wallScoreSprite;
		}
		
		if (position == PlayerPosition.Top) {
			playerRef.scorePcArena.topPointsExplosion = playerType.skins[skinIndex].pointExplosionVfx;
		}
		else {
			playerRef.scorePcArena.bottomPointsExplosion = playerType.skins[skinIndex].pointExplosionVfx;
		}
	}

	private void LoadCityArena(PlayerReferences playerRef, PlayerType playerType, PlayerPosition player) {
		if (player == PlayerPosition.Top) {
			playerRef.iconName.sprite = playerType.skins[topSkin].iconName;
			playerRef.backWalls.topWalls.activeMaterial = playerType.skins[topSkin].activeWallMaterial;
			playerRef.backWalls.topWalls.inactiveMaterial = playerType.skins[topSkin].inactiveWallMaterial;
			playerRef.scoreArena.topPoints.activeMaterial = playerType.skins[topSkin].lampMaterial;
			foreach (MeshRenderer renderer in playerRef.scoreArena.topPoints.points) {
				renderer.material = playerType.skins[topSkin].lampMaterial;
			}
		}
		else {
			playerRef.iconName.sprite = playerType.skins[bottomSkin].iconName;
			playerRef.backWalls.bottomWalls.activeMaterial = playerType.skins[bottomSkin].activeWallMaterial;
			playerRef.backWalls.bottomWalls.inactiveMaterial = playerType.skins[bottomSkin].inactiveWallMaterial;
			playerRef.scoreArena.bottomPoints.activeMaterial = playerType.skins[bottomSkin].lampMaterial;
			foreach (MeshRenderer renderer in playerRef.scoreArena.bottomPoints.points) {
				renderer.material = playerType.skins[bottomSkin].lampMaterial;
			}
		}
	}
	
	private void LoadGreekArena(PlayerReferences playerRef, PlayerType playerType, PlayerPosition player) {
		if (player == PlayerPosition.Top) {
			playerRef.scoreGreekArena.topPoints.activeMaterial = playerType.skins[topSkin].netMaterialOn;
			playerRef.scoreGreekArena.topPoints.inactiveMaterial = playerType.skins[topSkin].netMaterialOff;
			foreach (MeshRenderer renderer in playerRef.scoreGreekArena.topPoints.points) {
				renderer.material = playerType.skins[topSkin].netMaterialOn;
			}
			playerRef.scoreGreekArena.topColor = playerType.skins[topSkin].auraColor;
		}
		else {
			playerRef.scoreGreekArena.bottomPoints.activeMaterial = playerType.skins[bottomSkin].netMaterialOn;
			playerRef.scoreGreekArena.bottomPoints.inactiveMaterial = playerType.skins[bottomSkin].netMaterialOff;
			foreach (MeshRenderer renderer in playerRef.scoreGreekArena.bottomPoints.points) {
				renderer.material = playerType.skins[bottomSkin].netMaterialOn;
			}
			playerRef.scoreGreekArena.bottomColor = playerType.skins[bottomSkin].auraColor;
		}
	}

	private void LoadPlayer(PlayerReferences playerRef, PlayerType playerType, int skinIndex, PlayerPosition position, Arena arena) {
		playerRef.player.movementType = playerType.movementType;
		playerRef.player.swingType = playerType.swingType;
		playerRef.player.auraColor = playerType.skins[skinIndex].auraColor;
		playerRef.animator.runtimeAnimatorController = position == PlayerPosition.Top ? playerType.skins[skinIndex].animatorTop : playerType.skins[skinIndex].animatorBottom;
		playerRef.player.auraAnimator.runtimeAnimatorController = playerType.skins[skinIndex].auraController;
		playerRef.icon.sprite = playerType.skins[skinIndex].icon;
		playerRef.characterName.text = playerType.characterName;
		playerRef.characterName.colorGradient = GetColorGradient(playerType.skins[skinIndex].scoreColors);
		playerRef.characterIntroName.text = playerType.characterName;
		playerRef.characterIntroName.colorGradient = GetColorGradient(playerType.skins[skinIndex].scoreColors);
		playerRef.characterIntroImage.sprite = playerType.skins[skinIndex].characterIntroSprite;
		playerRef.characterScoreNumber.colorGradient = GetColorGradient(playerType.skins[skinIndex].scoreColors);
		playerRef.player.arenaMultipliers = arena == Arena.scn_case95 ? playerType.pcMultipliers : playerType.cityMultipliers;
	}

	private VertexGradient GetColorGradient(Color[] colors) {
		VertexGradient gradient;
		gradient.topLeft = colors[0];
		gradient.topRight = colors[0];
		gradient.bottomLeft = colors[1];
		gradient.bottomRight = colors[1];
		return gradient;
	}
}
