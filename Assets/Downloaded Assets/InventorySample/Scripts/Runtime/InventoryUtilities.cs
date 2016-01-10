// In this file are present all the scripts that provide functionalities to the item database,crafting manager,and the system itself.

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Reflection;

[System.Serializable]
public class OnUseEvent {           // For when the item it's used
    public GameObject target;
    public string eventName;
	public MethodInfo methodInfo;
	public bool Bool;
	public float Float;
	public string String;
	public int Int;
	public Vector2 Vector2Val;
	public Vector3 Vector3Val;
	public Rect rectVal;
	public Color color;
	public UnityEngine.Object Uobject;
	public string valueType;
	public string targetComponent;
	public AudioClip useSound;

	public int selectedIndex;

    public void TriggerEvent() {
		if (eventName != "none") {
			object temp = new object();
			switch(valueType) {
			case "Boolean" : {
				temp = Bool;
				break;
			}
			case "String" : {
				temp = String;
				break;
			}
			case "Single" : {
				temp = Float;
				break;
			}
			case "Int32" : {
				temp = Int;
				break;
			}
			case "Vector2" : {
				temp = Vector2Val;
				break;
			}
			case "Vector3" : {
				temp = Vector3Val;
				break;
			}
			case "Rect" : {
				temp = rectVal;
				break;
			}
			case "Color" : {
				temp = color;
				break;
			}
			default : {
				temp = Uobject;
				break;
			}
			}
			if (targetComponent != "GameObject") {
				var component = target.GetComponent(targetComponent);
				var mthds = component.GetType().GetMethods();
				foreach (var mthd in mthds) {
					if ((mthd.Name == eventName && mthd.GetParameters().Length < 2 && !mthd.IsGenericMethod)) {
						methodInfo = mthd;
						break;
					}
				}
				methodInfo.Invoke(component,new object[]{temp});
			}
			else {
				var mthds = typeof(GameObject).GetMethods();
				foreach (var mthd in mthds) {
					if ((mthd.Name == eventName && mthd.GetParameters().Length < 2 && !mthd.IsGenericMethod)) {
						methodInfo = mthd;
						break;
					}
				}
				methodInfo.Invoke(target,new object[]{temp});
			}
			if (useSound != null)
				AudioSource.PlayClipAtPoint(useSound,Vector3.zero);
	    }
	}
}

[System.Serializable]
public class Attribute {
    public string name;
    public float value;
	public int id;
}

[System.Serializable]
public class AttributeAssigner {
	public string attributeName;
	public int selectedIndexAttribute;
	public float value;
}

[System.Serializable]
public class ItemTemplate {
    public int id;
    public string name;
    public Sprite icon;
    public string type;
    public string description;
	public bool foldout;

    public bool stackable;
    public int maxInStack;

    public bool canBeUsed;
	public bool locked;
	
	public List<AttributeAssigner> assigners;
	public OnUseEvent onUseEvent;

	public int selectedIndexType;
	public int goodIndexType;

	public ItemTemplate(string name) {
		this.name = name;
	}

	public ItemTemplate() {}
}


// This class can be used to calculate the outcome of an item drop
public class CheckHandler {
    private Slot target, initial;
    private PointerEventData data;

    public CheckHandler(Slot initialSlot, PointerEventData data) {
		if (data.pointerCurrentRaycast.gameObject != null)
        	this.target = data.pointerCurrentRaycast.gameObject.GetComponent<Slot>();
        this.initial = initialSlot;
        this.data = data;
    }

    public void ComputeCheck() {
		// If you dropped an item on a slot
        if (target != null) { 
			if (target.CurrentItem != null && (target.mask.mask == Slot.draggedItem.Type || target.mask.mask == "All")) {
				// If "Can Stack" option is enabled
				if (Slot.canStack) {
					// If can you stack on this slot
					if ( target.ItemIsStackable && target.CurrentInStack < target.MaxInStack && target.ItemID == Slot.draggedItem.ID) {
						var amountToAdd = Slot.draggedItem.CurrentInStack;
						var sum = amountToAdd + target.CurrentInStack;
						if (sum <= target.MaxInStack) {
							Slot.draggedItem.AutoDestroy(0f);
							target.CurrentInStack = sum;
						}
						else {
							if (initial.CurrentItem != null) {
								Slot.draggedItem.AutoDestroy(0f);
								initial.CurrentInStack += amountToAdd;
							}
							else {
								initial.AssignItem = Slot.draggedItem;
							}
						}
					}
					else if (target.ItemID != Slot.draggedItem.ID) {
						if( !initial.receiver ) {
							if (target != initial) {
								initial.AssignItem = target.ReplaceItem(Slot.draggedItem);
							} 
							else 
								initial.AssignItem = Slot.draggedItem;
						}
						else {
							target.parentContainer.parentWindow.AddItem( Slot.draggedItem.ID,Slot.draggedItem.CurrentInStack );
							Slot.draggedItem.AutoDestroy( 0f );
						}
					}
					else if( initial.receiver && initial.parentContainer.isCraftWindow ) {
						target.parentContainer.parentWindow.AddItem( Slot.draggedItem.ID,Slot.draggedItem.CurrentInStack );
						Slot.draggedItem.AutoDestroy( 0f );
					}
					else {
						if (initial.CurrentItem != null) {
							initial.CurrentInStack += Slot.draggedItem.CurrentInStack;
						}
						else {
							initial.AssignItem = Slot.draggedItem;
						}
					}
				} 
				else {
					// If you didn't drop it on the same slot
					if (target != initial) {
						initial.AssignItem = target.ReplaceItem(Slot.draggedItem);
					} 
					else 
						initial.AssignItem = Slot.draggedItem;
				}
			}
			else if (target.mask.mask == Slot.draggedItem.Type || target.mask.mask == "All") {
				target.AssignItem = Slot.draggedItem;
			}
			else if (target.mask.mask != Slot.draggedItem.Type || target.mask.mask != "All") {
				initial.AssignItem = Slot.draggedItem;
			}
        }
		// Or if you dropped it on another ui element
        else if (data.pointerCurrentRaycast.gameObject != null) { 
			if (initial.CurrentItem == null) {
				initial.AssignItem = Slot.draggedItem;
			}
			else {
				initial.CurrentInStack += Slot.draggedItem.CurrentInStack;
				Slot.draggedItem.AutoDestroy(0f);
			}
        }
		// Else just drop the item (destroy it)
		else {
			Slot.CheckDrop();
		}

		Slot.draggedItem = null;
		Slot.dragging = false;
    }
}


