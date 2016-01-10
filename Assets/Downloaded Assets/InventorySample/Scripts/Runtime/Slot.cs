// This class holds the data and events that define a slot,like the current item,or the number of items,or events like dragging,dropping ......

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("Inventory/Container")]
public class Slot : Selectable,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler {

	// Initial item
	public InitialItem initialItem;

	// Craft sys
	[HideInInspector]
	public Container parentContainer;
	[HideInInspector]
	public List<CraftingRecipe> recipes;
	[HideInInspector]
	public Rect positionInCrafting;
	[HideInInspector]
	public bool receiver;

	// Sounds
	public AudioClip clickSound,beginDragSound,endDragSound;
	[Range(0,1)]
	public float soundVolume = 1.0f;

	// Item icon
	public float iconDimension = 2.5f;
	public bool preserveIconAspect = true;

    private Item _currentItem;

    // Keeping Track
    public static bool dragging;
    public static Item draggedItem;

    // Mask
	public SlotMask mask = new SlotMask() {mask = "All"};

    // Drag and drop system
    public static bool dragAndDrop, canStack, canSplit, dragOnPlanes, useEventSystem;
	public static KeyCode splitKey;
	public static PointerEventData.InputButton splitButton,useButton;

    private static RectTransform draggingPlane, draggedItemRect = null;
    private static Vector3 globalMousePosition = Vector3.zero;

	// Here the events that can happen on a slot, are defined
	public delegate void EventTemplate();
	public event EventTemplate OnChange;
	public static event EventTemplate OnInspectedChange;
	public static event EventTemplate OnEquipmentChange;
	public static event EventTemplate OnItemDrop;
	public static event EventTemplate OnSelectedChange;

	public static Slot inspectedSlot = null;
	public static Slot selectedSlot { get; set;}

    //Interfaces
	public bool Populated { get { return _currentItem == null ? false : true;}}
    public Item CurrentItem { get { return _currentItem == null ? null : _currentItem; } }
	public Sprite ItemIcon { get { return _currentItem == null ? null : _currentItem.Icon; } }
	public int ItemID { get { return _currentItem == null ? -1 : _currentItem.ID; } }
    public string ItemName { get { return _currentItem == null ? string.Empty : _currentItem.name; } }
	public string ItemDescription { get { return _currentItem == null ? "" : _currentItem.Description; } }
	public string ItemType { get { return _currentItem == null ? "" : _currentItem.Type; } }
	public bool ItemIsStackable { get { return _currentItem == null ? false : _currentItem.Stackable; } }
	public int MaxInStack { get { return _currentItem == null ? 0 : _currentItem.MaxInStack; }}
	public bool CanUseItem { get {return _currentItem == null ? false : _currentItem.CanBeUsed; } }
	public int CurrentInStack { 
		get { 
			return _currentItem == null ? 0 : _currentItem.CurrentInStack; 
		} 
		set {
			if ( value <= MaxInStack ) {
				if ( value < 1 && Populated ) {
					Destroy (_currentItem.gameObject);
					_currentItem = null;
				}
				else if( Populated ){
					_currentItem.CurrentInStack = value;
				}
				if (OnChange != null)
					OnChange();
			}
		}
	}

	// Initializing the slot ------------------------------------------------------------
	protected override void Start () {
		OnChange += OnPreviewChange;
	}


	public void UseItem() {
		if( !parentContainer.isCraftWindow && _currentItem != null ) {
			CurrentItem.Use();
			if (CanUseItem)
				CurrentInStack --;
		}
	}


	public float GetItemAttribute(string name) {
		float result = 0f;
		if (_currentItem != null)
			result = CurrentItem.GetAttribute(name);
		return result;
	}


	public Item AssignAsReceived { 
		set { 
			_currentItem = value; 
			if (_currentItem != null) {
				_currentItem.transform.SetParent(transform,false);
				_currentItem.transform.localPosition = Vector3.zero;
				_currentItem.transform.localScale = Vector3.one * iconDimension;
			}
			if (OnChange != null)
				OnChange();
			if (OnEquipmentChange != null)
				OnEquipmentChange();
		} 
	}


    public Item AssignItem { 
		set { 
			_currentItem = value; 
			if (_currentItem != null) {
				_currentItem.transform.SetParent(transform,false);
				_currentItem.transform.localPosition = Vector3.zero;
				_currentItem.transform.localScale = Vector3.one * iconDimension;
			}
			if (OnChange != null)
				OnChange();
		} 
	}


    public void DiscardItem (bool destroyItemObject) { 
			if (_currentItem != null) {
				if (destroyItemObject)
            		DestroyObject(_currentItem.gameObject);
				_currentItem = null;
			}
			if (OnChange != null)
				OnChange();
			if (OnEquipmentChange != null)
				OnEquipmentChange();
    }


	public void DiscardWithoutCheck () {
		if (_currentItem != null) {
			Destroy(_currentItem.gameObject);
			_currentItem = null;
		}
	}

	
	public Item ReplaceItem(Item newItem) { // This function will replace the current item only if the mask is the correct one,or it's set to "All"
		if (newItem != null) {
			Item replaced = newItem;
	        if (mask.mask == newItem.Type || mask.mask == "All") {
	            replaced = _currentItem;
	            _currentItem = newItem;
				_currentItem.transform.SetParent(transform,false);
				_currentItem.transform.localPosition = Vector3.zero;
				_currentItem.transform.localScale = Vector3.one * iconDimension;
	        } 
	       
			if (OnChange != null)
				OnChange();
			if (OnEquipmentChange != null)
				OnEquipmentChange();

	        return replaced;
		}
		return null;
    }


