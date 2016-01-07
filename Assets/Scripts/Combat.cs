using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {
	#region vars
	public GameObject opponent; 
	public AnimationClip attack; 

	#endregion

	#region methods
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (opponent)
			Debug.Log (opponent.name);

		//TODO: CHANGE MECHANISM TO CLICK INSTEAD 
		//space to attack 
		if (Input.GetKey(KeyCode.Space)) {
			//make sure opponent exists before we turn and attack
			if (opponent) {
				transform.LookAt(opponent.transform.position);
			}

			//play animation and lock movement 
			animation.Play(attack.name);
			PlayerMovement.attacking = true; 
		}

		// reset lock once animation finishes
		if (!animation.IsPlaying (attack.name)) {
			PlayerMovement.attacking = false; 
		}
	}


	#endregion
}
