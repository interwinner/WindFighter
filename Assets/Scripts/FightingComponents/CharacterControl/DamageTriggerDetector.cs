using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for applying knockback, damage and stun when collision occurs between a character or a shield and a damaging bodypart.
/// </summary>
public class DamageTriggerDetector : MonoBehaviour {

	public int characterIndex;
	private Character character;
	private InputController inputController;

	private AudioSource audioCenter;
	private AudioClip punch;

	private GameState gameState;
	private Rigidbody2D rigidBody;

	private GameObject hitParticle;

	void Start ()
	{
		this.inputController = transform.root.gameObject.GetComponent<InputController> ();
		this.character = StaticCharacterHolder.characters[characterIndex - 1];
		this.rigidBody = transform.root.gameObject.GetComponent<Rigidbody2D> ();
		this.gameState = GameObject.Find ("Handler").GetComponent<GameState> ();
		this.hitParticle = Resources.Load("Prefabs/hitParticle", typeof(GameObject)) as GameObject;
		audioCenter = GameObject.Find("AudioCenter").GetComponent<AudioSource>();
	}

	void OnTriggerEnter2D (Collider2D otherCollider)
	{
		Transform otherRootTransform = otherCollider.transform.root;
		GameObject otherCharacterObject = otherRootTransform.gameObject;
		InputController otherInputController = otherCharacterObject.GetComponent<InputController> ();
		if (otherInputController == null)
		{
			return; //If colliding object's root does not have an InputController, it is not a character.
		}
		Character otherCharacter = otherInputController.GetCharacter ();
		Move damagingMove = otherInputController.GetCurretlyPlayedMove ();
		if (otherCharacter == null || damagingMove == null || damagingMove.IsBlockMove ()
			|| otherCharacter.Equals (this.character) || otherCollider.transform.tag.Equals ("characterCollider"))
		{
			return; //Make sure other character object has all necessary info.
		}

		if (!damagingMove.GetActiveBodypart ().Equals (otherCollider.transform.name))
		{
			return; //Make sure other collider is actually the damaging move.
			//This should not be necessary but if another collider is on, it will do damage even if
			// it is not the damaging move. This is just to be absolutely sure.
		}
		if (this.character.isInvunerable ()) {
			return;
		}

		this.character.SetInvunderable (true); //Make sure no other collider can do damage to the character in this frame.

		otherCollider.enabled = false; //Make sure the other character's damaging bodypart does not also collide with character behind shield.
		audioCenter.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/punch_general");
		audioCenter.Play(); 

		GameObject particleObject = Instantiate(this.hitParticle, (otherCollider.transform.position + transform.position) / 2, otherCollider.transform.rotation);
		ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
		particleSystem.Play();
		Destroy (particleObject,particleSystem.main.duration);
		
        this.inputController.SetHitColor();


		// PARTICLE STUFF HERE

		float knockBackModifier = 0.0f;
		Move blockMove = this.inputController.GetCurretlyPlayedMove ();
		//If the collision occurs between the active shield of a block move, reduce dmg, knockback and stun.
		if (blockMove != null && blockMove.IsBlockMove () && this.transform.name.Contains ("Shield"))
		{
			this.character.ApplyMoveTo (damagingMove, blockMove); //Apply damage in model.

			transform.root.GetComponent<MovePlayer> ().reset ();
			otherCharacterObject.GetComponent<MovePlayer> ().reset ();
			otherCharacterObject.GetComponent<Animator> ().enabled = true;
			otherCharacterObject.GetComponent<Animator> ().SetBool ("Stunned", true);

			//Translate percentage strength of damaging move and translate to stun
			float minStrength = Parameters.minStrength;
			float maxStrength = Parameters.maxStrength;
			float stunPercentage = (damagingMove.GetStrength () - minStrength) / (maxStrength - minStrength);
			float minStun = Parameters.minStun;
			float maxStun = Parameters.maxStun;
			float stunTime = minStun + (maxStun - minStun) * stunPercentage;

			otherInputController.PauseSeconds (stunTime);
			knockBackModifier = Parameters.blockKnockbackModifier;
		}
		else
		{
			this.character.ApplyMoveTo (damagingMove); //Apply damage in model.
			knockBackModifier = Parameters.knockbackModifyer;
		}

		gameState.UpdateCharacterHealth (this.character); //Update health bars and check winner.

		//Apply knockback.
		Vector3 thisPosition = transform.position;
		Vector3 otherPosition = otherRootTransform.position;

		
		if(Mathf.Abs (this.rigidBody.velocity.y) > 0.001){
			knockBackModifier /=2;
		}
		Vector2 newVelocity = new Vector2(0,this.rigidBody.velocity.y);
		this.rigidBody.velocity = newVelocity;

		if (otherPosition.x < thisPosition.x)
		{
			this.rigidBody.AddForce (Vector2.right * knockBackModifier);
			this.inputController.KnockBack (); //Make the character unable to move while being knocked back.
		}
		else
		{
			this.rigidBody.AddForce (Vector2.left * knockBackModifier);
			this.inputController.KnockBack (); //Make the character unable to move while being knocked back.
		}
	}
}
