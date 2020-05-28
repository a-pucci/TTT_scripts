using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "newPillarSettings", menuName = "ScriptableObjects/Settings/PillarSettings")]
public class PillarSettings : ScriptableObject {
	
	public float outputWaitTime;
	[BoxGroup("Output Settings")] 
	public bool fixedOutputStrength;
	[BoxGroup("Output Settings"), ShowIf("fixedOutputStrength")] 
	public float outputStrength;
	[BoxGroup("Output Settings"), ReadOnly]
	public bool fixedOutputDirections;
	[BoxGroup("Output Settings"), ShowIf("fixedOutputDirections")] 
	public List<Vector3> outputDirections;
	[BoxGroup("Output Settings")] 
	public bool addRandomOffset;
	[BoxGroup("Output Settings"), ShowIf("addRandomOffset")] 
	public Vector3 directionOffset;
}

