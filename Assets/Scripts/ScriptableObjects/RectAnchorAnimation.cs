using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "rectAnchorAnimation", menuName = "ScriptableObjects/RectAnchorAnimation")]
public class RectAnchorAnimation : ScriptableObject {
	public float time;
	public AnimationCurve curve;
	[InfoBox("X => xMin\nY => yMin\nZ => xMax\nW => yMax")]
	public Vector4 anchors = new Vector4(0, 0, 0.5f, 1f);

	public IEnumerator Animate(RectTransform rect, Action finished = null, float finishedWait = 0) {
		Vector2 anchorMin = rect.anchorMin;
		Vector2 anchorMax = rect.anchorMax;
		var arrivingAnchorMin = new Vector2(anchors.x, anchors.y);
		var arrivingAnchorMax = new Vector2(anchors.z, anchors.w);
		float t = 0;
		while (t <= time) {
			t += Time.deltaTime;
			rect.anchorMin = Vector2.LerpUnclamped(anchorMin, arrivingAnchorMin, curve.Evaluate(Mathf.InverseLerp(0, time, t)));
			rect.anchorMax = Vector2.LerpUnclamped(anchorMax, arrivingAnchorMax, curve.Evaluate(Mathf.InverseLerp(0, time, t)));
			yield return null;
		}
		rect.anchorMin = arrivingAnchorMin;
		rect.anchorMax = arrivingAnchorMax;
		yield return new WaitForSeconds(finishedWait);
		finished?.Invoke();
	}
}