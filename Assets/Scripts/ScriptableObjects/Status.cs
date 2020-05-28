using UnityEngine;

public abstract class Status : ScriptableObject {
	public abstract void OnApply(IModdable moddable);
	public abstract void OnRemove(IModdable moddable);
}