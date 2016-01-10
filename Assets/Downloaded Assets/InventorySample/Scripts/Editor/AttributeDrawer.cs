using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Attribute))]
public class AttributeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { 
		Rect rect = position;

		SerializedProperty name = property.FindPropertyRelative("name");
		SerializedProperty id = property.FindPropertyRelative("id");

		// Name
		EditorGUI.PropertyField(rect,name,GUIContent.none);

		// ID
		rect.x = rect.xMax + 10;
		id.intValue = ItemManagerWindow.drawnAttribute;
		GUI.Label(rect,id.intValue.ToString());
	}
	
	public override float GetPropertyHeight(SerializedProperty property,GUIContent label) {
		return 16;
	}
}

[CustomPropertyDrawer(typeof(AttributeAssigner))]
public class AttributeAssignerDrawer : PropertyDrawer {
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { 
		Rect rect = position;	
		SerializedProperty value = property.FindPropertyRelative("value");
		SerializedProperty attributeName = property.FindPropertyRelative("attributeName");
		SerializedProperty selectedIndex = property.FindPropertyRelative("selectedIndexAttribute");

		// Dropdown
		selectedIndex.intValue = EditorGUI.Popup(rect,"",selectedIndex.intValue,ItemDatabaseWindow.attributes);
		if (ItemDatabaseWindow.attributes.Length >= selectedIndex.intValue + 1)							// Making sure that when deleting an attribute,we don't receive an error
			attributeName.stringValue = ItemDatabaseWindow.attributes[selectedIndex.intValue]; 
		else {
			selectedIndex.intValue = 0;
			attributeName.stringValue = ItemDatabaseWindow.attributes[0]; 
		}

		// Value 
		rect.x = rect.xMax;
		rect.width = 48;
		EditorGUI.PropertyField(rect,value,GUIContent.none);
	}
	
	public override float GetPropertyHeight(SerializedProperty property,GUIContent label) {
		return 16;
	}
}