    // Drag and drop system (optional)
    public void OnBeginDrag (PointerEventData data) {
        if (dragAndDrop && _currentItem != null && !dragging) {
			// Check for splitting input
			if (dragAndDrop && canSplit &&  Input.GetKey(splitKey) && CurrentInStack > 1 && data.button == splitButton) {
				var initialAmount = CurrentInStack;
				CurrentInStack = Mathf.RoundToInt(CurrentInStack / 2f);
				var leftAmount = initialAmount - CurrentInStack;

				draggedItem = InventoryManager.manager.RetrieveItem(ItemID,leftAmount,this);
				dragging = true;
				draggingPlane = transform as RectTransform;
				draggedItemRect = draggedItem.transform as RectTransform;
				draggedItemRect.SetParent(GetComponentInParent<Canvas>().transform, false);
	
				if( parentContainer.isCraftWindow ) {
					parentContainer.CheckForRecipe(receiver);
				}

				if (OnChange != null)
					OnChange();
			
				return;	
			}
            draggedItem = _currentItem;
            dragging = true;
            draggingPlane = transform as RectTransform;
            draggedItemRect = _currentItem.transform as RectTransform;
            draggedItemRect.SetParent(GetComponentInParent<Canvas>().transform, false);

			_currentItem = null;

			if( parentContainer.isCraftWindow ) {
				parentContainer.CheckForRecipe(receiver);
			}

			if (OnChange != null)
				OnChange();
			if (OnEquipmentChange != null)
				OnEquipmentChange();
		}
	}


    public void OnDrag (PointerEventData data) {
        if (dragAndDrop && dragging) {
            if (dragOnPlanes && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
                draggingPlane = data.pointerEnter.transform as RectTransform;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out globalMousePosition)) {
				if (draggedItem != null) {
	                draggedItemRect.position = globalMousePosition;
	                draggedItemRect.rotation = draggingPlane.rotation;
				}
            }
        }      
    }

    
    public void OnEndDrag (PointerEventData atad) {
        if (dragAndDrop && draggedItem != null && dragging) {
            var checker = new CheckHandler(this,atad);
			Slot foundSlot = null;
			try {
				foundSlot = atad.pointerCurrentRaycast.gameObject.GetComponent<Slot>();
			} catch {}
			if( foundSlot != null && foundSlot.receiver && foundSlot.parentContainer.isCraftWindow ) {
				parentContainer.parentWindow.AddItem( Slot.draggedItem.ID,Slot.draggedItem.CurrentInStack );
				Destroy(Slot.draggedItem.gameObject);
				draggedItem = null;
				dragging = false;
			}
			else {
				if( atad.pointerCurrentRaycast.gameObject != null ) {
					if( foundSlot != null && !foundSlot.receiver) {
		            	checker.ComputeCheck();
					}
					else if ( foundSlot != null ){
						this.AssignItem = Slot.draggedItem;
					}
					else {
						GetComponentInParent<InventoryWindow>().AddItem(Slot.draggedItem.ID,Slot.draggedItem.CurrentInStack);
						Destroy(Slot.draggedItem.gameObject);
					}
				}
				else {
					checker.ComputeCheck();
				}

				draggedItem = null;
				dragging = false;

				try {
					if( foundSlot != null && foundSlot.parentContainer.isCraftWindow && !foundSlot.receiver ) {
						foundSlot.parentContainer.CheckForRecipe( false );
					}
				} catch {}
			
				if (OnChange != null)
					OnChange();
				if (OnEquipmentChange != null)
					OnEquipmentChange();
			}
        }   
    }


	public void OnPointerClick (PointerEventData data) {
		selectedSlot = this;
		if (data.button == useButton && CanUseItem) {
			UseItem();
		}
		if (clickSound)
			AudioSource.PlayClipAtPoint(clickSound,Vector3.zero,soundVolume);
		if (OnChange != null)
			OnChange();
		if (OnSelectedChange != null) 
			OnSelectedChange();
	}


	public override void OnPointerEnter (PointerEventData data) {
		base.OnPointerEnter(data);
		inspectedSlot = this;
		if (OnInspectedChange != null)
			OnInspectedChange();
	}


	public override void OnPointerExit (PointerEventData data) {
		base.OnPointerExit(data);
		inspectedSlot = null;
		if (OnInspectedChange != null)
			OnInspectedChange();
	}


	// Crafting and other events
	public static void CheckDrop () {
		if (OnItemDrop != null) {
			OnItemDrop();
		}
		else {
			draggedItem.AutoDestroy(0f);
		}
	}


	public static void ForceRefresh() {
		if (OnSelectedChange != null)
			OnSelectedChange();
		if (OnInspectedChange != null)
			OnInspectedChange();
		if (OnItemDrop != null)
			OnItemDrop();
		if (OnEquipmentChange != null)
			OnEquipmentChange();
	}


	public bool CheckRecipe (int recipeIndex) {
		if ((recipes[recipeIndex].itemName == ItemName && recipes[recipeIndex].amount <= CurrentInStack) || receiver)
			return true;
		return false;
	}

	void OnPreviewChange() {
		if( receiver ) {
			parentContainer.previewExists = Populated;
		}
	}
}









