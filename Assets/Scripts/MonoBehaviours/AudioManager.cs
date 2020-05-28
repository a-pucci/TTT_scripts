using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;
	public float minRandomPitchShift = -10f;
	public float maxRandomPitchShift = 10f;
	public AudioMixerGroup sfxMixer;
	
	// private void Awake() {
	// 	// if (instance != null && instance != this) {
	// 	// 	Destroy(this);
	// 	// } else {
	// 	// 	instance = this;
	// 	// }
	// }
	
	private void Awake() {
		instance = this;
	}

	public void PlaySound(AudioClip sound, bool pitchShift = false, float volumeLevel = 1f) {
		StartCoroutine(PlaySoundCoroutine(sound, pitchShift, volumeLevel));
	}

	public void PlayAudioAnimation(AudioClip sound, AnimationCurve audioAnimationCurve, float audioAnimationTime, float audioAnimationStart, float audioAnimationFinish, float audioAnimationStartPitch, float audioAnimationFinishPitch, bool audioAnimationHold, Transform parent) {
		StartCoroutine(PlayAudioAnimationCoroutine(sound, audioAnimationCurve, audioAnimationTime, audioAnimationStart, audioAnimationFinish, audioAnimationStartPitch, audioAnimationFinishPitch, audioAnimationHold, parent));
	}

	private IEnumerator PlaySoundCoroutine(AudioClip sound, bool pitchShift, float volumeLevel) {
		var go = new GameObject();
		go.transform.parent = transform;
		var current = go.AddComponent<AudioSource>();

		if (pitchShift) {
			current.pitch = current.pitch + Random.Range(minRandomPitchShift, maxRandomPitchShift);
		}

		current.clip = sound;
		current.volume = volumeLevel;
		current.outputAudioMixerGroup = sfxMixer;
		current.Play();

		while (current.isPlaying) {
			yield return new WaitForSeconds(0.1f);
		}

		Destroy(go);
	}

	private IEnumerator PlayAudioAnimationCoroutine(AudioClip sound, AnimationCurve audioAnimationCurve, float audioAnimationTime, float audioAnimationStart, float audioAnimationFinish, float audioAnimationStartPitch, float audioAnimationFinishPitch, bool audioAnimationHold, Transform parent) {
		var go = new GameObject();
		go.transform.parent = parent;
		var current = go.AddComponent<AudioSource>();

		current.clip = sound;
		current.volume = audioAnimationStart;
		current.pitch = audioAnimationStartPitch;
		current.outputAudioMixerGroup = sfxMixer;
		current.loop = true;
		current.Play();

		float t = 0;
		while (t < audioAnimationTime) {
			current.volume = Mathf.Lerp(audioAnimationStart, audioAnimationFinish, audioAnimationCurve.Evaluate(t));
			current.pitch = Mathf.Lerp(audioAnimationStartPitch, audioAnimationFinishPitch, audioAnimationCurve.Evaluate(t));
			t += Time.deltaTime;
			yield return null;
		}

		if (!audioAnimationHold) {
			Destroy(go);
		}
	}
}
