using UnityEngine;

[CreateAssetMenu(fileName = "DashIncrement", menuName = "ScriptableObjects/Status/Dash Increment")]
public class DashIncrement : Status {
	public float incrementPercentage;
	public override void OnApply(IModdable moddable) {
		moddable.SetDash(incrementPercentage);
	}

	public override void OnRemove(IModdable moddable) {
		moddable.ResetDash();
	}
}