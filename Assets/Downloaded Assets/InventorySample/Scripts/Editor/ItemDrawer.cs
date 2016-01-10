using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ItemTemplate))]
public class ItemDrawer : PropertyDrawer {
	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		// Serializing the properties
		SerializedProperty itemID = property.FindPropertyRelative("id");
		SerializedProperty itemName = property.FindPropertyRelative("name");
		SerializedProperty itemIcon = property.FindPropertyRelative("icon");
		SerializedProperty itemType = property.FindPropertyRelative("type");
		SerializedProperty itemDescription = property.FindPropertyRelative("description");
		SerializedProperty stackable = property.FindPropertyRelative("stackable");
		SerializedProperty maxInStack = property.FindPropertyRelative("maxInStack");
		SerializedProperty canBeUsed = property.FindPropertyRelative("canBeUsed");     
		SerializedProperty selectedIndex = property.FindPropertyRelative("selectedIndexType");
		SerializedProperty onUseEvent = property.FindPropertyRelative("onUseEvent");
		SerializedProperty locked = property.FindPropertyRelative("locked");

		Rect rect = position;
		GUIStyle style = new GUIStyle();
		style.richText = true;
		GUI.Box(new Rect(0,position.y + 88,Screen.width * 4f,1),"");

		// ID
		rect.y -= 16;
		rect.x += 8;
		GUI.Label(rect,"<B>ID</B>",style);
		rect.x -= 8;
		rect.y += 16;
		rect.width = 32;
		rect.height = 24;
		GUI.Box(rect,ItemDatabaseWindow.drawnItem.ToString());
		itemID.intValue = ItemDatabaseWindow.drawnItem;

		// Name
		rect.y -= 16;
		rect.x += 48;
		GUI.Label(rect,"<B>Name</B>",style);
		rect.y += 16;
		rect.width = 200;
		EditorGUI.PropertyField(rect,itemName,GUIContent.none);

		// Description
		rect.width = 200;
		rect.height = 48;
		rect.y = rect.yMax - 8;
		rect.y -= 16;
		GUI.Label(rect,"<B>Description</B>",style);
		rect.y += 16;
		EditorGUI.BeginProperty(rect,GUIContent.none,itemDescription);
		itemDescription.stringValue = EditorGUI.TextArea(rect,itemDescription.stringValue);
		EditorGUI.EndProperty();

		// Icon
		rect.y -= 40;
		rect.x = rect.xMax + 10;
		rect.width = 192;
		rect.height = 16;
		rect.y -= 18;
		GUI.Label(rect,"<B>Icon</B>",style);
		rect.y += 18;
		EditorGUI.PropertyField(rect,itemIcon,GUIContent.none);

		// Type
		rect.y += 22;
		GUI.Label(rect,"<B>Type</B>",style);
		rect.y += 18;
		selectedIndex.intValue = EditorGUI.Popup(rect,selectedIndex.intValue,ItemDatabaseWindow.types);
		if (ItemDatabaseWindow.types != null && ItemDatabaseWindow.types.Length != 0 && selectedIndex.intValue <= ItemDatabaseWindow.types.Length - 1)
			itemType.stringValue = ItemDatabaseWindow.types[selectedIndex.intValue];
		else if ((ItemDatabaseWindow.types != null && ItemDatabaseWindow.types.Length != 0 && selectedIndex.intValue > ItemDatabaseWindow.types.Length - 1))
			selectedIndex.intValue = 0;

		// Stacking
		rect.y = rect.yMax + 8;
		GUI.Label(rect,"<B>Stackable?</B>",style);
		rect.x += 72;
		rect.width = 12;
		EditorGUI.PropertyField(rect,stackable,GUIContent.none);
		rect.x += 16;
		rect.width = 104;
		if (stackable.boolValue) {
			EditorGUI.PropertyField(rect,maxInStack,GUIContent.none);
			if (maxInStack.intValue < 1)
				maxInStack.intValue = 1;
		}
		else {
			stackable.intValue = 0;
		}

		// Can be used?
		rect.y -= 64;
		rect.x += 128;
		GUI.Label(rect,"<B>Consumable?</B>",style);
		rect.x += 96;
		float width = rect.width;
		float height = rect.height;
		rect.width = 12;
		rect.height = 12;
		EditorGUI.PropertyField(rect,canBeUsed,GUIContent.none);
		rect.width = width;
		rect.height = height;
		rect.x -= 64;

		if (canBeUsed.boolValue) {
			// On use event
			rect.x = rect.xMax;
			rect.width = 144;
			rect.y -= 16;
			GUI.Label(rect,"<B>On use event</B>",style);
			rect.y += 16;
			EditorGUI.PropertyField(rect,onUseEvent,GUIContent.none,true);
		}

		// Attributes
		rect.x = rect.xMax + 10;
		rect.y += 32;
		if (GUI.Button(rect,"Edit Attributes")) {
			ItemDatabaseWindow.selectionIndex = ItemDatabaseWindow.drawnItem;
		}

		// Deleting
		rect = position;
		rect.y += 32;
		rect.width = 32;
		rect.height = 32;
		GUI.backgroundColor = Color.red;
		if (GUI.Button(rect,"X") && ItemDatabaseWindow.drawnItem != ItemDatabaseWindow.lockedItem) {
			ItemDatabaseWindow.DeleteItemAtIndex(ItemDatabaseWindow.drawnItem);
			return;
		}
		GUI.backgroundColor = Color.white;

		// Locking
		rect.x = rect.xMax + 512;
		rect.width = 12;
		rect.height = 12;
		rect.x -= 32;
		GUI.Label(rect,"<B>LOCK</B>",style);
		rect.x += 32;
		EditorGUI.PropertyField(rect,locked,GUIContent.none);
		if (locked.boolValue) {
			ItemDatabaseWindow.lockedItem = ItemDatabaseWindow.drawnItem;
		}
		else {
			ItemDatabaseWindow.lockedItem = -1;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label) {
		return 64;
	}
}





