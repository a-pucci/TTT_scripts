using UnityEngine;

[CreateAssetMenu(fileName = "SizeIncrement", menuName = "ScriptableObjects/Status/Size Increment")]
public class SizeIncrement : Status {
	public float incrementPercentage;
	
	public override void OnApply(IModdable moddable) {
		moddable.SetSize(incrementPercentage);
	}

	public override void OnRemove(IModdable moddable) {
		moddable.ResetSize();
	}
}