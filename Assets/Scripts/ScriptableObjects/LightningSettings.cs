using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewLightningSettings", menuName = "ScriptableObjects/Settings/Lightning Settings")]
public class LightningSettings : ScriptableObject {

	[MinValue(1)]
	public int lines;
	[MinValue(2)]
	public int lineVertex;
	[MinValue(0.01)]
	public float lineWidth;
	[ShowIf("isLightning")]
	public float lineOffset;
	public float duration;
	public float changeTime;
	public Material material;
	public AudioClip lightningSfx;

	private bool isLightning;

	private void UpdateBool() {
		if (lineVertex > 2) {
			isLightning = true;
		}
		else {
			isLightning = false;
		}
	}

	private void OnEnable() {
		UpdateBool();
	}
}