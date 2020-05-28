using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TrapSurface : Surface {
	private StunnableSurface character;

	#region Methods

	public void Stun(float stunTime) {
		if (character) {
			character.TrapStun(stunTime);
		}
	}

	public override void OnTriggerEnter(Collider other) {
		 if (other.gameObject.CompareTag("Character")) {
			 character = other.GetComponent<StunnableSurface>();
		 }
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("Character")) {
			character = null;
		}
	}
	
	[PunRPC]
	protected override void PlayBounceEffect(Vector3 position) {
		//
	}

	#endregion

}