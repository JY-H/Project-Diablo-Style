using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("Inventory/InventoryManager")]
public class InventoryManager : MonoBehaviour {

    // Making sure that there is only one instance of this class in the scene
    public static InventoryManager manager;        

    // Options
    public bool dragAndDrop, canStack, canSplit, dragOnPlanes, useEventSystem;
	public KeyCode splitKey;
	public PointerEventData.InputButton splitButton,useButton;

    // Database
    public List<ItemTemplate> templates = new List<ItemTemplate>();
    public List<string> types = new List<string>();
    public List<Attribute> attributes = new List<Attribute>();

	// Crafting
	public Container currentInspectedContainer;
	public float spacingCoefficient;
	public Vector2 layoutOffset;
	public float sizeCoefficient;

    // Registered windows , active in the scene
	public InventoryWindow[] windows;

    // Initializing
    void Awake() {
        if (manager == null)
            manager = this;
        else if (manager != this)
            Destroy(gameObject);

		Initialize();
    }


    private void Initialize() {
        // Slot initializing
        Slot.dragAndDrop = dragAndDrop;
        Slot.canSplit = canSplit;
        Slot.canStack = canStack;
        Slot.dragOnPlanes = dragOnPlanes;
        Slot.useEventSystem = useEventSystem;
		if (dragAndDrop && canStack) {
			Slot.splitKey = splitKey;
			Slot.splitButton = splitButton;
		}
		Slot.useButton = useButton;
		windows = FindObjectsOfType<InventoryWindow>();
    }


    // Interfaces
    public void AddItemToWindow(string windowName,int itemID,int amount) {
        foreach (var window in windows)
            if (window.windowName == windowName)
                window.AddItem(itemID, amount);
    }


	// This method adds an item to a specific slot,it's usually not called directly.
    public Item RetrieveItem(int id,int amount,Slot targetSlot) {
        Item itemToAdd = null;

        foreach (var template in templates) {
            if (template.id == id) {
                GameObject newItem = new GameObject(template.name);
                newItem.transform.SetParent(targetSlot.transform,false);
                itemToAdd = newItem.AddComponent<Item>();
				itemToAdd.transform.localScale = Vector3.one * targetSlot.iconDimension;
                itemToAdd.Init(template);
				itemToAdd.GetComponent<Image>().preserveAspect = targetSlot.preserveIconAspect;
                itemToAdd.CurrentInStack = amount;
				break;
            }
        }

        return itemToAdd;
    }


	// This method adds an item to a specific slot,it's usually not called directly.
	public Item RetrieveItem(string name,int amount,Slot targetSlot) {
		Item itemToAdd = null;
		
		foreach (var template in templates) {
			if (template.name == name) {
				GameObject newItem = new GameObject(template.name);
				newItem.transform.SetParent(targetSlot.transform,false);
				itemToAdd = newItem.AddComponent<Item>();
				itemToAdd.Init(template);
				itemToAdd.GetComponent<Image>().preserveAspect = targetSlot.preserveIconAspect;
				itemToAdd.CurrentInStack = amount;
				itemToAdd.GetComponent<Transform>().localScale = Vector3.one * targetSlot.iconDimension;
				break;
			}
		}
		
		return itemToAdd;
	}


	// Item info ------------------------------------------------------------------------------------------
	public ItemTemplate GetItemTemplate(int id) {
		foreach(var template in templates) {
			if (template.id == id)
				return template;
		}
		return new ItemTemplate("Empty");
	}


	public int GetAttributeID (string name) {
		foreach (var atrb in attributes) {
			if (atrb.name == name) {
				return atrb.id;
			}
		}
		return -1;
	}


	public string GetItemName (int id) {
		foreach(var t in templates) {
			if (t.id == id)
				return t.name;
		}
		return "";
	}


	public int GetItemID (string name) {
		foreach(var t in templates) {
			if (t.name == name)
				return t.id;
		}
		return -1;
	}


	public bool ItemIsStackable( string itemName ) {
		foreach( var template in templates ) {
			if( template.name == itemName ) {
				if(template.stackable) {
					return true;
				}
			}
		}
		return false;
	}
	// ---------------------------------------------------------------------------------------------------------------------

}









