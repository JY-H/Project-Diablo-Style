using UnityEngine;
using System.Collections;

public class Mob: MonoBehaviour {
	#region vars
	public float speed; 
	public float range;
	public CharacterController controller; 
	public AnimationClip run;
	public AnimationClip idle;
	public AnimationClip die; 

	public Transform player; 

	int _health; 
	#endregion

	#region methods
	// Use this for initialization
	void Start () {
		_health = 100; 
	}
	
	// Update is called once per frame
	void Update () {
		//if dead, play death animation and destroy 
		if (isDead()) {
			onDeath (); 
		}
		//otherwise, chase player
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
		return Vector3.Distance(player.transform.position, this.transform.position) <= range;
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
		player.GetComponent<Combat>().opponent = this;
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
	/// Checks whether mob is dead. 
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
