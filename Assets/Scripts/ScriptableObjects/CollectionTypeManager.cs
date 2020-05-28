using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "CollectionTypeManager", menuName = "ScriptableObjects/Settings/CollectionTypeManager")]
public class CollectionTypeManager : SerializedScriptableObject
{
	private static CollectionTypeManager instance;

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine,
		IsReadOnly = false, KeyLabel = "Type", ValueLabel = "Preset")]
	public Dictionary<CollectionType, List<EffectStructure>> collectionTypePresets;

	[Button]
	public static void GenerateCollectionTypes()
	{
		GetInstance().collectionTypePresets = new Dictionary<CollectionType, List<EffectStructure>>();
		foreach (CollectionType collectionType in Enum.GetValues(typeof(CollectionType))) {
			instance.collectionTypePresets.Add(collectionType, new List<EffectStructure>());
		}
	}

	public static CollectionTypeManager GetInstance()
	{
		if (!instance)
		{
			CollectionTypeManager[] all = Resources.FindObjectsOfTypeAll<CollectionTypeManager>();
			instance = (all.Length > 0) ? all[0] : null;
		}

#if UNITY_EDITOR

		if (!instance)
		{
			string[] configsGUIDs = AssetDatabase.FindAssets("t:" + typeof(CollectionTypeManager).Name);
			if (configsGUIDs.Length > 0)
			{
				instance = Resources.Load<CollectionTypeManager>(AssetDatabase.GUIDToAssetPath(configsGUIDs[0]));
			}
		}

#endif

		if (!instance)
		{
			instance = CreateInstance<CollectionTypeManager>();
			instance.name = "CollectionTypeManager";
		}
		return instance;
	}
}