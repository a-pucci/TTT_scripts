using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LookAtComponent : MonoBehaviour {

	//Gameobject to look at. Should always be the current camera.
	//TODO Set LookAtObject to current camera always
	public GameObject LookAtObject;
	private SpriteRenderer spriteRenderer;
	private SortingGroup sortingGroup;

	private void Start() {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		sortingGroup = transform.parent.gameObject.GetComponent<SortingGroup>();
		LookAtObject = GameObject.FindGameObjectWithTag("MainCamera");
	}

	// Update is called once per frame
	private void Update () {
		//spriteRenderer.sortingOrder = (int)-transform.position.z;
		if(sortingGroup){
			sortingGroup.sortingOrder = (int)-transform.position.z;
		}
		float originalZ = transform.localRotation.eulerAngles.z;
		transform.LookAt(LookAtObject.transform);
		transform.localRotation = Quaternion.Euler(new Vector3(-transform.localRotation.eulerAngles.x,0,originalZ));
	}
}
