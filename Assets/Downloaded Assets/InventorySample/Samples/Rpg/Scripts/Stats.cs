// This script updates the health,mana,experience bars,the health,handles the death of the character,and the sounds and effects of those events.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stats : MonoBehaviour {

	public Image healthBar,expBar,manaBar;
	public GameObject healthPotion,staminaPotion,manaPotion;
	public AudioClip[] drinkSounds;
	public AudioClip deadSound,levelUpSound,getHitSound;
	public Renderer rend;
	public ParticleSystem levelUpEffect;
	public Text levelText;

	public GameObject ragdoll;
	public Transform potionHolder;
	public Vector3 rotation;

	private float health,exp,mana;
	private float maxHealth,maxMana,expToLevel;
	private bool dead = false;
	private int level = 1;
	private Color color;

	private Animator animator;

	void Start () {
		health = 100;
		mana = 100;
		exp = 25;
		maxHealth = health;
		maxMana = mana;
		expToLevel = 120;

		animator = GetComponent<Animator>();
		color = rend.material.color;
	}

	void Update() {
		health += Time.deltaTime * 5;
		if( health > maxHealth ) 
			health = maxHealth;
		healthBar.fillAmount = health / maxHealth;
	}

	public void ModifyHealth (float amount) {
		if( dead ) 
			return;
		health += amount;
		if( health < 0 ) {
			health = 0;
			Die();
		}
		else if( health > maxHealth ) 
			health = maxHealth;
		if( amount > 0 ) {
			GameObject potion = (GameObject)Instantiate(healthPotion,potionHolder.position,Quaternion.identity);
			potion.transform.SetParent(potionHolder);
			potion.transform.localRotation = Quaternion.Euler(rotation);
			StartCoroutine("DrinkPotion",potion);
		}
		else {
			if( health > 0 )
				StartCoroutine( "GetHit" );
		}
		healthBar.fillAmount = health / maxHealth;
	}

	public void ModifyExperience (float amount) {
		if( dead )
			return;
		exp += amount;
		if( exp > expToLevel ) {
			LevelUp( exp - expToLevel );
		}
		expBar.fillAmount = exp / expToLevel;
	}

	public void ModifyMana (float amount) {
		if( dead )
			return;
		mana += amount;
		if( mana > maxMana )
			mana = maxMana;
		else if( mana < 0 ) 
			mana = 0;
			
		manaBar.fillAmount = mana / maxMana;
	}

	IEnumerator DrinkPotion (GameObject potion) {
		animator.SetTrigger("drink");
		yield return new WaitForSeconds(1.5f);
		foreach(var drinkSound in drinkSounds)
			GetComponent<AudioSource>().PlayOneShot(drinkSound);
		yield return new WaitForSeconds(1);
		Destroy(potion);
	}

	IEnumerator GetHit() {
		rend.material.color = Color.red;
		GetComponent<AudioSource>().PlayOneShot( getHitSound );
		yield return new WaitForSeconds( 0.65f );
		rend.material.color = color;
	}

	void Die() {
		ragdoll.SetActive( true );
		ragdoll.GetComponent<AudioSource>().volume = 0.5f;
		ragdoll.GetComponent<AudioSource>().PlayOneShot( deadSound );
		gameObject.SetActive( false );
		dead = true;
	}

	void LevelUp(float remainingExp) {
		if( dead )
			return;
		GetComponent<AudioSource>().PlayOneShot( levelUpSound );
		levelUpEffect.Play();
		expToLevel *= 1.2f;
		exp = remainingExp;
		level ++;
		levelText.text = level.ToString();
	}
}












