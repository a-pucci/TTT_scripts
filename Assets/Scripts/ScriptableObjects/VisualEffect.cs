using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewVisualEffect", menuName = "ScriptableObjects/Effects/Visual Effect")]

public class VisualEffect : ScriptableObject {
	public List<GameObject> controllers;
	public List<GameObject> particles;
	public List<AudioClip> audioClips;
	public AnalogGlitchTransition analogGlitchTransition;
	public bool pitchShift;
	public float volumeLevel;
	public bool audioAnimation;
	[ShowIf("audioAnimation")]
	public float audioAnimationTime;
	[ShowIf("audioAnimation")]
	public AnimationCurve audioAnimationCurve;
	[ShowIf("audioAnimation")]
	public float audioStartingVolume;
	[ShowIf("audioAnimation")]
	public float audioFinishVolume;
	[ShowIf("audioAnimation")]
	public float audioStartingPitch;
	[ShowIf("audioAnimation")]
	public float audioFinishPitch;
	[ShowIf("audioAnimation")]
	public bool holdAudio;

	public Vector3 localPosition;
	public Quaternion localRotation;
	public Vector3 localScale = Vector3.one;
	
	public void Play(Vector3 position, Vector3 offset, Transform parent = null, Color color = default(Color), float lifetime = 0f ) {
		if (audioClips != null && audioClips.Count > 0 && !audioAnimation) {
			AudioManager.instance.PlaySound(audioClips[Random.Range(0, audioClips.Count)], pitchShift, volumeLevel);
		}
		else if (audioAnimation) {
			AudioManager.instance.PlayAudioAnimation(audioClips[0], audioAnimationCurve, audioAnimationTime, audioStartingVolume, audioFinishVolume, audioStartingPitch, audioFinishPitch, holdAudio, parent);
		}

		if (controllers != null) {
			foreach (GameObject controller in controllers) {
				GameObject effect = Instantiate(controller, position + offset, localRotation, parent);
			
				effect.transform.localPosition = parent == null ? effect.transform.localPosition + localPosition + offset : localPosition + offset;
				effect.transform.localRotation = parent == null ? effect.transform.localRotation : localRotation;
				effect.transform.localScale = parent == null ? effect.transform.localScale + localScale : localScale;
				if (lifetime != 0) {
					Destroy(effect, lifetime);
				}
			}
		}
		if (particles != null) {
			foreach (GameObject particle in particles) {
				GameObject effect = Instantiate(particle, position + offset, localRotation, parent);
			
				effect.transform.localPosition = parent == null ? effect.transform.localPosition + localPosition + offset : localPosition + offset;
				effect.transform.localRotation = parent == null ? effect.transform.localRotation : localRotation;
				effect.transform.localScale = parent == null ? effect.transform.localScale + localScale : localScale;
				
				if (color != default(Color)) {
					ParticleSystem.MainModule mainModule = effect.GetComponent<ParticleSystem>().main;
					mainModule.startColor = color;
				}
				effect.GetComponent<ParticleSystem>().Play();
				
				
				if (lifetime != 0) {
					Destroy(effect, lifetime);
				}
			}
		}
		if (analogGlitchTransition) {
			GameManager.Instance.StartCoroutine(analogGlitchTransition.Animate(GameManager.Instance.analogGlitch));
		}
	}
}
