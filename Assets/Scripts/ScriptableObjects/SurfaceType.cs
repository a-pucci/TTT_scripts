using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceType", menuName = "ScriptableObjects/SurfaceType")]
public class SurfaceType : ScriptableObject {
	#region Fields

	public EffectCollection effectCollection;
	public float dampening;
	public bool consideredForBounces;

	#endregion
}
