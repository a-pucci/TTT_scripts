using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSpriteCollection", menuName = "ScriptableObjects/SpriteCollection")]
public class SpriteCollection : ScriptableObject {

	#region Fields
	
	// Public
	public List<Sprite> collection;
	
	#endregion
}