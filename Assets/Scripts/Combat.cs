using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {
	#region vars
	public Mob opponent; 
	public AnimationClip attack;
	public float range;
	public int damage; 

	public double impactTime; 

	bool _impacted; 

	#endregion

	#region methods
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if enemy is dead, clear focus
		if (opponent && opponent.isDead ()) {
			opponent = null; 
		}

		//TODO: CHANGE MECHANISM TO CLICK INSTEAD 
		//TODO: CONSIDER ALLOWING ATTACKS WHEN NO OPPONENT IS SELECTED
		//		vs. DISPLAYING " I NEED A TARGET " TEXT. 
		//		vs. AUTO-SELECTING CLOSEST ENEMY. 
		//make sure opponent exists and is in range before we turn and attack
		if (opponent && Input.GetKey(KeyCode.Space) && inRange ()) {
			transform.LookAt(opponent.transform.position);
			//play animation and lock movement 
			animation.Play(attack.name);
			PlayerMovement.attacking = true; 
		}

		//reset lock once animation finishes
		if (animation[attack.name].time >= animation[attack.name].length * 0.9) {
			PlayerMovement.attacking = false; 
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
		if (opponent && animation.IsPlaying (attack.name)) {
			float currentTime = animation[attack.name].time; 
			float totalLength = animation[attack.name].length;

			//impact when dagger's most extended. 
			if (currentTime >= totalLength * impactTime && currentTime < totalLength * 0.9) {
				opponent.getHit (damage);
				_impacted = true; 
			}
		}
	}

	/// <summary>
	/// Checks whether enemy is in range for attack
	/// </summary>
	/// <returns><c>true</c>, if in range, <c>false</c> otherwise.</returns>
	bool inRange() {
		return opponent && Vector3.Distance (opponent.transform.position, this.transform.position) <= range; 
	}


	#endregion
}
