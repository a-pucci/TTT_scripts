using UnityEngine;

[CreateAssetMenu(fileName = "newHitSettings", menuName = "ScriptableObjects/Hit Settings")]
public class HitSettings : ScriptableObject {

	#region Fields

	// Public
	[Header("Stun Settings")]
	public int fadeIntervals;
	[Tooltip("Light Stun Time in Seconds")]
	[Range(0.01f , 10f)]
	public float lightStunTime;
	[Tooltip("Strong Stun Time in Seconds")]
	[Range(0.01f , 10f)]
	public float strongStunTime;
	[Tooltip("Fade Percentage")]
	[Range(0f, 100f)]
	public float fadePercentage;

	[Header("Shake Settings")]
	[Tooltip ("Shake Enabled")]
	public bool shakeOnHit;
	[Tooltip ("Shake Duration")]
	public float shakeDuration;
	[Tooltip("Shake Strength")]
	public float shakeStrength;
	[Tooltip("Shake Vibrato")]
	public int shakeVibrato;

	#endregion
}