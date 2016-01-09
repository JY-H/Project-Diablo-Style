using UnityEngine;
using System.Collections;

public class Mob: Actor {
	#region methods
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		//otherwise, chase player
		if (!inRange(enemy, range)) {
			chase ();
		} else {
			animationController.CrossFade(idle.name);
		}
	}

	/// <summary>
	/// Chase player if not in range.
	/// </summary>
	void chase() {
		//turn 
		transform.LookAt (enemy.transform.position);
		controller.SimpleMove (transform.forward * speed); 
		//animation
		animationController.CrossFade (run.name);
	}

	/// <summary>
	/// Raises the mouse over event.
	/// Set the current object as the player's opponent upon mouseover. 
	/// </summary>
	void OnMouseOver() {
		//upon mouseover, set mob to the target of the player's combat system. 
		enemy.GetComponent<Player>().enemy = this;
	}

	#endregion
}
