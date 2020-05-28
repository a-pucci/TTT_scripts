using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "newDashType", menuName = "ScriptableObjects/Types/Dash Type")]
public class MovementType : ScriptableObject {

	public float speed;
	
	#region Dash Settings

	[Header ("Dash Settings")]
	[Tooltip ("Time it takes for the dash to execute")]
	public float dashDuration;
	[Tooltip ("Distance of the dash")]
	public float dashDistance;
	[Tooltip("The dash curve. Check easings.net for examples")]
	public Ease dashEase;
	[Tooltip("The dash sound effect")] 
	public EffectCollection effectCollection;

	#endregion
}