using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("Inventory/Item")]
public class Item : Button {

	// Disabling interactibility
	CanvasGroup myCanvasGroup;
	public ButtonClickedEvent aha;

    // Keeping Track
    private bool initiated = false;

    // Usability
    private bool _canBeUsed;

    // General fields
    private int _id;
    private string _name, _description, _type;
    private Sprite _icon;

    private List<AttributeAssigner> attributes = new List<AttributeAssigner>();
	private OnUseEvent _onUseEvent;

    // For stacking / splitting
    private bool _stackable;
    private int  _maxInStack;

    // General properties
    public int ID { get { return _id; } }
    public string Name { get { return _name; } }
    public string Description { get { return _description; } }
    public string Type { get { return _type; } }
    public bool Stackable { get { return _stackable; } }
    public Sprite Icon { get { return _icon; } }
    public bool CanBeUsed { get { return _canBeUsed; } }

	public int CurrentInStack { get; set;}
    public int MaxInStack { get { return _maxInStack; } }

    // Accesing the attributes
    public float GetAttribute(string name) {
		float val = 0f;
        foreach (var attribute in attributes) {
            if (attribute.attributeName == name)
                val = attribute.value;
        }
        return val;
    }

    public void SetAttribute(string name, float value) { 
        foreach (var attribute in attributes) {
            if (attribute.attributeName == name)
                attribute.value = value;
        }
    }

	public void AutoDestroy(float time) {
		Destroy (gameObject,time);
	}

    // Using this item
    public void Use() {
        if (_canBeUsed) {
			_onUseEvent.TriggerEvent();
        }
    }

    public void Init(ItemTemplate template) {    // This function must be called once,for initiating the item
        if (!initiated) {
            _id = template.id;
            _name = template.name;
            _icon = template.icon;
            _stackable = template.stackable;
            _type = template.type;
            _description = template.description;
            _canBeUsed = template.canBeUsed;
			_onUseEvent = template.onUseEvent;
			foreach(var atrb in template.assigners) {
				attributes.Add(atrb);
			}
            initiated = true;

            if (_stackable) {
                this._maxInStack = template.maxInStack;
			}
			this.CurrentInStack = 1;

            // Adding the components
            myCanvasGroup = gameObject.AddComponent<CanvasGroup>();
			myCanvasGroup.blocksRaycasts = false;
			myCanvasGroup.interactable = false;
            gameObject.AddComponent<RectTransform>();
            gameObject.AddComponent<CanvasRenderer>();
            Image image = gameObject.AddComponent<Image>();
            image.sprite = _icon;
        }
    }

	protected override void OnEnable() {
		if (myCanvasGroup != null) {
	 		myCanvasGroup.blocksRaycasts = false;
			myCanvasGroup.interactable = false;
		}
	}
}
