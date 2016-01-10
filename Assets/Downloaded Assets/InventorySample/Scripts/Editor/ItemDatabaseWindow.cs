using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ItemDatabaseWindow : EditorWindow {

	SerializedObject manager;
	static SerializedProperty itemList;

	// Drawing
	delegate void DrawEditor();
	DrawEditor drawFunction;
	Vector2 scrollPosition;
	float inspectedItemPosition;
	bool shouldRepaint = true;

	public static int drawnItem;
	public static string[] types;
	public static string[] attributes;
	public static int lockedItem;
	public static int selectionIndex;

	[MenuItem ("Inventory/Item Database")]
	static void Init () {
		var window = (ItemDatabaseWindow)EditorWindow.GetWindow(typeof(ItemDatabaseWindow));
		window.Repaint();
	}

	void  OnGUI() {
		if (manager != null)
			manager.Update();
		drawFunction();
		if (manager != null) {
			shouldRepaint = EditorGUILayout.Toggle("Repaint every frame?",shouldRepaint);
			DrawSelectionInspector();
			manager.ApplyModifiedProperties();
		}
		var temp = Random.Range(0,2);
		if (shouldRepaint && temp == 0)
			Repaint ();
	}

	void OnEnable() {
		Initialize();
	}

	void Initialize() {
		if (foundManager != null) {
			manager = new SerializedObject(foundManager);
			itemList = manager.FindProperty("templates");
			drawFunction = DrawDatabase;
		} 
		else {
			drawFunction = DrawWarningMessage;
		}
	}

	InventoryManager foundManager { get { var manag = FindObjectOfType<InventoryManager>(); return manag == null ? null : manag; } }

	void DrawWarningMessage() {
		EditorGUILayout.LabelField(new GUIContent("No Inventory manager present in the scene,please set it up"));
		if (GUILayout.Button("Set it up!")) {
			InstallInventory();
		}
	}

	private void InstallInventory () {
		var managerInstance = new GameObject("InventoryManager");
		var managerComponent = managerInstance.AddComponent<InventoryManager>();
		manager = new SerializedObject(managerComponent);
		itemList = manager.FindProperty("templates");
		drawFunction = DrawDatabase;
	}

	void DrawDatabase() {
		// Getting the types
		var stypes = manager.FindProperty("types");
		var sattributes = manager.FindProperty("attributes");
		types = new string[stypes.arraySize];
		attributes = new string[sattributes.arraySize];
		for (int i = 0;i < stypes.arraySize;i ++) {
			types[i] = stypes.GetArrayElementAtIndex(i).stringValue;
		}
		for (int i = 0;i < sattributes.arraySize;i ++) {
			attributes[i] = sattributes.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
		}

		// Menu
		if (GUI.Button(new Rect(0,96,128,32),"Add Item")) {
			itemList.arraySize ++;
		}
		if (GUI.Button(new Rect(144,96,128,32),"Remove All")) {
			itemList.arraySize = 0;
		}
		GUI.Box(new Rect(0,128,Screen.width * 0.75f,1),"");
		GUI.BeginGroup(new Rect(0,128,Screen.width * 0.75f,Screen.height - 128));
		float scrollAmount = (Screen.height - 128 - itemList.arraySize * 96) > 0 ? 0f : (Mathf.Abs(Screen.height - 128 - itemList.arraySize * 96 - 32));
		Rect databaseRect = new Rect(0,0,Screen.width * 0.75f,Screen.height - 128 + scrollAmount);
		Rect viewRect = new Rect(0,0,Screen.width * 0.75f,Screen.height - 128);
		scrollPosition = GUI.BeginScrollView(viewRect,scrollPosition,databaseRect,false,true);

		Rect itemRect = new Rect(0,20,Screen.width * 0.75f,96);
		int indexToBreak = itemList.arraySize;
		for (int i = 0; i < itemList.arraySize; i++) {
			if (lockedItem != -1) {
				if (i == lockedItem) {
					drawnItem = i;
					EditorGUI.PropertyField(itemRect,itemList.GetArrayElementAtIndex(i));
					indexToBreak = i + 10;
					inspectedItemPosition = itemRect.y;
				}
				else if (i == indexToBreak) {
					break;
				}
				else {
					GUI.Label(itemRect,itemList.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);
				}
				itemRect.y += 96;
			}
			else {
				if ((itemRect.Contains(Event.current.mousePosition) && (Event.current.mousePosition.y - scrollPosition.y) > 0) || inspectedItemPosition == itemRect.y) {
					drawnItem = i;
					EditorGUI.PropertyField(itemRect,itemList.GetArrayElementAtIndex(i));
					indexToBreak = i + 10;
					inspectedItemPosition = itemRect.y;
				}
				else if (i == indexToBreak) {
					break;
				}
				else {
					GUI.Label(itemRect,itemList.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);
				}
				itemRect.y += 96;
			}
		}
		GUI.EndScrollView();
		GUI.EndGroup();
	}

	void DrawSelectionInspector() {
		if (itemList.arraySize > selectionIndex) {
			var selectedItem = itemList.GetArrayElementAtIndex(selectionIndex);
			var attributes = selectedItem.FindPropertyRelative("assigners");
			GUI.BeginGroup(new Rect(Screen.width * 0.77f,128,Screen.width * 0.23f,Screen.height - 128));
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			style.stretchHeight = true;
			style.stretchWidth = true;
			style.richText = true;
			GUILayout.Label("Attributes of:  \n" + "<color=green>" +selectedItem.FindPropertyRelative("name").stringValue + "</color>",style);
			Rect rect = new Rect(16,96,128,16);

			for (int i = 0;i < attributes.arraySize;i ++) {
				EditorGUI.PropertyField(rect,attributes.GetArrayElementAtIndex(i));
				rect.y += 16;
			}
			rect.y += 16;
			if(GUI.Button(rect,"Add Attribute")) {
				attributes.arraySize ++;
			}
			rect.x = rect.xMax;
			if (GUI.Button(rect,"Remove Attribute")) {
				attributes.arraySize --;
			}

			GUI.EndGroup();
		}
	}

	public static void DeleteItemAtIndex (int index) {
		itemList.DeleteArrayElementAtIndex(index);
	}
}