// This class handles the adding / removing / checking that is required in an Inventory window.
public class ItemManagementHandler {

	private Container[] containers;

    public ItemManagementHandler(Container[] containers) {
     		this.containers = containers;
    }

    public void AddItem(int itemID, int amount) {
        int remainder = amount;

		foreach(var c in containers) {
			if(!c.isCraftWindow) {
				List<Slot> slots = c.GetSlots;
		        foreach (var slot in slots) {
		            if (slot.CurrentItem == null && (slot.mask.mask == "All" || slot.mask.mask == InventoryManager.manager.GetItemTemplate(itemID).type)) {
		                slot.AssignItem = InventoryManager.manager.RetrieveItem(itemID, remainder, slot);
		                remainder -= slot.CurrentInStack;
		            }
					else if (slot.ItemID == itemID && slot.ItemIsStackable && slot.CurrentInStack < slot.MaxInStack && Slot.canStack && slot.mask.mask == "All"){
		                int newAmount = slot.CurrentInStack + remainder;
		                int maxInStack = slot.MaxInStack;

		                if (newAmount <= maxInStack) {
		                    slot.CurrentInStack = newAmount;
		                    remainder = 0;
		                }
		                else {
		                    slot.CurrentInStack = maxInStack;
		                    remainder = newAmount - maxInStack;
		                }
		            }
					if (remainder < 1)
						return;
				}
			}
		}
	}
	
	public void RemoveItem(int itemID, int amount) {
		int remainder = amount;

		foreach(var c in containers) {
			if(!c.isCraftWindow) {
				var slots = c.GetSlots;

		        for (int i = slots.Count - 1; i >= 0; i --) {
		            if (slots[i].CurrentItem != null) {
						var current = slots[i];
		                if (current.ItemID == itemID) {
		                    int initialInStack = current.CurrentInStack;
		                    current.CurrentInStack -= remainder;
		                    remainder -= initialInStack;
		                    if (remainder < 1)
		                        return;
		                }
		            }
		        }
			}
		}
    }

    public void RemoveAll() {
		foreach(var c in containers) {
			if(!c.isCraftWindow) {
				var slots = c.GetSlots;
		        foreach (var slot in slots)
					slot.DiscardItem(true);
			}
		}
    }

    public bool Contains(int itemID) {
		bool contains = false;
		foreach(var c in containers) {
			if(!c.isCraftWindow) {
				var slots = c.GetSlots;
		        foreach (var slot in slots) {
		            if (slot.ItemID == itemID) {
		                contains = true;
		                break;
		            }
		        }
			}
		}

        return contains;
    }

    public int HowManyContains(int itemID) {
        int amount = 0;

		foreach(var c in containers) {
			if(!c.isCraftWindow) {
				var slots = c.GetSlots;
		        foreach (var slot in slots) {
		            if (slot.ItemID == itemID) {
		                amount += slot.CurrentInStack;
		                break;
		            }
		        }
			}
		}

        return amount;
    }
}


// ------------------------------- HERE ARE THE DIFFERENT DRAWERS AND MENUS OF THE SYSTEM ----------------------
[System.Serializable]
public class InitialItem {
	public string itemName;
	public int amount,selectedIndex,currentSize;
}

[System.Serializable]
public struct SlotMask {
	public string mask;
	public int selectedIndex;
}

[System.Serializable]
public class CraftingRecipe {
	public string itemName;
	public int amount;
}

public static class InventoryInstantiater {

	[MenuItem("GameObject/Inventory/Slot")]
	public static void AddSlot () {
		var go = new GameObject("Slot",new Type[] {typeof(RectTransform),typeof(Image),typeof(Slot)});
		go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Slot");
		var canvas = GameObject.FindObjectOfType<Canvas>();
		if (canvas != null)
			go.transform.SetParent(canvas.transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}

	[MenuItem("GameObject/Inventory/Container")]
	public static void AddContainer () {
		var go = new GameObject("Container",new Type[] {typeof(Container),typeof(Image)});
		go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Container");
		var canvas = GameObject.FindObjectOfType<Canvas>();
		if (canvas != null)
			go.transform.SetParent(canvas.transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}

	[MenuItem("GameObject/Inventory/InventoryWindow")]
	public static void AddInventoryWindow () {
		var go = new GameObject("InventoryWindow",new Type[] {typeof(InventoryWindow)});
		var canvas = GameObject.FindObjectOfType<Canvas>();
		if (canvas != null)
			go.transform.SetParent(canvas.transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.GetComponent<InventoryWindow>().windowName = "Inventory";
	}
}








