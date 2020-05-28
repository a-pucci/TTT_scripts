using Sirenix.OdinInspector;
using UnityEngine;

public class MenuGrid : MonoBehaviour {

	public Collider collider;
	private void OnBecameInvisible() {
		gameObject.SetActive(false);
		Destroy(gameObject);
	}

	public Vector3 Size() {
		return collider.bounds.size;
	}
}