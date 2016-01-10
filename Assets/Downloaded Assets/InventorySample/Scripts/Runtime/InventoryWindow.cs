using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Inventory/InventoryWindow")]
public class InventoryWindow : MonoBehaviour {

    public string windowName;
	public bool visible;
	public Vector3 scale;
	
    // Item management 
    ItemManagementHandler manager;

	// The containers in this window
	private Container[] containers = new Container[2];
 
	public void TryEquip(Slot initialSlot) {
		foreach(var container in containers) {
			if (container.TryEquip(initialSlot))
				return;
		}
	}

	public void TryUnEquip(Slot equipmentSlot) {
		foreach(var container in containers) {
			if (container.TryUnEquip(equipmentSlot))
				return;
		}
	}

    // Item management
    public bool Contains(int id) { return manager.Contains(id); }
    public int HowManyContains(int id) { return manager.HowManyContains(id); }
    public void AddItem(int id, int amount) { manager.AddItem(id, amount); }
    public void RemoveItem(int id, int amount) { manager.RemoveItem(id, amount); }
    public void RemoveAll() { manager.RemoveAll(); } 

	public void RegisterContainer(Container container) {
		if(containers.Length < container.orderInWindow + 1) {
			var temp = new Container[container.orderInWindow + 1];
			for (int cnt = 0;cnt < containers.Length;cnt ++) {
				temp[cnt] = containers[cnt];
			}
			containers = new Container[container.orderInWindow + 1];
			for (int cnt = 0;cnt < temp.Length;cnt ++) {
				containers[cnt] = temp[cnt];
			}
		}
		containers[container.orderInWindow] = container;
		manager = new ItemManagementHandler(containers);
	}

	public void SetVisible(bool visibility) {
		if (visibility) {
			transform.localScale = scale;
			visible = true;
		} else {
			scale = transform.localScale;
			transform.localScale = Vector3.zero;
			visible = false;
		}
	}
}















