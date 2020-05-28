using UnityEngine;

[CreateAssetMenu(fileName = "MovementReduction", menuName = "ScriptableObjects/Status/Movement Reduction")]
public class MovementReduction : Status {
	public float reductionPercentage;
		
	public override void OnApply(IModdable moddable) {
		moddable.SetSpeed(reductionPercentage);
	}

	public override void OnRemove(IModdable moddable) {
		moddable.ResetSpeed();
	}
}