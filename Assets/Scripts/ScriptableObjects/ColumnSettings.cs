using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newColumnSettings", menuName = "ScriptableObjects/Settings/ColumnSettings")]
public class ColumnSettings : ScriptableObject {

	public float waitBeforeActivation;
	public float timeToAppear;
	public float timeToDisappear;
	public float timeActivated;

	public Material triggerMaterial;
	public Material triggerActivatedMaterial;

	public EffectCollection effects;
}