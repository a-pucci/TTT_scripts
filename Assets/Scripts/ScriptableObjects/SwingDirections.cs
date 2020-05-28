using UnityEngine;

[CreateAssetMenu(fileName = "swingDirections", menuName = "ScriptableObjects/Swing Directions")]
public class SwingDirections : ScriptableObject {
	public Vector3 leftForward = new Vector3(-0.5f,0.2f,1);
	public Vector3 straightForward = new Vector3(0,0.2f,1);
	public Vector3 rightForward = new Vector3(0.5f,0.2f,1);
	public Vector3 leftBack = new Vector3(-0.5f,0.2f,-1);
	public Vector3 straightBack = new Vector3(0,0.2f,-1);
	public Vector3 rightBack = new Vector3(0.5f,0.2f,-1);
}
