using System;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "newPlayerType", menuName = "ScriptableObjects/Types/Player Type")]
public class PlayerType : ScriptableObject {
	[Title("Character Properties")]
	public MovementType movementType;
	public SwingType swingType;
	public string characterName;
	[LabelText("Pc Arena Multipliers")] public ArenaMultipliers pcMultipliers;
	[LabelText("City Arena Multipliers")] public ArenaMultipliers cityMultipliers;

	public Skin[] skins;
	public byte stat1;
	public byte stat2;
	public byte stat3;
}
[Serializable, InlineProperty(LabelWidth = 80)]
public class ArenaMultipliers {
	[HorizontalGroup] public float speed = 1;
	[HorizontalGroup] public float strength = 1;
}