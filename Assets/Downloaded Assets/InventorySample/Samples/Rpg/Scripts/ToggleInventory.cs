// ON / OFF switch of the inventory.

using UnityEngine;

public class ToggleInventory : MonoBehaviour {
	
	public KeyCode key;
	public InventoryWindow window;


	void Update() {
		if( Input.GetKeyDown( key ) ) {
			window.SetVisible( !window.visible );
		}
	}
}
