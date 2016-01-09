using UnityEngine;
using System.Collections;

public class Player : Actor {
	#region vars
	public double impactTime; 

	bool _impacted; 

	#endregion

	#region methods
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		//if enemy is dead, clear focus
		if (enemy && enemy.isDead ()) {
			enemy = null; 
		}

		//reset lock once animation finishes
		if (animationController[attack.name].time >= animationController[attack.name].length * 0.9) {
			ClickToMove.attacking = false; 
			_impacted = false; 
		}

		//TODO: CHANGE MECHANISM TO CLICK INSTEAD 
		//TODO: CONSIDER ALLOWING ATTACKS WHEN NO OPPONENT IS SELECTED
		//		vs. DISPLAYING " I NEED A TARGET " TEXT. 
		//		vs. AUTO-SELECTING CLOSEST ENEMY. 
		//make sure opponent exists and is in range before we turn and attack
		if (enemy && Input.GetKey(KeyCode.Space) && inRange (enemy, range)) {
			attackEnemy();
		}

	}

	/// <summary>
	/// Attacks the enemy.
	/// </summary>
	public override void attackEnemy() {
		base.attackEnemy ();

		//lock movement for player
		ClickToMove.attacking = true; 
		
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
		if (enemy && animationController.IsPlaying (attack.name)) {
			float currentTime = animationController[attack.name].time; 
			float totalLength = animationController[attack.name].length;

			//impact when dagger's most extended. 
			if (currentTime >= totalLength * impactTime && currentTime < totalLength * 0.9) {
				enemy.getHit (damage);
				_impacted = true; 
			}
		}
	}

	public override void onDeath() {
		//quit game
		if (animationController[die.name].time >= 0.9 * animationController[die.name].length) {
			Debug.Log ("GAME OVER");
			Application.Quit ();
		}
		base.onDeath ();
	}
	#endregion
}
