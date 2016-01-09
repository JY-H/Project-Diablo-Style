using UnityEngine;
using System.Collections;

/// <summary>
/// Interface for actors who are threats. Threats are considered
/// those that can attack and get killed.
/// </summary>
public interface IThreat {
	bool inRange(Actor target, float range);
	void attackEnemy();
	void getHit(int dmg);
	bool isDead();
	void onDeath();
}
