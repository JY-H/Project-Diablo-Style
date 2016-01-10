// This script can be attached to a slot 
// This script checks the slot for being populated by an item,and if it is,then it shows the 3d model that coresponds with the item that is in the slot.

using UnityEngine;

public class Equipment : MonoBehaviour {

	public Equipments[] equipments; 			// A list of ----  ITEM ID - 3D object ----   associations.
	private Slot slot;											// The slot that is being checked
	private Equipments activeEquipment = null;					// The active equipment

	void Start () {
		slot = GetComponent<Slot>();
		// Registering the ChangeEquipment function to the event OnEquipmentChange
		Slot.OnEquipmentChange += ChangeEquipment;
	}

	// This function changes the equipment or unequips everything,by case
	public void ChangeEquipment () {
		if (slot.Populated) {
			if ((activeEquipment != null && activeEquipment.itemID != slot.ItemID) || activeEquipment == null) {
				foreach (var eq in equipments) {
					if (eq.itemID == slot.ItemID) {
						eq.targetGO.SetActive(true);
						activeEquipment = eq;
					}
					else
						eq.targetGO.SetActive(false);
				}
			}
		}
		else {
			UnequipAll();
		}
	}

	void UnequipAll() {
		foreach (var eq in equipments) {
			eq.targetGO.SetActive(false);
		}
		activeEquipment = null;
	}
}

[System.Serializable]
public class Equipments {
	public GameObject targetGO;
	public int itemID;
}