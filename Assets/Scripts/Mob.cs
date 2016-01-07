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

	#region methods
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!inRange ()) {
			chase ();
		} else {
			animation.CrossFade(idle.name);
		}
	}

	/// <summary>
	/// Checks whether the player is in attack range.
	/// </summary>
	/// <returns><c>true</c>, if in range, <c>false</c> otherwise.</returns>
	bool inRange() {
		if (Vector3.Distance(player.transform.position, this.transform.position) > range) {
			return false; 
		}

		return true; 
	}

	/// <summary>
	/// Chase player if not in range.
	/// </summary>
	void chase() {
		//turn 
		transform.LookAt (player.position);
		controller.SimpleMove (transform.forward * speed); 
		//animation
		animation.CrossFade (run.name);
	}

	/// <summary>
	/// Raises the mouse over event.
	/// Set the current object as the player's opponent upon mouseover. 
	/// </summary>
	void OnMouseOver() {
		//upon mouseover, set mob to the target of the player's combat system. 
		player.GetComponent<Combat> ().opponent = this.gameObject;
	}
	#endregion
}
