// This one updates a text based on the parent slot item's info, it can read the amount of items in a slot,or the name of the item.

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Inventory/Samples/Displayer")]
public class Displayer : MonoBehaviour {

	public Text displayer;
	public enum DisplayInfo { Name,Amount }
	public DisplayInfo info;

	private Slot parentSlot;

    void Start () {
		parentSlot = GetComponentInParent<Slot>();
		parentSlot.OnChange += OnChange;
	}

	void OnChange () {
		if (info == DisplayInfo.Name)
			displayer.text = parentSlot.ItemName;
		else if (info == DisplayInfo.Amount) {
			displayer.text = parentSlot.CurrentInStack.ToString();
			if (parentSlot.CurrentInStack < 2)
				displayer.text = "";
		}
	}
}
