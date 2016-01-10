using UnityEngine;
using System.Collections.Generic;

public class EquipmentSystem : MonoBehaviour {

	public List<Item> equipedItems = new List<Item>();
	public List<Equipable> equipments = new List<Equipable>();
	public static EquipmentSystem sys;
	private Animator animator;

	void Start () {
		if (sys == null) {
			sys = this;
		} else if (sys != this) {
			Destroy(gameObject);
		}
		var temp = GetComponentsInChildren<Equipable>();
		foreach(var equipmnt in temp) {
			equipments.Add(equipmnt);
		}

		animator = GetComponent<Animator>();
	}


	// This method adds an item to the list and checks to see if there is a 3D representation for the item that was added.
	public void AddItem(Item item) {
		equipedItems.Add(item);
		foreach(var i in equipedItems) {
			foreach(var g in equipments) {
				if (g.itemID == i.ID) {
					g.gameObject.SetActive(true);
					if (i.Type == "Weapon") {
						animator.SetBool("weaponHold",true);
					}
				}
			}
		}
	}

	public void RemoveItem(Item item) {
		foreach(var i in equipedItems) {
			foreach(var g in equipments) {
				if (g.itemID == i.ID && g.itemID == item.ID) {
					g.gameObject.SetActive(false);
					if (i.Type == "Weapon") {
						animator.SetBool("weaponHold",false);
					}
					break;
				}
			}
		}
		equipedItems.Remove(item);
	}
}
