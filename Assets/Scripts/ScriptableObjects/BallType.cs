using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "newBallType", menuName = "ScriptableObjects/Types/Ball Type")]
public class BallType : ScriptableObject {

	#region Enumerators

	public enum DrawType {
		None,
		Sphere,
		Wire
	}

	#endregion

	#region Editor Variables

	[Title("Physics Settings")]
	[Tooltip("Custom gravity applied to ball")]
	public float gravity;
	[Tooltip("Collision radius")]
	public float radius;
	[Tooltip("Ball's current movement speed")]
	[Range(0.01f,0.03f)]
	public float movementSpeed;
	// [Tooltip("Default Dampening Applied on bounce")]
	// [Range(0.1f, 1f)]
	// public float defaultDampening = 0.9f;
	// [Tooltip("Dampening Applied on bounce on No Dampening tagged walls")]
	// [Range(0.1f, 1f)]
	// public float noDampening = 1f;
	[Tooltip ("Sound reproduce when the player hits the ball")]
	public AudioClip ballHitSound;
	[Tooltip ("Sound reproduce when the player hits the ball very hard")]
	public AudioClip ballHitSoundHard;

	// [Title("Collision Masks")]
	// [Tooltip("Will bounce on what is selected")]
	// public LayerMask bounceCollisionMask;
	// [Tooltip("Will swing on what is selected")]
	// public LayerMask swingCollisionMask;
	// [Tooltip("Will hurt what is selected")]
	// public LayerMask hurtCollisionMask;
	//
	// [Title("Trap Settings")]
	// [Tooltip("Which shots enable traps?")]
	// [EnumToggleButtons]
	// public SwingPower trapSwings;
	// [Tooltip("Will bouncing on the walls disable traps?")]
	// public bool wallsDisableTraps;

	[Title("Other Settings")]
	[Tooltip("After How Many Bounces should the ball respawn?")]
	public int bouncesUntilRespawn;
	public float minVelocityToRespawn;
	// [Tooltip("Are bounces on walls considered for respawn count?")]
	// public bool considerVerticalWalls;
	[Tooltip ("Interval between one swing and another")]
	public float swingTimer;
	public EffectCollection swingEffects;

	[Title("Gizmo Settings")]
	[InfoBox("These values are used only when the Gizmo of the ball is enabled.")]
	[Tooltip("Gizmo color")]
	public Color gizmoColor;
	[Tooltip("Draw icon on collision points")]
	public bool gizmoDrawCollision;
	[Tooltip("Icon to draw on collision points")]
	public Sprite gizmoDrawCollisionImage;

	#endregion
}
