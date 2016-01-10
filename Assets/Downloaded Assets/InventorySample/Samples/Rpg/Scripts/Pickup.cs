// When attached to a GameObject,this script checks the mouse for being over the object,
// and also if the player clicks while the cursor is on the object,then it adds the corresponding item to an Inventory window

using UnityEngine;

[AddComponentMenu("Inventory/Samples/Pickup")]
public class Pickup : MonoBehaviour {

	public int itemID,amount;
	public string windowName;
	private InventoryWindow inventory;
	private Color initialColor;

	void Start () {
		foreach (var window in InventoryManager.manager.windows) {
			if (window.windowName == windowName) {
				inventory = window;
				break;
			}
		}
		initialColor = GetComponent<Renderer>().material.color;
	}

	void OnMouseEnter() {
		GetComponent<Renderer>().material.color = Color.green;
	}

	void OnMouseExit() {
		GetComponent<Renderer>().material.color = initialColor;
	}

	void OnMouseDown() {
		if (inventory != null) {
			inventory.AddItem(itemID,amount);
			Destroy (gameObject);
		}
		else {
			Debug.LogWarning("'" + windowName + "'" + " not found in scene");
		}
	}
}
