using UnityEngine;
using System.Collections;

public class Player : Actor {
	#region vars
	public float speed;
	public float range;
	public double impactTime; 

	bool _impacted; 

	#endregion

	#region methods
	// Update is called once per frame
	public override void Update () {
		//if enemy is dead, clear focus
		if (enemy && enemy.isDead ()) {
			enemy = null; 
		}

		//TODO: CHANGE MECHANISM TO CLICK INSTEAD 
		//TODO: CONSIDER ALLOWING ATTACKS WHEN NO OPPONENT IS SELECTED
		//		vs. DISPLAYING " I NEED A TARGET " TEXT. 
		//		vs. AUTO-SELECTING CLOSEST ENEMY. 
		//make sure opponent exists and is in range before we turn and attack
		if (enemy && Input.GetKey(KeyCode.Space) && inRange (enemy, range)) {
			transform.LookAt(enemy.transform.position);
			//play animation and lock movement 
			animation.Play(attack.name);
			ClickToMove.attacking = true; 
		}

		//reset lock once animation finishes
		if (animation[attack.name].time >= animation[attack.name].length * 0.9) {
			ClickToMove.attacking = false; 
			_impacted = false; 
		}

		//only impact opponent once. 
		if (!_impacted) {
			impact ();
		}
	}

	/// <summary>
	/// Deduct HP from opponent upon weapon hit. 
	/// Impact is determined by the animation, where the weapon is extended to attack 
	/// </summary>
	void impact() {
		if (enemy && animation.IsPlaying (attack.name)) {
			float currentTime = animation[attack.name].time; 
			float totalLength = animation[attack.name].length;

			//impact when dagger's most extended. 
			if (currentTime >= totalLength * impactTime && currentTime < totalLength * 0.9) {
				enemy.getHit (damage);
				_impacted = true; 
			}
		}
	}

	#endregion
}
