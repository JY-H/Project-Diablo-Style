using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	#region constants
	const int _MAX_RAYCAST_DISTANCE = 1000; 
	const int _PLAYER_TURN_SPEED = 12;
	#endregion 

	#region vars
	public float speed;
	public CharacterController characterController;
	public AnimationClip run;
	public AnimationClip idle;

	Vector3 _mousePosition;
	#endregion

	#region methods
	// Use this for initialization
	void Start () {
		_mousePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			//lcoate position of the mouse
			locatePosition();
		}
		moveToMousePosition();
	}

	/// <summary>
	/// Locates the position of the mouse click and returns it. 
	/// If no raycast detection, return the player's position
	/// </summary>
	/// <returns>The position.</returns>
	void locatePosition() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		//structure used to get information back from a raycast
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, _MAX_RAYCAST_DISTANCE)) {
			//retrieve coordinates from raycast and store it as position
			_mousePosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
		}
	}

	/// <summary>
	/// Moves player to mouse position
	/// </summary>
	void moveToMousePosition() {
		if (Vector3.Distance (_mousePosition, transform.position) > 1) {
			//get the direction player should be facing
			Quaternion faceDirection = Quaternion.LookRotation (_mousePosition - transform.position);
			
			//only rotate y 
			faceDirection.x = 0f;
			faceDirection.z = 0f;
			//face the direction to move to
			transform.rotation = Quaternion.Slerp (transform.rotation, faceDirection, Time.deltaTime * _PLAYER_TURN_SPEED);
			
			//move player
			characterController.SimpleMove (transform.forward * speed); 
			//smoothen the transition
			animation.CrossFade (run.name);
		} else {
			animation.CrossFade (idle.name);
		}
	}
	#endregion
}
