using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Sirenix.OdinInspector;

public class IntroManager : MonoBehaviour {

	public bool skip;
	public AudioClip introMusic;

	[LabelText("Character"), FoldoutGroup("Top Player")]
	public CharacterManager topPlayerCharacter;
	[LabelText("CharacterAnimator"), FoldoutGroup("Top Player")]
	public Animator topPlayerCharacterAnimator;
	[LabelText("Text"), FoldoutGroup("Top Player")]
	public RectTransform topPlayerText;
	[LabelText("TextAnimation"), FoldoutGroup("Top Player")]
	public RectAnchorAnimation topPlayerTextAnimation;
	[LabelText("Image"), FoldoutGroup("Top Player")]
	public RectTransform topPlayerImage;
	[LabelText("ImageAnimation"), FoldoutGroup("Top Player")]
	public RectAnchorAnimation topPlayerImageAnimation;
	[LabelText("CameraPosition"), FoldoutGroup("Top Player")]
	public Vector3 topPlayerCameraPosition;

	[LabelText("Character"), FoldoutGroup("Bottom Player")]
	public CharacterManager bottomPlayerCharacter;
	[LabelText("CharacterAnimator"), FoldoutGroup("Bottom Player")]
	public Animator bottomPlayerCharacterAnimator;
	[LabelText("Text"), FoldoutGroup("Bottom Player")]
	public RectTransform bottomPlayerText;
	[LabelText("TextAnimation"), FoldoutGroup("Bottom Player")]
	public RectAnchorAnimation bottomPlayerTextAnimation;
	[LabelText("Image"), FoldoutGroup("Bottom Player")]
	public RectTransform bottomPlayerImage;
	[LabelText("ImageAnimation"), FoldoutGroup("Bottom Player")]
	public RectAnchorAnimation bottomPlayerImageAnimation;
	[LabelText("CameraPosition"), FoldoutGroup("Bottom Player")]
	public Vector3 bottomPlayerCameraPosition;

	[LabelText("Transform"), FoldoutGroup("CameraMovements")]
	public Transform cameraTransform;
	[LabelText("AnalogGlitch"),FoldoutGroup("CameraMovements")]
	public Kino.AnalogGlitch cameraAnalogGlitch;
	[LabelText("VFX"), FoldoutGroup("CameraMovements")]
	public GameObject cameraVFX;
	[LabelText("Zoom Time"), FoldoutGroup("CameraMovements")]
	public float cameraZoomToPlayer;
	[LabelText("Wait after Zoom Time"), FoldoutGroup("CameraMovements")]
	public float cameraWait;
	[LabelText("Zoom Curve"), FoldoutGroup("CameraMovements")]
	public AnimationCurve cameraAnimationCurve;

	[FoldoutGroup("OtherReferences")]
	public RectTransform versus;
	[FoldoutGroup("OtherReferences")]
	public RectAnchorAnimation versusAnimation;
	[FoldoutGroup("OtherReferences")]
	public BallSpawner ballSpawner;
	[LabelText("UI"), FoldoutGroup("OtherReferences")]
	public GameObject ui;
	[FoldoutGroup("OtherReferences")]
	public AnalogGlitchTransition startingTransition;
	[FoldoutGroup("OtherReferences")]
	public AnalogGlitchTransition finishingTransitionStart;
	[FoldoutGroup("OtherReferences")]
	public AnalogGlitchTransition finishingTransitionFinish;

	private Vector3 startCameraPosition;
	private bool isFinishing;
	private AudioSource audioSource;
	private AudioClip startClip;

	// Use this for initialization
	void Start () {
		audioSource = AudioManager.instance.GetComponent<AudioSource>();
		startClip = audioSource.clip;
		StartCoroutine(StartIntro());
	}

	private void Update() {
		if (!Photon.Pun.PhotonNetwork.IsConnected && !isFinishing && (Input.GetButton("Pause1") || Input.GetButton("Pause2"))) {
			StopAllCoroutines();
			StartCoroutine(FinishIntro());
		}
	}

