using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class Column : MonoBehaviour {

	#region Fields
	
	// Public
	public ColumnSettings settings;
	public Transform floor;
	
	// Private
	private Vector3 positionActivated;
	private Vector3 positionDeactivated;
	private Transform column;
	private MeshRenderer triggerMesh;
	private Collider triggerCollider;
	private bool activated;
	private SortingGroup sortingGroup;

	#endregion

	#region Unity Callbacks
	[Button]
	private void Start () {
		triggerMesh = GetComponent<MeshRenderer>();
		triggerCollider = GetComponent<Collider>();
		column = transform.parent;
		float halfHeight = column.gameObject.GetComponent<Collider>().bounds.extents.y;
		positionActivated = new Vector3(column.position.x, floor.position.y+halfHeight, column.position.z);
		positionDeactivated = new Vector3(column.position.x, floor.position.y-halfHeight, column.position.z);
		column.position = positionDeactivated;
		
		sortingGroup = transform.parent.gameObject.GetComponent<SortingGroup>();
		sortingGroup.sortingOrder = (int)-transform.position.z;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Character")) {
			settings.effects.Get("Activation")?.Play(transform.position, Vector3.zero);
			activated = true;
			StartCoroutine(WaitForActivation());
		}
	}
	#endregion

	#region Methods

	private IEnumerator WaitForActivation() {
		triggerMesh.material = settings.triggerActivatedMaterial;
		yield return new WaitForSeconds(settings.waitBeforeActivation);
		StartCoroutine(Move(positionActivated, settings.timeToAppear));
	}

	private IEnumerator Move(Vector3 destination, float time) {
		float startTIme = Time.time;
		float elapsedTime = 0;
		Vector3 startingPosition = column.position;
		if (activated) {
			Hide(activated);
		}
		while (elapsedTime < time) {
			elapsedTime = Time.time - startTIme;
			column.position = Vector3.Lerp(startingPosition, destination, (elapsedTime/time));
			yield return null;
		}
		if (!activated) {
			Hide(activated);
		}
		if (activated) {
			StartCoroutine(WaitToDisappear());
		}
	}

	private IEnumerator WaitToDisappear() {
		activated = false;
		yield return new WaitForSeconds(settings.timeActivated);
		StartCoroutine(Move(positionDeactivated, settings.timeToDisappear));
	}

	private void Hide(bool value) {
		if (!value) {
			triggerMesh.material = settings.triggerMaterial;
		}
		triggerMesh.enabled = !value;
		triggerCollider.enabled = !value;
	}
	
	#endregion

}