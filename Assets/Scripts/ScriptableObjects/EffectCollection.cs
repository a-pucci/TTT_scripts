using System;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEffectCollection", menuName = "ScriptableObjects/Effects/Effect Collection")]
public class EffectCollection : ScriptableObject {

	#region Fields
	// Public
	[OnValueChanged("UpdateEffectList")]
	public CollectionType type;
	
	[ShowInInspector]
	public List<EffectStructure> effectStructures = new List<EffectStructure>();

	#endregion

	#region Methods

	public VisualEffect Get(string effectName) {
		for (int i = 0; i < effectStructures.Count; i++) {
			if (effectStructures[i].effectName == effectName) {
				return effectStructures[i].effect;
			}
		}
		return null;
	}

	private void UpdateEffectList()
	{
		effectStructures.Clear();
		effectStructures.AddRange(CollectionTypeManager.GetInstance().collectionTypePresets[type]);
	}

	#endregion

}

public enum CollectionType {
	Surface,
	Ball,
	Exploder,
	Dash,
	Other
}

[Serializable]
[InlineProperty(LabelWidth = 70)]
public struct EffectStructure {
	[LabelText("Name")]
	[HorizontalGroup]
	public string effectName;
	[HorizontalGroup]
	public VisualEffect effect;

}