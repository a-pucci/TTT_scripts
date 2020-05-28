using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectScroll : MonoBehaviour {

	public bool invert;
	public float speed;
	public List<RectTransform> scrollItems;
	public Vector2 limitX;
	public Vector2 startX;

	// Update is called once per frame
	void Update () {
		foreach (RectTransform scrollItem in scrollItems) {
			Vector2 newAnchorMin = scrollItem.anchorMin;
			Vector2 newAnchorMax = scrollItem.anchorMax;

			if (invert && newAnchorMax.x >= limitX.x) {
				newAnchorMin.x -= Mathf.Abs(limitX.x) + Mathf.Abs(startX.x);
				newAnchorMax.x -= Mathf.Abs(limitX.x) + Mathf.Abs(startX.x);
			}
			else if (!invert && newAnchorMin.x <= limitX.x) {
				newAnchorMin.x += Mathf.Abs(limitX.x) + Mathf.Abs(startX.x);
				newAnchorMax.x += Mathf.Abs(limitX.x) + Mathf.Abs(startX.x);
			}
			newAnchorMin.x += invert ? speed : -speed;
			newAnchorMax.x += invert ? speed : -speed;
			scrollItem.anchorMin = newAnchorMin;
			scrollItem.anchorMax = newAnchorMax;
		}
	}
}
