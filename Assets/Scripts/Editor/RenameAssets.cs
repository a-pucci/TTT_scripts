using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine.Experimental.UIElements.StyleEnums;
using Object = UnityEngine.Object;

public class RenameAssetsWindow : OdinEditorWindow
{
	#region Assets Batch
	
	[Space(10)]
	[FoldoutGroup("Assets Batch"), PropertyOrder(-10)]
	public string assetsName;
	[FoldoutGroup("Assets Batch"), PropertyOrder(-9)]
	public int startingIndex;
	[FoldoutGroup("Assets Batch"), PropertyOrder(-9)]
	public bool autoSort;
	[Space(10)]
	[FoldoutGroup("Assets Batch"), OnValueChanged("Sort")]public List<Object> assetsBatch;

	[MenuItem("Tools/Rename Assets")]
	private static void OpenWindow()
	{
		var window = GetWindow<RenameAssetsWindow>();
		window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
	}

	// [PropertyOrder(-8)]
	// [FoldoutGroup("Assets Batch")]
	// [Button(ButtonSizes.Medium)]
	public void Sort()
	{
		if (autoSort) {
			List<Object> newList = assetsBatch.OrderBy(o => o.name).ToList();
			assetsBatch = newList;
		}
	}
	
	[PropertyOrder(-7)]
	[FoldoutGroup("Assets Batch")]
	[Button("Rename", ButtonSizes.Medium)]
	public void RenameAssetsBatch()
	{
		int listSize = assetsBatch.Count;
		for (int i = 0; i < listSize; i++)
		{
			string path = AssetDatabase.GetAssetPath(assetsBatch[i]);

			string newName = assetsName + "_" + (i+startingIndex);
			AssetDatabase.RenameAsset(path, newName);
			float progress = (float)i /(listSize - 1);
			EditorUtility.DisplayProgressBar("Assets Rename", "Renaming Asset " + i + " / " + listSize, progress);
		}
		EditorUtility.ClearProgressBar();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[PropertyOrder(-6)]
	[FoldoutGroup("Assets Batch")]
	[Button("Clear", ButtonSizes.Medium)]
	public void ClearAssetsBatch()
	{
		assetsBatch.Clear();
		//assetsName = "";
		startingIndex = 0;
	}
	
	#endregion

	#region Multiple Assets

	//[FoldoutGroup("Assets Types"), PropertySpace(10)] public Type type;
	
	[FoldoutGroup("Multiple Assets"), PropertySpace(20), PropertyOrder(1), OnValueChanged("UpdateList"), InfoBox("DRAG HERE ITEMS TO RENAME")]
	[ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowIndexLabels = false), LabelText(">>  HERE  <<")] public List<Object> assetsToRename;
	[Space(20)]
	[FoldoutGroup("Multiple Assets"), PropertyOrder(2)] public string prefix;
	[FoldoutGroup("Multiple Assets"), PropertyOrder(3)] public List<RenameStruct> assets;

	[Serializable]
	public class RenameStruct {
		[HorizontalGroup,HideLabel]
		public Object asset;
		[HorizontalGroup, LabelWidth(70)]
		public string newName;
	}

	// [FoldoutGroup("Assets Types")][Button(ButtonSizes.Medium)]
	// public void GetAll() {
	// 	assets.Clear();
	// 	List<Object> blas = GetAllInstances(type).ToList();
	// 	foreach (Object bla in blas) {
	// 		assets.Add(new RenameStruct{asset = bla});
	// 	}
	// }

	[FoldoutGroup("Multiple Assets")][Button(ButtonSizes.Large)]
	public void RenameAssets() {
		int listSize = assets.Count;
		for (int i = 0; i < listSize; i++)
		{
			string path = AssetDatabase.GetAssetPath(assets[i].asset);
			if (path != "") {
				AssetDatabase.RenameAsset(path, prefix + assets[i].newName);
			}

			float progress = (float)i /(listSize - 1);
			EditorUtility.DisplayProgressBar("Assets Rename", "Renaming Asset " + i + " / " + listSize, progress);
		}
		EditorUtility.ClearProgressBar();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
	}

	[FoldoutGroup("Multiple Assets")][Button(ButtonSizes.Large)]
	public void ClearLists() {
		assets.Clear();
		assetsToRename.Clear();
	}
	
	private static Object[] GetAllInstances(Type type) {
		string[] guids = AssetDatabase.FindAssets("t:" + type.Name);
		var a = new Object[guids.Length];
		for (int i = 0; i < guids.Length; i++) {
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			a[i] = AssetDatabase.LoadAssetAtPath<Object>(path);
		}
		return a;
	}

	private void UpdateList() {
		assets.Clear();
		foreach (Object bla in assetsToRename) {
			assets.Add(new RenameStruct{asset = bla});
		}
		
		List<RenameStruct> newList = assets.OrderBy(o => o.asset.name).ToList();
		assets = newList;
	}
	
	#endregion
	
}