	private IEnumerator StartIntro() {
		if (!skip) {
			audioSource.clip = introMusic;
			audioSource.Play();
			//Setup
			GameManager.Instance.canPause = false;
			topPlayerCharacter.enabled = false;
			bottomPlayerCharacter.enabled = false;
			startCameraPosition = cameraTransform.position;
			cameraVFX.SetActive(false);
			ui.SetActive(false);
			topPlayerText.gameObject.SetActive(true);
			topPlayerImage.gameObject.SetActive(true);
			bottomPlayerText.gameObject.SetActive(true);
			bottomPlayerImage.gameObject.SetActive(true);
			versus.gameObject.SetActive(true);
			//Starting Transition
			yield return StartCoroutine(startingTransition.Animate(cameraAnalogGlitch));
			//Top Player Animation
			cameraVFX.SetActive(true);
			yield return StartCoroutine(MoveCamera(startCameraPosition, topPlayerCameraPosition));
			topPlayerCharacter.swingType.swingEffects.Get("Swing").Play(transform.position, Vector3.zero);
			topPlayerCharacterAnimator.SetBool("IsSwingingRight", true);
			yield return new WaitForSeconds(cameraWait);
			topPlayerCharacterAnimator.SetBool("IsSwingingRight", false);
			cameraTransform.position = startCameraPosition;
			//Bottom Player Animation
			yield return StartCoroutine(MoveCamera(startCameraPosition, bottomPlayerCameraPosition));
			bottomPlayerCharacter.swingType.swingEffects.Get("Swing").Play(transform.position, Vector3.zero);
			bottomPlayerCharacterAnimator.SetBool("IsSwingingLeft", true);
			yield return new WaitForSeconds(cameraWait);
			bottomPlayerCharacterAnimator.SetBool("IsSwingingLeft", false);
			cameraVFX.SetActive(false);
			//Return to center
			yield return StartCoroutine(MoveCamera(bottomPlayerCameraPosition, startCameraPosition));
			//Showing Player Images and Text and Versus Image
			yield return StartCoroutine(versusAnimation.Animate(versus));
			StartCoroutine(topPlayerImageAnimation.Animate(topPlayerImage));
			StartCoroutine(topPlayerTextAnimation.Animate(topPlayerText));
			StartCoroutine(bottomPlayerImageAnimation.Animate(bottomPlayerImage));
			yield return StartCoroutine(bottomPlayerTextAnimation.Animate(bottomPlayerText));
			yield return new WaitForSeconds(cameraWait);
			yield return StartCoroutine(FinishIntro());
		}
		else {
			ballSpawner.StartSpawning();
			Destroy(gameObject);
		}
	}

	private IEnumerator MoveCamera(Vector3 pointA, Vector3 pointB) {
		float elapsedTime = 0;
		while (elapsedTime < cameraZoomToPlayer) {
			var delta = cameraAnimationCurve.Evaluate(elapsedTime/cameraZoomToPlayer);
			cameraTransform.position = Vector3.Lerp(pointA, pointB, delta);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator FinishIntro() {
		audioSource.Stop();
		isFinishing = true;
		//Finishing Transition
		yield return StartCoroutine(finishingTransitionStart.Animate(cameraAnalogGlitch));
		cameraTransform.position = startCameraPosition;
		topPlayerText.gameObject.SetActive(false);
		topPlayerImage.gameObject.SetActive(false);
		bottomPlayerText.gameObject.SetActive(false);
		bottomPlayerImage.gameObject.SetActive(false);
		versus.gameObject.SetActive(false);
		yield return StartCoroutine(finishingTransitionFinish.Animate(cameraAnalogGlitch));
		//Closing
		topPlayerCharacter.enabled = true;
		bottomPlayerCharacter.enabled = true;
		cameraVFX.SetActive(false);
		ui.SetActive(true);
		isFinishing = false;
		GameManager.Instance.canPause = true;
		ballSpawner.StartSpawning();
		if(PhotonNetwork.IsConnected) ballSpawner.photonView.RPC(nameof(ballSpawner.AnimationCompleted), RpcTarget.All, PhotonNetwork.IsMasterClient);
		audioSource.clip = startClip;
		audioSource.loop = true;
		audioSource.Play();
		Destroy(gameObject);
	}
}
