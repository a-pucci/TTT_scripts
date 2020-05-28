using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Lightning : MonoBehaviour {

	#region Fields

	// Public
	public LightningSettings lightning;
	

	#endregion

	private LightningStruct SetNewLightning() {
		var newLightning = new GameObject("Lightning");
		var renderers = new List<LineRenderer>();
		
		for (int i = 0; i < lightning.lines; i++) {
			var child = new GameObject("Line");
			child.transform.SetParent(newLightning.transform);
			var line = child.AddComponent<LineRenderer>();
			line.startWidth = lightning.lineWidth;
			line.endWidth = lightning.lineWidth;
			line.material = lightning.material;
			line.positionCount = lightning.lineVertex;
			renderers.Add(line);
		}
		var boo = new LightningStruct {lightning = newLightning, renderers = renderers};
		return boo;
	}

	#region Methods

	public void CreateLightning(Vector3 source, Vector3 target) {
		LightningStruct lightningStruct = SetNewLightning();
		
		lightningStruct.source = source;
		lightningStruct.target = target;
		StartCoroutine((ShowLightning(lightningStruct)));
	}
	
	private IEnumerator ShowLightning(LightningStruct lightningStruct) {
		float initialTime = Time.time;
		float elapsedTime = 0;
		
		AudioManager.instance.PlaySound(lightning.lightningSfx, true);

		while (elapsedTime < lightning.duration) {
			DrawLightning(lightningStruct);
			elapsedTime = Time.time - initialTime;
			yield return new WaitForSeconds(lightning.changeTime);
		}

		Destroy(lightningStruct.lightning);
	}

	private void DrawLightning(LightningStruct lightningStruct) {
		foreach (LineRenderer line in lightningStruct.renderers) {
			
			var positions = new Vector3[lightning.lineVertex];

			// start point
			positions[0] = lightningStruct.source;
			
			// middle points
			for(int i = 1; i < positions.Length; i++)
			{
				Vector3 pos = Vector3.Lerp(lightningStruct.source, lightningStruct.target, (float) i/(positions.Length));
				pos.x += Random.Range(-lightning.lineOffset, lightning.lineOffset);
				pos.y += Random.Range(-lightning.lineOffset, lightning.lineOffset);
 
				positions[i] = pos;
			}
			
			// end point
			positions[positions.Length - 1] = lightningStruct.target;
			
			line.SetPositions(positions);
			
			line.positionCount = lightning.lineVertex;
		}
	}

#endregion

	private class LightningStruct {
		public GameObject lightning;
		public List<LineRenderer> renderers;
		public Vector3 source;
		public Vector3 target;
	}
}