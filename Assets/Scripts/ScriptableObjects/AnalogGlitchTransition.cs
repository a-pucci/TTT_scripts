using System;
using System.Collections;
using Kino;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AnalogGlitchTransition", menuName = "ScriptableObjects/Analog Glitch Transition")]
public class AnalogGlitchTransition : ScriptableObject {
	public float animationTime;
	public AnimationCurve animationCurve;
	[Range(0,1)]
	public float scanLineJitter;
	[Range(0,1)]
	public float verticalJump;
	[Range(0,1)]
	public float horizontalShake;
	[Range(0,1)]
	public float colorDrift;
	public bool audioAnimation;
	[ShowIf(nameof(audioAnimation))]
	public bool destroyAudioWhenTransitionOver = true;
	[ShowIf(nameof(audioAnimation))]
	public AudioClip audioClip;
	[ShowIf(nameof(audioAnimation))]
	public AnimationCurve audioAnimationCurve;
	[ShowIf(nameof(audioAnimation))]
	public float audioStartingVolume;
	[ShowIf(nameof(audioAnimation))]
	public float audioFinishVolume;
	[ShowIf(nameof(audioAnimation))]
	public float audioStartingPitch;
	[ShowIf(nameof(audioAnimation))]
	public float audioFinishPitch;
	public bool returnToPrevious;
	[ShowIf(nameof(returnToPrevious))]
	public float returnAnimationTime;
	[ShowIf(nameof(returnToPrevious))]
	public AnimationCurve returnAnimationCurve;
	[ShowIf(nameof(returnToPrevious))]
	[Range(0,1)]
	public float returnScanLineJitter;
	[ShowIf(nameof(returnToPrevious))]
	[Range(0,1)]
	public float returnVerticalJump;
	[ShowIf(nameof(returnToPrevious))]
	[Range(0,1)]
	public float returnHorizontalShake;
	[ShowIf(nameof(returnToPrevious))]
	[Range(0,1)]
	public float returnColorDrift;
	[ShowIf(nameof(audioAnimation))]
	[ShowIf(nameof(returnToPrevious))]
	public AnimationCurve returnAudioAnimationCurve;
	[ShowIf(nameof(audioAnimation))]
	[ShowIf(nameof(returnToPrevious))]
	public float returnAudioStartingVolume;
	[ShowIf(nameof(audioAnimation))]
	[ShowIf(nameof(returnToPrevious))]
	public float returnAudioFinishVolume;
	[ShowIf(nameof(audioAnimation))]
	[ShowIf(nameof(returnToPrevious))]
	public float returnAudioStartingPitch;
	[ShowIf(nameof(audioAnimation))]
	[ShowIf(nameof(returnToPrevious))]
	public float returnAudioFinishPitch;

	public Action AnimationFinished;

	public IEnumerator Animate(AnalogGlitch analogGlitch) {
		float startScanLineJitter = analogGlitch.scanLineJitter;
		float startVerticalJump = analogGlitch.verticalJump;
		float startHorizontalShake = analogGlitch.horizontalShake;
		float startColorDrift = analogGlitch.colorDrift;
		float t = 0;
		GameObject go = new GameObject();
		if (audioAnimation) {
			AudioManager.instance.PlayAudioAnimation(audioClip, audioAnimationCurve, animationTime, audioStartingVolume, audioFinishVolume, audioStartingPitch, audioFinishPitch, true, go.transform);
		}
		while (t <= animationTime) {
			float delta = animationCurve.Evaluate(Mathf.InverseLerp(0, animationTime, t));
			analogGlitch.scanLineJitter = Mathf.Lerp(startScanLineJitter, scanLineJitter, delta);
			analogGlitch.verticalJump = Mathf.Lerp(startVerticalJump, verticalJump, delta);
			analogGlitch.horizontalShake = Mathf.Lerp(startHorizontalShake, horizontalShake, delta);
			analogGlitch.colorDrift = Mathf.Lerp(startColorDrift, colorDrift, delta);
			t += Time.deltaTime;
			yield return null;
		}
		analogGlitch.scanLineJitter = scanLineJitter;
		analogGlitch.verticalJump = verticalJump;
		analogGlitch.horizontalShake = horizontalShake;
		analogGlitch.colorDrift = colorDrift;
		if (returnToPrevious) {
			if (audioAnimation) {
				Destroy(go.transform.GetChild(0).gameObject);
				AudioManager.instance.PlayAudioAnimation(audioClip, returnAudioAnimationCurve, returnAnimationTime, returnAudioStartingVolume, returnAudioFinishVolume, returnAudioStartingPitch, returnAudioFinishPitch, true, go.transform);
			}
			t = 0;
			while (t <= returnAnimationTime) {
				float delta = returnAnimationCurve.Evaluate(Mathf.InverseLerp(0, returnAnimationTime, t));
				analogGlitch.scanLineJitter = Mathf.Lerp(scanLineJitter, returnScanLineJitter, delta);
				analogGlitch.verticalJump = Mathf.Lerp(verticalJump, returnVerticalJump, delta);
				analogGlitch.horizontalShake = Mathf.Lerp(horizontalShake, returnHorizontalShake, delta);
				analogGlitch.colorDrift = Mathf.Lerp(colorDrift, returnColorDrift, delta);
				t += Time.deltaTime;
				yield return null;
			}
			analogGlitch.scanLineJitter = returnScanLineJitter;
			analogGlitch.verticalJump = returnVerticalJump;
			analogGlitch.horizontalShake = returnHorizontalShake;
			analogGlitch.colorDrift = returnColorDrift;
		}
		if (audioAnimation && destroyAudioWhenTransitionOver) {
			Destroy(go);
		}
		AnimationFinished?.Invoke();
	}
}
