using UnityEngine;
using UnityEngine.UI;

public class DescriptionArea : MonoBehaviour {

	public Text itemName,price,equiped,score;
	public GameObject starPrefab;
	public Image[] bars;

	public GameObject toggableArea;

	void Start () {
		Slot.OnSelectedChange += OnChange;
	}

	public void OnChangingMenus() {
		Slot.selectedSlot = null;
		OnChange();
	}

	void OnChange() {
		if (Slot.selectedSlot != null && Slot.selectedSlot.Populated && gameObject.activeInHierarchy) {
			toggableArea.SetActive(true);
			itemName.text = Slot.selectedSlot.ItemName;
			price.text = "Price:\t" + Slot.selectedSlot.GetItemAttribute("Value");
			if (EquipmentSystem.sys.equipedItems.Contains(Slot.selectedSlot.CurrentItem)) {
				equiped.text = "Equiped: " + "<color=green>YES</color>";
 			} 
			else {
				equiped.text = "Equiped: " + "<color=red>NO</color>";
			}
			float barsValues = 0f;
			foreach(var bar in bars) {
				bar.GetComponentInParent<BarAttribute>().OnChange();
				barsValues += bar.fillAmount;
			}
			barsValues *= 20;
			score.text = "Score: " + barsValues.ToString() + "%";
		} 
		else {
			toggableArea.SetActive(false);
		}
	}
}
