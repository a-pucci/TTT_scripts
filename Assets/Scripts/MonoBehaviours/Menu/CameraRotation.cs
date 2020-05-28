using UnityEngine;

public class CameraRotation : MonoBehaviour {

	#region Fields

	public Transform target;
	public float rotationSpeed;
	public float rotationAngle;

	private Vector3 startingRotation;

	#endregion

	#region Unity Callbacks

	private void Start () {
		startingRotation = transform.eulerAngles;
	}
	
	private void Update () {
		transform.RotateAround (target.position, Vector3.down, Time.deltaTime * rotationSpeed);
		if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, startingRotation.y )) > rotationAngle/2) {
			rotationSpeed *= -1;
		}
	}

	#endregion

}