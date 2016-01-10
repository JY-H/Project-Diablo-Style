using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Inventory/Samples/MultiPurposeButton")]
public class MultiPurposeButton : MonoBehaviour {

	public enum FuncType { Sell,Buy,Discard,Equip,Unequip };
	public FuncType funcType;
	public bool useSelectedSlot;				

	private Slot parentSlot;

	void Start () {
		parentSlot = GetComponentInParent<Slot>();
	}


	// Base on the function type,this method executes the specific function,like equipping an item,discarding,selling ....
	public void Execute () {
		if (!useSelectedSlot) {
			if (funcType == FuncType.Sell) {
				if (!EquipmentSystem.sys.equipedItems.Contains(parentSlot.CurrentItem)) {
					MoneySystem.sys.money += parentSlot.GetItemAttribute("Value");
					parentSlot.DiscardItem(true);
				}
			} 
			else if (funcType == FuncType.Discard) {
				if (!EquipmentSystem.sys.equipedItems.Contains(parentSlot.CurrentItem)) {
					parentSlot.DiscardItem(true);
				}
			} 
			else if (parentSlot != null && parentSlot.Populated && funcType == FuncType.Equip) {
				if (!EquipmentSystem.sys.equipedItems.Contains(parentSlot.CurrentItem)) {
					EquipmentSystem.sys.AddItem(parentSlot.CurrentItem);
				}
			}
			else if (parentSlot != null && parentSlot.Populated && funcType == FuncType.Unequip) {
				if (EquipmentSystem.sys.equipedItems.Contains(parentSlot.CurrentItem)) {
					EquipmentSystem.sys.RemoveItem(parentSlot.CurrentItem);
				}
			}
			else {
				MoneySystem.sys.money -= parentSlot.GetItemAttribute("Value");
				InventoryManager.manager.AddItemToWindow("Inventory",parentSlot.ItemID,1);
			}
		} 
		else {
			if (funcType == FuncType.Sell) {
				if (!EquipmentSystem.sys.equipedItems.Contains(Slot.selectedSlot.CurrentItem)) {
					MoneySystem.sys.money += Slot.selectedSlot.GetItemAttribute("Value");
					Slot.selectedSlot.DiscardItem(true);
				}
			} 
			else if (Slot.selectedSlot != null && Slot.selectedSlot.Populated && funcType == FuncType.Equip) {
				if (!EquipmentSystem.sys.equipedItems.Contains(Slot.selectedSlot.CurrentItem)) {
					EquipmentSystem.sys.AddItem(Slot.selectedSlot.CurrentItem);
				}
			}
			else if (Slot.selectedSlot != null && Slot.selectedSlot.Populated && funcType == FuncType.Unequip) {
				if (EquipmentSystem.sys.equipedItems.Contains(Slot.selectedSlot.CurrentItem)) {
					EquipmentSystem.sys.RemoveItem(Slot.selectedSlot.CurrentItem);
				}
			}
			else if (funcType == FuncType.Discard) {
				if (!EquipmentSystem.sys.equipedItems.Contains(Slot.selectedSlot.CurrentItem)) {
					Slot.selectedSlot.DiscardItem(true);
				}
			} 
			else {
				MoneySystem.sys.money -= Slot.selectedSlot.GetItemAttribute("Value");
				InventoryManager.manager.AddItemToWindow("Inventory",Slot.selectedSlot.ItemID,1);
			}
		}
		Slot.ForceRefresh();
	}
}
