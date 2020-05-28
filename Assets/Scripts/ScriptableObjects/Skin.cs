using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "newSkin", menuName = "ScriptableObjects/Skin")]
public class Skin : ScriptableObject {

	[Title("Character Aesthetic")] 
	public Sprite icon;
	[InfoBox("First color = Top color\nSecond color = Bottom color")]
	public Color[] scoreColors;
	public Sprite characterSelectionIconSprite;
	public Sprite characterPreviewSprite;
	public Sprite characterIntroSprite;
	public RuntimeAnimatorController animatorTop;
	public RuntimeAnimatorController animatorBottom;
	public RuntimeAnimatorController auraController;
	public Color auraColor;

	[Title("PcArena")]
	public RuntimeAnimatorController wallController;
	public Sprite wallScoreSprite;
	public VisualEffect pointExplosionVfx;
	public Sprite iconBackgroundPcArena;

	[Title("CityArena")] 
	public Material activeWallMaterial;
	public Material inactiveWallMaterial;
	public Material lampMaterial;
	public Sprite iconName;
	
	[Title("TempleArena")] 
	public Material netMaterialOn;
	public Material netMaterialOff;
}