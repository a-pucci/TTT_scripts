using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProColorSpin : MonoBehaviour {
	[Title("Color Spin")] public bool colorSpin = true;
	[ShowIf("colorSpin")] public Color colorA;
	[ShowIf("colorSpin")] public Color colorB;
	[ShowIf("colorSpin")] public float colorTime;

	[Title("Puff Animation")] public bool puffUp = true;
	[ShowIf("puffUp")] public float puffUpAnimationTime;
	[ShowIf("puffUp")] public AnimationCurve puffUpAnimation;
	[ShowIf("puffUp")] public Vector3 minSize;
	[ShowIf("puffUp")] public Vector3 maxSize;

	public bool unscaledTime;

	private float colorTimeCounter;
	private float puffTimeCounter;
	private int colorMultiplier = 1;
	private int puffMultiplier = 1;
	private TextMeshProUGUI textMeshProUgui;

	public bool ColorSpin {
		get { return colorSpin; }
		set {
			colorSpin = value;
			ResetColor();
		}
	}

	public bool PuffUp {
		get { return puffUp; }
		set {
			puffUp = value;
			ResetSize();
		}
	}

	// Use this for initialization
	void Start() {
		textMeshProUgui = GetComponent<TextMeshProUGUI>();
		transform.localScale = minSize;
	}

	// Update is called once per frame
	void Update() {
		if (ColorSpin) {
			colorTimeCounter += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * colorMultiplier;
			if (colorTimeCounter >= colorTime) {
				colorTimeCounter = colorTime;
				colorMultiplier = -1;
			}
			else if (colorTimeCounter <= 0) {
				colorTimeCounter = 0;
				colorMultiplier = 1;
			}
			var vertexGradient = new VertexGradient();
			Color top = Color.Lerp(colorA, colorB, colorTimeCounter / colorTime);
			Color bottom = Color.Lerp(colorB, colorA, colorTimeCounter / colorTime);
			vertexGradient.topLeft = top;
			vertexGradient.topRight = top;
			vertexGradient.bottomLeft = bottom;
			vertexGradient.bottomRight = bottom;
			textMeshProUgui.colorGradient = vertexGradient;
		}
		if (PuffUp) {
			puffTimeCounter += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * puffMultiplier;
			if (puffTimeCounter >= puffUpAnimationTime) {
				puffTimeCounter = puffUpAnimationTime;
				puffMultiplier = -1;
			}
			else if (puffTimeCounter <= 0) {
				puffTimeCounter = 0;
				puffMultiplier = 1;
			}
			transform.localScale = Vector3.Lerp(minSize, maxSize, puffUpAnimation.Evaluate(Mathf.InverseLerp(0, puffUpAnimationTime, puffTimeCounter)));
		}
	}
	
	private void ResetColor() {
		colorTimeCounter = 0;
		colorMultiplier = 1;
		textMeshProUgui.colorGradient = new VertexGradient(colorA, colorA, colorB, colorB);
	}
	
	private void ResetSize() {
		puffTimeCounter = 0;
		puffMultiplier = 1;
		transform.localScale = minSize;
	}
}