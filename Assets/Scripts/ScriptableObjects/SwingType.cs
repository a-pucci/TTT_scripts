using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "newSwingType", menuName = "ScriptableObjects/Types/Swing Type")]
public class SwingType : ScriptableObject {

	#region Swing Settings

	[FormerlySerializedAs("lightSwing")]
	[Title ("Swing Settings")]
	[Tooltip ("Regular Swing Strength")]
	public float regularSwing;
	
	[Range(0f, 100f), OnValueChanged("UpdateSwings")]
	[Tooltip ("Light Swing percentage of regular swing in a range of (0,1)")]
	public float lightSwingPercentage;
	[ReadOnly] public float lightSwing;
	
	[Range(100f, 1000f), OnValueChanged("UpdateSwings")]
	[Tooltip ("Strong Swing percentage of regular swing in a range of (1,10)")]
	public float strongSwingPercentage;
	[ReadOnly] public float strongSwing;

	[Title ("Swing Charge Settings")]
	[Tooltip ("This is the value the speed setting is divided by while the player is charging the strike")]
	public float chargingDivider;
	[Tooltip ("This is the value the animator speed setting is divided by while the player is charging the strike")]
	public float animatorChargingDivider;
	
	// [Tooltip ("This value will be added to the swing counter each one hundredth of a second")]
	// public float swingChargeSpeed;
	// [Tooltip ("After the charge value reaches this parameter, the swing will be strong")]
	// public float swingLimitStrong;
	// [Tooltip ("After the charge value reaches this parameter, the swing will stop")]
	// public float swingLimitMax;
	
	[Tooltip ("This is the time of Light part of Charge")]
	public float lightSwingChargeTime;
	[Tooltip ("This is the time of Strong part of Charge")]
	public float strongSwingChargeTime;
	
	[Tooltip ("Light Swing Charge Color")]
	public Color lightSwingColor;
	[Tooltip ("Strong Swing Charge Color")]
	public Color strongSwingColor;

	public float swingY;
	public float highSwingYMultiplier;
	public float lowSwingYMultiplier;
	public float swingX = 1f;

	public EffectCollection swingEffects;

	#endregion

	private void UpdateSwings() {
		lightSwing = regularSwing * lightSwingPercentage /100;
		strongSwing = regularSwing * strongSwingPercentage /100;
	}
}