using UnityEngine;
using System.Collections;

public abstract class Actor : MonoBehaviour {
	#region vars
	public CharacterController controller; 
	public AnimationClip run;
	public AnimationClip idle;
	public AnimationClip die; 
	public AnimationClip waitToAttack;
	public AnimationClip attack;
	public int damage;
	public Actor enemy;

	int _health; 
	#endregion 

	#region methods
	// Use this for initialization
	void Start () {
		_health = 100; 	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//check if still alive 
		if (isDead ()) {
			onDeath();
		}
	}

	/// <summary>
	/// Checks if target is in range
	/// </summary>
	/// <returns><c>true</c>, if target is in range, <c>false</c> otherwise.</returns>
	/// <param name="target">Target.</param>
	public bool inRange(Actor target, float range) {
		return Vector3.Distance(target.transform.position, this.transform.position) <= range;
	}

	/// <summary>
	/// Gets hit, deducts HP 
	/// </summary>
	/// <param name="dmg">Dmg.</param>
	public void getHit(int dmg) {
		_health -= dmg; 
		Debug.Log (_health);
	}

	/// <summary>
	/// Checks whether HP is 0; 
	/// </summary>
	/// <returns><c>true</c>, if is dead, <c>false</c> otherwise.</returns>
	public bool isDead() {
		return _health <= 0;
	}

	/// <summary>
	/// Upon death, play die animation. 
	/// Upon animation completion, destroy gameobject. 
	/// </summary>
	void onDeath() {
		if (animation [die.name].time >= 0.9 * animation [die.name].length) {
			Destroy(gameObject);
		}
		
		animation.Play (die.name);
	}
	#endregion
}
