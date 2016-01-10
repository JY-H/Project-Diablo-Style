// This script can be attached to a slot,to allow the player to press a key in order to consume an item from the parent slot.

using UnityEngine;
public class HotbarSlot : MonoBehaviour {

	public KeyCode key;
	Slot slot;

	void Start() {
		slot = GetComponent<Slot>();
	}

	void Update() {
		if( Input.GetKeyDown( key ) ) {
			slot.UseItem();
		}
	}
}
