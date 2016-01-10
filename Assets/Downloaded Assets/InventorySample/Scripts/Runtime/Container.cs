using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Inventory/Container")]
public class Container : MonoBehaviour {

	// General
	[HideInInspector]
	public InventoryWindow parentWindow;
	public bool isCraftWindow = false;
	public bool isNormalContainer = true;
	public int orderInWindow = 0;

	// Crafting
	public Slot receiver;
	public int inspectedRecipe;
	public bool previewExists = false;
	public int numberOfRecipes;
	
	public bool visible = true;
	public Vector3 scale;
	
	private List<Slot> slots = new List<Slot>();

	public List<Slot> GetSlots { get { SearchForSlots();return slots; } }


	void Start() {
		parentWindow = GetComponentInParent<InventoryWindow>();
		SearchForSlots();
		// Adding the initial item
		foreach(var slot in slots) {
			var initial = slot.initialItem;
			if (initial.itemName != "none") {
				slot.AssignItem = InventoryManager.manager.RetrieveItem(initial.itemName,initial.amount,slot);
			}
		}

		if (isNormalContainer) {
			if(GetComponent<InventoryWindow>() != null) {
				GetComponent<InventoryWindow>().RegisterContainer(this);
			}
			else if(GetComponentInParent<InventoryWindow>() != null) {
				GetComponentInParent<InventoryWindow>().RegisterContainer(this);
			}
		}
	}


	// This method finds the slots that are children of this container
	void SearchForSlots() {
		slots = new List<Slot>();
		var foundSlots = GetComponentsInChildren<Slot>();
		foreach(var slot in foundSlots) {
			slots.Add(slot);
			slot.parentContainer = this;
			slot.receiver = false;
		}
		if (receiver != null)
			receiver.receiver = true;
	}

	// This method tries to equip an item( meaning that it tries to move an item to a specific window / slot )
	public bool TryEquip(Slot initialSlot) {
		bool equipedSuccesful = false;
		foreach (var slot in slots) {
			if (slot.mask.mask == initialSlot.CurrentItem.Type && slot.CurrentItem == null) {
				slot.AssignItem = initialSlot.CurrentItem;
				initialSlot.DiscardItem(false);
				equipedSuccesful = true;
				break;
			}
			else if (slot.mask.mask == initialSlot.CurrentItem.Type && slot.CurrentItem != null) {
				Item temp = slot.CurrentItem;
				slot.AssignItem = initialSlot.CurrentItem;
				initialSlot.AssignItem = temp;
				equipedSuccesful = true;
				break;
			}
		}
		return equipedSuccesful;
	}


	public bool TryUnEquip(Slot equipmentSlot) {
		bool unequipedSuccesful = false;
		foreach (var slot in slots) {
			if (slot.mask.mask == "All" && slot.CurrentItem == null) {
				slot.AssignItem = equipmentSlot.CurrentItem;
				equipmentSlot.DiscardItem(false);
				unequipedSuccesful = true;
				break;
			}
		}
		return unequipedSuccesful;
	}

	// This method checks all the slots for a recipe,and if some formula is found,then it shows a preview of the result
	public void CheckForRecipe( bool rec ) {
		int foundRecipe = -1;
		int amount = 1;
		int startingToCountFrom = 0;
		bool resultIsStackable = false;
		bool dontCheckAnymore = false;

		for( int i = 0;i < numberOfRecipes;i ++ ) {
			resultIsStackable = InventoryManager.manager.ItemIsStackable( receiver.recipes[i].itemName );
			for( int j = 0;j < slots.Count;j ++ ) {
				if( !slots[j].CheckRecipe( i ) ) {
					break;
				}
				else if( resultIsStackable ) {
					if( !slots[j].receiver && slots[j].recipes[i].amount != 0 ) {
						startingToCountFrom = j;
						amount = (slots[j].CurrentInStack / slots[j].recipes[i].amount);
					}
					else if( j != startingToCountFrom && !slots[j].receiver && slots[j].recipes[i].amount != 0 ) {
						int amountInThisSlot = (slots[j].CurrentInStack / slots[j].recipes[i].amount);
						if( amount > amountInThisSlot ) {
							amount = amountInThisSlot;
						}
					}
				}
				if( j == slots.Count - 1 ) {
					foundRecipe = i;
				}
			}
			if( foundRecipe != -1 ) {
				break;
			}
		}
		if( foundRecipe == -1 ) {
			receiver.DiscardItem( true );
			return;
		}
		else if(rec) {
			foreach( var slot in slots ) {
				if( !slot.receiver ) {
					slot.CurrentInStack -= slot.recipes[foundRecipe].amount * amount;
					if( slot.CurrentInStack < slot.recipes[foundRecipe].amount ) 
						dontCheckAnymore = true;
				}
			}
			if( resultIsStackable || dontCheckAnymore )
				return;
			else 
				CheckForRecipe( false );
		}
		receiver.DiscardItem( true );
		receiver.AssignItem = InventoryManager.manager.RetrieveItem( receiver.recipes[foundRecipe].itemName,receiver.recipes[foundRecipe].amount * amount,receiver );
	}


	// This property toggles the visibility of this container ON / OFF
	public bool Visible {
		get { return visible; }
		set {
			visible = value;
			if (value) {
				if (scale != Vector3.zero)
					transform.localScale = scale;
				else
					transform.localScale = Vector3.one;
			}
			else
				transform.localScale = Vector3.zero;
		}
	}
}