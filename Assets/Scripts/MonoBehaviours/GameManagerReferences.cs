using System;
using System.Collections;
using System.Collections.Generic;
using Kino;
using TMPro;
using UnityEngine;

public class GameManagerReferences : MonoBehaviour {

	public BallSpawner ballSpawner;
	public List<BackWall> backWalls;
	public Camera mainCamera;
	public AnalogGlitch analogGlitch;
	public Transform net;
	public GameObject resetCanvas;
	public RectTransform backgroundImage;
	public GameObject pauseCanvas;
	public TextMeshProUGUI victoryName;
	public BigScoreAnimator scoreAnimator;
	public AnalogGlitchTransition scoreTransition;
}