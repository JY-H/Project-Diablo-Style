using UnityEngine;
using UnityEngine.UI;

public class AddItem : MonoBehaviour {

	public InputField itemName;
	public string windowName;

	public void Add() {
		InventoryManager.manager.AddItemToWindow( windowName,InventoryManager.manager.GetItemID(itemName.text),1 );
	}
}
