using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class BigScoreAnimator : MonoBehaviour {
	[Title("References")]
	public RectTransform leftScore;
	public RectTransform rightScore;
	public RectTransform background;

	public RectAnchorAnimation arriveScoreAnimationLeft;
	public RectAnchorAnimation arriveScoreAnimationRight;
	public RectAnchorAnimation arriveBGAnimation;
	public float animationWait;
	public RectAnchorAnimation leaveScoreAnimationLeft;
	public RectAnchorAnimation leaveScoreAnimationRight;
	public RectAnchorAnimation leaveBGAnimation;

	public TextMeshProUGUI leftScoreText;
	public TextMeshProUGUI rightScoreText;

	[ShowInInspector, Sirenix.OdinInspector.ReadOnly]
	private bool animating;

	private TextMeshProColorSpin leftScoreTextSpin;
	private TextMeshProColorSpin rightScoreTextSpin;

	private void Start() {
		leftScoreTextSpin = leftScoreText.GetComponent<TextMeshProColorSpin>();
		rightScoreTextSpin = rightScoreText.GetComponent<TextMeshProColorSpin>();
	}

	[Button]
	public void MoveScores() {
		animating = true;
		StartCoroutine(arriveScoreAnimationLeft.Animate(leftScore, () => {StartCoroutine(leaveScoreAnimationLeft.Animate(leftScore));}, animationWait));
		StartCoroutine(arriveScoreAnimationRight.Animate(rightScore, () => {StartCoroutine(leaveScoreAnimationRight.Animate(rightScore));}, animationWait));
		StartCoroutine(arriveBGAnimation.Animate(background, () => { StartCoroutine(leaveBGAnimation.Animate(background)); }, animationWait));
	}

	public void UpdateScores(string leftScoreValue, string rightScoreValue, bool bounceLeft = false, bool spinLeft = false, bool bounceRight = false, bool spinRight = false) {
		this.leftScoreText.text = leftScoreValue;
		leftScoreTextSpin.PuffUp = bounceLeft;
		//leftScoreTextSpin.ColorSpin = spinLeft;
		this.rightScoreText.text = rightScoreValue;
		rightScoreTextSpin.PuffUp = bounceRight;
		//rightScoreTextSpin.ColorSpin = spinRight;
	}
}
