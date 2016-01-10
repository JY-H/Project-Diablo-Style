using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ItemManagerWindow : EditorWindow {
	
	public static bool dragging;
	SerializedObject container;
	SerializedProperty numberOfRecipes;
	int selectedItemIndex;
	public static string draggedItem;
	bool drawLayout = true;
	int inspectedRecipe;

	// Drawing
    private delegate void DrawEditor();
    private DrawEditor drawFunction;
	private bool shouldRepaint;

    public static SerializedObject manager = null;
    public static SerializedProperty itemList = null;
	public static int drawnAttribute;
	
    // Scrolling
	private Vector2 typesScrollPosition = Vector2.zero;
	private Vector2 attributesScrollPosition = Vector2.zero;
	private Vector2 layoutScrollPosition;
	private Vector2 recipeScrollPosition;
	private Vector2 recipesScrollPosition;

    // Types & Attributes
    private SerializedProperty listOfTypes;
    private SerializedProperty listOfAttributes;
    public static string[] types = new string[1];
	public static string[] attributes = new string[1];
	public static string[] items = new string[1];

	// Style
	private GUIStyle style;

    [MenuItem ("Inventory/Generic")]
	static void Init () {
        var window = (ItemManagerWindow)EditorWindow.GetWindow(typeof(ItemManagerWindow));
        window.Repaint();
	}

    void OnEnable() {
		InitializeWindow();
		// Style
		style = new GUIStyle();
		style.richText = true;
		style.fontStyle = FontStyle.Bold;
    }

    void OnGUI() {
        // Types & Attributes
		if (manager != null) {
			items = new string[itemList.arraySize];
			for (int i = 0;i < itemList.arraySize;i ++) {
				items[i] = itemList.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
			}

			if (listOfTypes.arraySize != 0) {
		        types = new string[listOfTypes.arraySize];
		        for (int i = 0; i < listOfTypes.arraySize; i++) {
		            types[i] = listOfTypes.GetArrayElementAtIndex(i).stringValue;
				}
	    	}
			if (listOfAttributes.arraySize != 0) {
				attributes = new string[listOfAttributes.arraySize];
				for (int i = 0; i < listOfAttributes.arraySize; i++) {
					attributes[i] = listOfAttributes.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
				}
			}

			Rect temp1 = new Rect(Screen.width / 2 - 128,10,128,32);
			Rect temp2 = new Rect(Screen.width / 2, 10, 128, 32);
			if (GUI.Button(temp1, "Types & Attributes"))
				drawFunction = DrawWindow;
			else if (GUI.Button(temp2, "Crafting Manager")) {
				drawFunction = DrawCraftingManager;
			}
			GUI.Box(new Rect(0, 128, Screen.width, 1), "");
			DrawStats();
		}
        drawFunction();
    }

    private void DrawWarningMessage() {
        EditorGUILayout.LabelField(new GUIContent("No Inventory manager present in the scene,please set it up"));
        if (GUILayout.Button("Set it up!")) {
			InstallInventory();
        }
    }

    private void DrawWindow() {
        manager.Update();

		DrawTypes();
		DrawAttributes();

       manager.ApplyModifiedProperties();
    }

    private void DrawTypes() {
		GUI.BeginGroup(new Rect(0,128, Screen.width * 0.5f, Screen.height - 128));
		
		float scrollAmount = (listOfTypes.arraySize * 24 - 290) > 0 ? listOfTypes.arraySize * 24 - 290 : 0f;
		Rect scrollSize = new Rect(0, 0, Screen.width * 0.5f,Screen.height - 128 + scrollAmount);
		typesScrollPosition = GUI.BeginScrollView(new Rect(0,0,Screen.width * 0.5f,Screen.height - 128), typesScrollPosition, scrollSize,true,true);
		// Types list
		Rect rect = new Rect(10,10,36,16);
		EditorGUI.PropertyField(rect,listOfTypes);

		if (listOfTypes.isExpanded) {
			rect.y += 16;
			Rect temp = new Rect(rect);
			for (int cnt = 0;cnt < listOfTypes.arraySize; cnt ++) {
				EditorGUI.PropertyField(new Rect(temp.x,temp.y,128,16),listOfTypes.GetArrayElementAtIndex(cnt),GUIContent.none);
				temp.y += 24;
			}
			temp.width = 24;
			if (GUI.Button(temp,"+")) {
				listOfTypes.arraySize ++;
			}
			temp.x += 24;
			if (GUI.Button(temp,"-")) {
				listOfTypes.arraySize --;
			}
		}
		GUI.EndScrollView();
		GUI.EndGroup();
	}

	private void DrawAttributes() {
		GUI.BeginGroup(new Rect(Screen.width * 0.5f,128, Screen.width * 0.5f, Screen.height - 128));
		
		float scrollAmount = (listOfAttributes.arraySize * 24 - 280) > 0 ? listOfAttributes.arraySize * 24 - 280: 0f;
		Rect scrollSize = new Rect(0, 0, Screen.width * 0.5f,Screen.height - 128 + scrollAmount);
		attributesScrollPosition = GUI.BeginScrollView(new Rect(0,0,Screen.width * 0.5f,Screen.height - 128), attributesScrollPosition, scrollSize,true,true);
		// Attributes list
		Rect rect = new Rect(10,10,36,16);
		EditorGUI.PropertyField(rect,listOfAttributes);
		listOfAttributes.isExpanded = true;
		
		if (listOfAttributes.isExpanded) {
			GUI.Label(new Rect(10,24,256,16),"\t            Name\t\t  ID",style);
			Rect temp = new Rect(10,36,256,16);
			for (int cnt = 0;cnt < listOfAttributes.arraySize; cnt ++) {
				drawnAttribute = cnt;
				EditorGUI.PropertyField(temp,listOfAttributes.GetArrayElementAtIndex(cnt),GUIContent.none,true);
				temp.y += 24;
			}
			temp.width = 48;
			if (GUI.Button(temp,"+")) {
				listOfAttributes.arraySize ++;
			}
			temp.x = temp.xMax;
			if (GUI.Button(temp,"-")) {
				listOfAttributes.arraySize --;
			}
		}
		GUI.EndScrollView();
		GUI.EndGroup();
	}

	// System
	private void InstallInventory () {
		var managerInstance = new GameObject("InventoryManager");
		var managerComponent = managerInstance.AddComponent<InventoryManager>();
		manager = new SerializedObject(managerComponent);
		itemList = manager.FindProperty("templates");
		listOfAttributes = manager.FindProperty("attributes");
		listOfTypes = manager.FindProperty("types");
		drawFunction = DrawWindow;
	}

	InventoryManager InventoryExists { get { var manag = FindObjectOfType<InventoryManager>(); return manag == null ? null : manag; } }

	private void InitializeWindow() {
		if (InventoryExists != null) {
			manager = new SerializedObject(InventoryExists);
			itemList = manager.FindProperty("templates");
			listOfTypes = manager.FindProperty("types");
			listOfAttributes = manager.FindProperty("attributes");
			drawFunction = DrawWindow;
		} else {
			drawFunction = DrawWarningMessage;
		}
	}
	
	private void DrawStats () {
		Rect rect = new Rect(32,10,48,32);
		GUI.Label(rect,"<b>Stats:</b>",style);

		// Item number
		rect.y += 24;
		rect.x /= 2f;
		rect.width = 128;
		GUI.Label(rect,"Items: " + itemList.arraySize.ToString(),style);

		// Types number
		rect.y += 18;
		GUI.Label(rect,"Types: " + listOfTypes.arraySize.ToString(),style);

		// Attributes number
		rect.y += 18;
		GUI.Label(rect,"Attributes: " + listOfAttributes.arraySize.ToString(),style);

		// Recipes number
		rect.y += 18;
		if (container != null)
			GUI.Label(rect,"Recipes: " + container.FindProperty( "numberOfRecipes" ).intValue,style);
	}

	private void DrawCraftingManager() {
		manager.Update();

		Rect rect = new Rect(16,160,384,16);
		var sc = manager.FindProperty("currentInspectedContainer");
		EditorGUI.PropertyField(rect,sc,GUIContent.none);
		container = new SerializedObject(manager.FindProperty("currentInspectedContainer").objectReferenceValue);
		rect.y -= 16;
		GUI.Label(rect,"Inspected  Container:");
		DrawCraftingPanels();
		manager.ApplyModifiedProperties();

		rect.height = 1;
		rect.y += 48;
		rect.width = Screen.width;
		rect.x = 0;
		GUI.Box(rect,"");
	}

	void DrawCraftingPanels () {
		container.Update();
		numberOfRecipes = container.FindProperty("numberOfRecipes");

		// Settings -----------------------------------------------------------------------------------------------------
		var offset = manager.FindProperty("layoutOffset");
		var spacingCoefficient = manager.FindProperty("spacingCoefficient");
		var sizeCoefficient = manager.FindProperty("sizeCoefficient");

		var settingsRect = new Rect(Screen.width * 0.75f + 16,256,128,16);
		EditorGUI.PropertyField(settingsRect,offset,GUIContent.none);
		settingsRect.y -= 16;
		GUI.Label(settingsRect,"Offset");
		settingsRect.y += 16;
		settingsRect.y += 48;
		EditorGUI.PropertyField(settingsRect,spacingCoefficient,GUIContent.none);
		settingsRect.y -= 16;
		GUI.Label(settingsRect,"Spacing");
		settingsRect.y += 16;
		settingsRect.y += 48;
		EditorGUI.PropertyField(settingsRect,sizeCoefficient,GUIContent.none);
		settingsRect.y -= 16;
		GUI.Label(settingsRect,"Slot size");
		settingsRect.y += 16;

		settingsRect.y += 48;
		if (GUI.Button(settingsRect,"EditLayout")) {
			drawLayout = true;
		}
		settingsRect.x = settingsRect.xMax;
		if (GUI.Button(settingsRect,"EditRecipes")) {
			drawLayout = false;
		}
		//  ----------------------------------------------------------------------------------------------------------

		// Layout setup ----------------------------------------------------------------------------------------------------------
		if (drawLayout) {
			layoutScrollPosition = GUI.BeginScrollView(new Rect(0,192,Screen.width * 0.75f,Screen.height - 220),layoutScrollPosition,new Rect(0,192,Screen.width * 4,Screen.height * 4),true,true);
			var slots = (container.targetObject as Container).GetComponentsInChildren<Slot>();

			// Creating the layout
			for (int i = 0;i < slots.Length;i ++) {
				var sslot = new SerializedObject(slots[i]);
				sslot.Update();
				var rt = slots[i].transform as RectTransform;
				Rect temp = new Rect(rt.localPosition.x,-rt.localPosition.y,rt.rect.width,rt.rect.height);
				temp.y = Screen.height / 2 - 192 - offset.vector2Value.y + spacingCoefficient.floatValue * temp.y;
				temp.x = Screen.width * 0.35f + offset.vector2Value.x + spacingCoefficient.floatValue * temp.x;
				temp.width *= sizeCoefficient.floatValue;
				temp.height *= sizeCoefficient.floatValue;
				sslot.FindProperty("positionInCrafting").rectValue = temp;
				GUI.Box(temp,"");

				if (container.FindProperty("receiver").objectReferenceValue == sslot.targetObject) {
					GUI.color = Color.green;
				}
				if (GUI.Button(new Rect(temp.x,temp.y,20,20),"R")) {
					container.FindProperty("receiver").objectReferenceValue = sslot.targetObject;
				}
				GUI.color = Color.white;
				sslot.ApplyModifiedProperties();
			}
			GUI.EndScrollView();
		}
		//  ----------------------------------------------------------------------------------------------------------
		else {
			// Recipe menu ------------------------------------------------------------------------------------------------------------------------------------------
			numberOfRecipes = container.FindProperty("numberOfRecipes");
			var serializedReceiver = new SerializedObject( container.FindProperty( "receiver" ).objectReferenceValue );
			serializedReceiver.Update();

			var receiverRecipes = serializedReceiver.FindProperty( "recipes" );
			var inspectorRect = new Rect( 440,160,128,16 );
			var initialNumberOfRecipes = numberOfRecipes.intValue;
			if( GUI.Button( inspectorRect,"Add" ) ) {
				receiverRecipes.arraySize ++;
				numberOfRecipes.intValue ++;
				receiverRecipes.GetArrayElementAtIndex( numberOfRecipes.intValue - 1 ).FindPropertyRelative( "itemName" ).stringValue = "New" + (numberOfRecipes.intValue - 1).ToString();
			}
			inspectorRect.x = inspectorRect.xMax;
			if( GUI.Button( inspectorRect,"Remove" ) && numberOfRecipes.intValue > 0 ) {
				numberOfRecipes.intValue --;
			}
			// Showing the popup with recipes
			string[] recipeNames = new string[initialNumberOfRecipes];

			for ( int i = 0;i < initialNumberOfRecipes;i ++ ) {
				recipeNames[i] = receiverRecipes.GetArrayElementAtIndex( i ).FindPropertyRelative( "itemName" ).stringValue;
			}
			inspectorRect.x = inspectorRect.xMax;
			inspectedRecipe = EditorGUI.Popup( inspectorRect,inspectedRecipe,recipeNames );
			if( inspectedRecipe >= numberOfRecipes.intValue )
				inspectedRecipe = numberOfRecipes.intValue - 1;
			serializedReceiver.ApplyModifiedProperties();

			// Finding the slots and initializing ------------------------------------------------------------------------------------------------------------------------------------------
			var slots = (container.targetObject as Container).GetComponentsInChildren<Slot>();
			recipesScrollPosition = GUI.BeginScrollView(new Rect(0,192,Screen.width * 0.75f,Screen.height - 220),recipesScrollPosition,new Rect(0,220,Screen.width * 0.735f,Screen.height - 220));
			var style = new GUIStyle();
			style.alignment = TextAnchor.MiddleCenter;

			// Displaying the slots ------------------------------------------------------------------------------------------------------------------------------------------
			if( numberOfRecipes.intValue > 0 ) {
				for ( int i = 0; i < slots.Length;i ++ ) {
					var serializedSlot = new SerializedObject ( slots[i] );
					serializedSlot.Update();

					// Drawing the raw slots
					var recipes = serializedSlot.FindProperty ( "recipes" );
					recipes.arraySize = numberOfRecipes.intValue;
					var slotRect = serializedSlot.FindProperty ( "positionInCrafting" ).rectValue;
					GUI.Box( slotRect,"" );
					GUI.Label( slotRect,recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "itemName" ).stringValue,style );

					// Dragging and dropping
					if( slotRect.Contains( Event.current.mousePosition ) && Event.current.type == EventType.mouseUp && dragging ) {
						recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "itemName" ).stringValue = draggedItem;
						dragging = false;
						draggedItem = "";
					}

					// Buttons... and labels...
					if( GUI.Button( new Rect( slotRect.x,slotRect.y,20,20 ),"x" ) ) {
						if( serializedSlot.targetObject == serializedReceiver.targetObject )
							recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "itemName" ).stringValue = "empty" + inspectedRecipe.ToString();
						else
							recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "itemName" ).stringValue = "";
						recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "amount" ).intValue = 0;
					}
					else if( GUI.Button( new Rect( slotRect.x,slotRect.yMax - 20,20,20 ),"-" ) && recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "amount" ).intValue > 0 ) {
						recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "amount" ).intValue --;
					}
					else if( GUI.Button( new Rect( slotRect.xMax - 20,slotRect.yMax - 20,20,20 ),"+" ) ) {
						recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "amount" ).intValue ++;
					}
					GUI.Label( new Rect( slotRect.xMax - 20,slotRect.y,48,48 ),recipes.GetArrayElementAtIndex( inspectedRecipe ).FindPropertyRelative( "amount" ).intValue.ToString() );

					serializedSlot.ApplyModifiedProperties();
				}
			}

			GUI.EndScrollView();
		}
		DrawDatabase();
		container.ApplyModifiedProperties();
	}

	void DrawDatabase () {
		Rect rect = new Rect(Screen.width * 0.75f + 16,440,128,16);
		string item;
		selectedItemIndex = EditorGUI.Popup(rect,selectedItemIndex,items);
		item = items[selectedItemIndex];
		rect.x = rect.xMax;
		rect.height = 32;
		GUI.Box(rect,item);

		// Checking for dragging
		if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDrag) {
			dragging = true;
			draggedItem = item;
		}
		if (dragging) {
			GUI.Label(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,128,32),draggedItem);
			Repaint();
		}
	}
}








