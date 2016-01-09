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
	public AnimationClip hit;
	public int damage;
	public float speed; 
	public float range;
	public Actor enemy;

	[HideInInspector]
	public Animation animationController; 
	
	int _health; 
	#endregion

	#region methods
	// Use this for initialization
	void Start () {
		_health = 100; 	
		animationController = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//check if still alive 
		if (isDead ()) {
			onDeath();
			return;
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
		Debug.Log (this.name + " HP: " + _health);
		if (!animationController.IsPlaying (attack.name) && !animationController.IsPlaying(die.name)) {
			animationController.CrossFade(hit.name);
			animationController.Play(hit.name);
		}
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
	public virtual void onDeath() {
		speed = 0;
		animationController.Play (die.name);

	}
	#endregion
}
