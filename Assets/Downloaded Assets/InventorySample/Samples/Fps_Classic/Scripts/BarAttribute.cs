// This script updates an image based on an attribute,like:  "Damage", "Fire Rate"........

using UnityEngine;
using UnityEngine.UI;

public class BarAttribute : MonoBehaviour {

	public Image targetBarImage;
	public string targetAttribute;

	public void OnChange() {
		targetBarImage.fillAmount = Slot.selectedSlot.GetItemAttribute(targetAttribute) / 100f;
	}
}
