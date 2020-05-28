using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

// Disable "variable assigned but not used" warning
#pragma warning disable 0414

public class ComponentAdder : OdinEditorWindow {
 	
 	[Space(10)]
 	[InfoBox("Il componente da cercare")]
 	public Type componentToSearch;

	[InfoBox("Numero dei componenti da aggiungere o rimuovere")][OnValueChanged("UpdateList")]
	public int componentsToAdd;
 	[ShowIf("showList")]public List<Type> componentsToAddOrRemove = new List<Type>();
	private bool showList;

 	[MenuItem("Tools/Add Components")]
 	private static void OpenWindow(){
 		var window = GetWindow<ComponentAdder>();
 		window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
 	}
 	[PropertySpace(10)]
 	[ButtonGroup]
 	[Button(ButtonSizes.Medium)]
 	private void AddComponents() {
 		ComponentAddOrRemove(false);
 	}
 
 	[ButtonGroup]
 	[Button(ButtonSizes.Medium)]
 	private void RemoveComponents() {
 		ComponentAddOrRemove(true);
 	}
 
 	private void ComponentAddOrRemove(bool removing) {
 		var objectsWithComponent = (Component[])Resources.FindObjectsOfTypeAll(componentToSearch);
 		
 		foreach (Component target in objectsWithComponent) {
 			foreach (Type componentType in componentsToAddOrRemove) {
 				if (removing == false) {
 					if (target.gameObject.GetComponent(componentType) == null) {
 						target.gameObject.AddComponent(componentType);
 					}
 				}
 				else {
 					if (target.gameObject.GetComponent(componentType) != null) {
 						DestroyImmediate(target.gameObject.GetComponent(componentType), true);
 					}
 				}
 			}
 		}
 	}
 
 	[ButtonGroup]
 	[Button(ButtonSizes.Medium)]
 	private void Clear() {
 		componentToSearch = null;
 		componentsToAddOrRemove.Clear();
 	}

	private void UpdateList() {
		if (componentsToAdd == 0) {
			showList = false;
		}
		else {
			showList = true;
			componentsToAddOrRemove = new Type[componentsToAdd].ToList();
		}

	}
 }