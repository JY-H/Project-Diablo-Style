using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Container))]
public class ContainerDrawer : Editor {

	public override void OnInspectorGUI () {
		serializedObject.Update();
		(serializedObject.targetObject as Container).gameObject.SetActive(true);

		SerializedProperty isCraftWindow = serializedObject.FindProperty("isCraftWindow");
		SerializedProperty orderInWindow = serializedObject.FindProperty("orderInWindow");
		SerializedProperty visible = serializedObject.FindProperty("visible");
		SerializedProperty scale = serializedObject.FindProperty("scale");
		SerializedProperty isNormalContainer = serializedObject.FindProperty("isNormalContainer");

		EditorGUILayout.PropertyField(isCraftWindow);
		EditorGUILayout.PropertyField(isNormalContainer);
		if (isNormalContainer.boolValue)
			EditorGUILayout.PropertyField(orderInWindow);

		Transform tr = (serializedObject.targetObject as Container).transform;
		EditorGUILayout.PropertyField(visible);
		if (visible.boolValue) {
			if (scale.vector3Value != Vector3.zero) {
				if (tr.localScale != Vector3.zero)
					scale.vector3Value = tr.localScale;
				tr.localScale = scale.vector3Value;
			}
		}
		else {
			if (tr.localScale != Vector3.zero)
				scale.vector3Value = tr.localScale;
			tr.localScale = Vector3.zero;
		}

		serializedObject.ApplyModifiedProperties();
	}
}

[CustomEditor(typeof(InventoryWindow))]
public class InvWindowDrawer : Editor {
	
	public override void OnInspectorGUI () {
		serializedObject.Update();
		(serializedObject.targetObject as InventoryWindow).gameObject.SetActive(true);
		
		SerializedProperty windowName = serializedObject.FindProperty("windowName");
		SerializedProperty scale = serializedObject.FindProperty("scale");
		SerializedProperty visible = serializedObject.FindProperty("visible");

		EditorGUILayout.PropertyField(windowName);
		
		Transform tr = (serializedObject.targetObject as InventoryWindow).transform;
		EditorGUILayout.PropertyField(visible);
		if (visible.boolValue) {
			if (scale.vector3Value != Vector3.zero) {
				if (tr.localScale != Vector3.zero)
					scale.vector3Value = tr.localScale;
				tr.localScale = scale.vector3Value;
			}
		}
		else {
			if (tr.localScale != Vector3.zero)
				scale.vector3Value = tr.localScale;
			tr.localScale = Vector3.zero;
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}

[CustomPropertyDrawer(typeof(InitialItem))]
public class InitialItemDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
		var rect = position;
		GUI.Label(rect,"Initial item:");
		var manager = new SerializedObject(GameObject.FindObjectOfType<InventoryManager>());
		manager.Update();
		var serializedTemplates = manager.FindProperty("templates");
		List<string> templates = new List<string>();
		for (int i = 0;i < serializedTemplates.arraySize;i ++) {
			templates.Add(serializedTemplates.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);
		}
		var itemNames = new string[templates.Count + 1];
		for (int i = 0;i < templates.Count;i ++) {
			itemNames[i] = templates[i];
		}
		itemNames[templates.Count] = "none";

		var itemName = property.FindPropertyRelative("itemName");
		var selectedIndex = property.FindPropertyRelative("selectedIndex");
		var amount = property.FindPropertyRelative("amount");
		var size = property.FindPropertyRelative("currentSize");
		if(size.intValue < templates.Count)
			property.FindPropertyRelative ("selectedIndex").intValue ++;
		size.intValue = templates.Count;
		rect.x += 72;
		rect.width = 96;
		if (selectedIndex.intValue > itemNames.Length)
			selectedIndex.intValue = 0;
		selectedIndex.intValue = EditorGUI.Popup(rect,selectedIndex.intValue,itemNames);
		if (serializedTemplates.arraySize > selectedIndex.intValue) {
			var stackable = serializedTemplates.GetArrayElementAtIndex(selectedIndex.intValue).FindPropertyRelative("stackable").boolValue;
			if (stackable) {
				rect.x = rect.xMax;
				rect.width = 72;
				EditorGUI.PropertyField(rect,amount,GUIContent.none);
			}
		}
		if (itemNames.Length > selectedIndex.intValue)
			itemName.stringValue = itemNames[selectedIndex.intValue];
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		return 16;
	}
}

[CustomPropertyDrawer(typeof(SlotMask))]
public class SlotMaskDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		var rect = position;
		GUI.Label(rect,"Slot mask:");
		var manager = new SerializedObject(GameObject.FindObjectOfType<InventoryManager>());
		manager.Update();
		var mask = property.FindPropertyRelative("mask");
		var selectedIndex = property.FindPropertyRelative("selectedIndex");
		var serializedTypes = manager.FindProperty("types");
		string[] types;
		types = new string[serializedTypes.arraySize + 1];
		for (int i = 0;i < serializedTypes.arraySize;i ++) {
			types[i] = serializedTypes.GetArrayElementAtIndex(i).stringValue;
		}
		types[serializedTypes.arraySize] = "All";
		rect.x += 72;
		rect.width = 108;
		if (types.Length > selectedIndex.intValue) {
			selectedIndex.intValue = EditorGUI.Popup(rect,selectedIndex.intValue,types);
			mask.stringValue = types[selectedIndex.intValue];
		}
		else {
			selectedIndex.intValue = 0;
		}
	}
}


[CustomEditor( typeof( InventoryManager ) )]
public class InvManagerDrawer : Editor {

	public override void OnInspectorGUI () {
		serializedObject.Update();

		SerializedProperty dragAndDrop = serializedObject.FindProperty( "dragAndDrop" );
		SerializedProperty canStack = serializedObject.FindProperty( "canStack" );
		SerializedProperty canSplit = serializedObject.FindProperty( "canSplit" );
		SerializedProperty dragOnPlanes = serializedObject.FindProperty( "dragOnPlanes" );
		SerializedProperty splitKey = serializedObject.FindProperty( "splitKey" );
		SerializedProperty splitButton = serializedObject.FindProperty( "splitButton" );
		SerializedProperty useButton = serializedObject.FindProperty( "useButton" );

		EditorGUILayout.PropertyField( splitButton,true );
		EditorGUILayout.PropertyField( splitKey,true );
		EditorGUILayout.PropertyField( dragAndDrop,true );
		EditorGUILayout.PropertyField( canStack,true );
		EditorGUILayout.PropertyField( canSplit,true );
		EditorGUILayout.PropertyField( dragOnPlanes,true );
		EditorGUILayout.PropertyField( useButton,true );

		serializedObject.ApplyModifiedProperties();
	}
}

















