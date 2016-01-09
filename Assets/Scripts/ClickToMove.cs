using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	#region constants
	const int _MAX_RAYCAST_DISTANCE = 1000; 
	const int _PLAYER_TURN_SPEED = 12;
	const int _MIN_MOVE_DISTANCE = 2; 
	#endregion 

	#region vars
	public static bool attacking;

	Vector3 _mousePosition;
	Player _player; 
	#endregion

	#region methods
	// Use this for initialization
	void Start () {
		_mousePosition = transform.position;
		_player = GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!attacking & !_player.isDead()) {
			if(Input.GetMouseButtonDown(0)) {
				//lcoate position of the mouse
				locatePosition();
			}
			moveToMousePosition();
		}
	}

	/// <summary>
	/// Locates the position of the mouse click and returns it. 
	/// If no raycast detection, return the player's position
	/// </summary>
	/// <returns>The position.</returns>
	void locatePosition() {
		//points from camera to where the ray hits against the terrain. 
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		//structure used to get information back from a raycast
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, _MAX_RAYCAST_DISTANCE)) {
			//make sure we are not trying to get to players or enemys positions
			//which are already occupied. 
			if (hit.collider.tag != "Player" && hit.collider.tag != "Enemy")
				_mousePosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
		}
	}

	/// <summary>
	/// Moves player to mouse position
	/// </summary>
	void moveToMousePosition() {
		if (Vector3.Distance (_mousePosition, transform.position) > _MIN_MOVE_DISTANCE) {
			//get the direction player should be facing
			Quaternion faceDirection = Quaternion.LookRotation (_mousePosition - transform.position);
			
			//only rotate y 
			faceDirection.x = 0f;
			faceDirection.z = 0f;

			//face the direction to move to
			transform.rotation = Quaternion.Slerp (transform.rotation, faceDirection, Time.deltaTime * _PLAYER_TURN_SPEED);
			
			//move player
			_player.controller.SimpleMove (transform.forward * _player.speed); 
			//smoothen the transition
			_player.animationController.CrossFade (_player.run.name);
		
		// if we have targeted an opponent, play attack idle animation 
		} else if (_player.enemy) {
			_player.animationController.CrossFade (_player.waitToAttack.name);
		} else {
			_player.animationController.CrossFade (_player.idle.name);		
		}
	}
	#endregion
}
