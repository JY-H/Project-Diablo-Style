using UnityEngine;
using System.Collections;

public class Mob: Actor {
	#region vars
	public double impactTime;
	bool _impacted;
	#endregion

	#region methods
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		//since we only destroy gameobject after die animation plays, 
		//we still need to check whether the mob is dead as there should
		//be a few seconds delay from actual death to death registered. 
		if (!isDead ()) {
			//chase player
			if (!inRange(enemy, range)) {
				chase ();
			} else {
				//reset lock once animation finishes
				if (animationController[attack.name].time >= animationController[attack.name].length * 0.9) {
					_impacted = false; 
				}		
				if (!enemy.isDead()) {
					animationController.Play(attack.name);
					attackEnemey(); 
				}
			}
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

	public virtual void attackEnemey() {
		if (!_impacted && enemy && animationController.IsPlaying (attack.name)) {
			float currentTime = animationController[attack.name].time; 
			float totalLength = animationController[attack.name].length;
			
			//impact when dagger's most extended. 
			if (currentTime >= totalLength * impactTime && currentTime < totalLength * 0.9) {
				enemy.getHit (damage);
				_impacted = true; 
			}
		}
	}
	/// <summary>
	/// Raises the mouse over event.
	/// Set the current object as the player's opponent upon mouseover. 
	/// </summary>
	void OnMouseOver() {
		//upon mouseover, set mob to the target of the player's combat system. 
		enemy.enemy = this;
	}

	public override void onDeath() {
		if (animationController[die.name].time >= 0.9 * animationController[die.name].length) {
			Destroy(gameObject);
		}
		base.onDeath();
	}

	#endregion
}
