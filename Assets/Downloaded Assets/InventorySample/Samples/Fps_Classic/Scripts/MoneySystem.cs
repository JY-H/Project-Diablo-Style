using UnityEngine;
using UnityEngine.UI;

public class MoneySystem : MonoBehaviour {

	public Text moneyDisplayer;
	public static MoneySystem sys;

	private float _money;
	public float money { get { return _money; } set { _money = value; OnChange(); } }

	void Start () {
		if (sys == null)
			sys = this;
		else if (sys != this)
			Destroy(gameObject);
		OnChange += RefreshMoney;
	}

	void RefreshMoney () {
		moneyDisplayer.text = money.ToString() + "$";
	}

	public event Slot.EventTemplate OnChange;
}
