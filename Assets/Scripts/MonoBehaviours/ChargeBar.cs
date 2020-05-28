using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour {

	public RectTransform lightBarRect;
	public RectTransform strongBarRect;

	public Image lightBar;
	public Image strongBar;
	
	private float lightChargeDuration;
	private float strongChargeDuration;

	public void SetValues(float lightDuration, float strongDuration) {
		lightChargeDuration = lightDuration;
		strongChargeDuration = strongDuration;
		float totalDuration = lightDuration + strongDuration;
		float rectPercentage = (lightDuration / totalDuration);
		lightBarRect.anchorMax = new Vector2(rectPercentage, lightBarRect.anchorMax.y);
		strongBarRect.anchorMin = new Vector2(rectPercentage, strongBarRect.anchorMin.y);
		lightBar.fillAmount = 0;
		strongBar.fillAmount = 0;
	}

	public void UpdateLightBar(float time) {
		lightBar.fillAmount = time / lightChargeDuration;
	}
	
	public void UpdateStrongBar(float time) {
		strongBar.fillAmount = time / strongChargeDuration;
	}

	public void ResetBars() {
		lightBar.fillAmount = 0;
		strongBar.fillAmount = 0;
	}
}
