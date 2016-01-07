using UnityEngine;
using System.Collections;

public class Mob: MonoBehaviour {
	#region vars
	public float speed; 
	public float range;
	public CharacterController controller; 

	public AnimationClip run;
	public AnimationClip idle;

	public Transform player; 
	#endregion
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log (inRange ()); 
		if (!inRange ()) {
			chase ();
		} else {
			animation.CrossFade(idle.name);
		}
	}

	bool inRange() {
		if (Vector3.Distance(player.transform.position, this.transform.position) > range) {
			return false; 
		}

		return true; 
	}

	void chase() {
		//turn 
		transform.LookAt (player.position);
		controller.SimpleMove (transform.forward * speed); 
		//animation
		animation.CrossFade (run.name);
	}
}
