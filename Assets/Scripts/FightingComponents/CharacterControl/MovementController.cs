using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all sideways character movement.
/// </summary>
public class MovementController : MonoBehaviour {

	private Rigidbody2D thisBody;
	private bool collisionLeft;
	private bool collisionRight;
	private bool knockedBack;
	private float previousPushedVelocity = 0; //Used to compare when the character has been knocked back and is approaching velocity 0 again.
	private Vector2 prePauseVelocity;

	// Use this for initialization
	void Awake () {
		thisBody = gameObject.GetComponent<Rigidbody2D> ();
		thisBody.mass = Parameters.mass;
	}
	
	// Update is called once per frame
	void Update () {
		//Check if player has been knocked back and is now standing still again.
		//Without previousPushedVelocity, the velocity is zero in the first frame and no knockback is applied.
		if (Mathf.Abs (thisBody.velocity.x) < 0.1 && knockedBack && previousPushedVelocity > Mathf.Abs (thisBody.velocity.x)) {
			//Remove any colision that was detected before the character was knocked back.
			this.collisionLeft = false;
			this.collisionRight = false;
			//Reset for the first comparison next time KnockBack is called.
			knockedBack = false;
			previousPushedVelocity = 0;
		}
	}

	//Check previous velocity after update has run.
	void LateUpdate()
	{
		if (knockedBack)
		{
			previousPushedVelocity = Mathf.Abs (thisBody.velocity.x);
		}
	}

	public void MoveLeft(){
		if(!collisionLeft && !knockedBack)
		{
			thisBody.velocity = new Vector2(-Parameters.moveSpeed, thisBody.velocity.y);
			collisionRight = false; //If moving left, there is no longer a collision to the right.
		}
	}

	public void DashLeft(){
		if(!collisionLeft && !knockedBack)
		{
			thisBody.AddForce (Vector2.left * Parameters.knockbackModifyer * 1.3f);
			this.KnockBack();
			collisionRight = false; //If moving left, there is no longer a collision to the right.
		}
	}

	public void MoveRight(){
		if (!collisionRight && !knockedBack)
		{
			thisBody.velocity = new Vector2(Parameters.moveSpeed, thisBody.velocity.y);
			collisionLeft = false; //If moving right, there is no longer a collision to the left
		}
	}

	public void DashRight(){
		if (!collisionRight && !knockedBack)
		{

			thisBody.AddForce (Vector2.right * Parameters.knockbackModifyer * 1.3f);
			this.KnockBack();
			collisionLeft = false; //If moving right, there is no longer a collision to the left
		}
	}

    public void CrouchRight()
    {
        if (!collisionRight && !knockedBack)
        {
            thisBody.velocity = new Vector2(Parameters.crouchSpeed, thisBody.velocity.y);
            collisionLeft = false; //If moving right, there is no longer a collision to the left
        }
    }

    public void CrouchLeft()
    {
        if (!collisionRight && !knockedBack)
        {
            thisBody.velocity = new Vector2(-Parameters.crouchSpeed, thisBody.velocity.y);
            collisionLeft = false; //If moving right, there is no longer a collision to the left
        }
    }

    public void Stop(){
		if(!knockedBack)
		{
			thisBody.velocity = new Vector2(0, thisBody.velocity.y); //Without the knockedBack bool, this stops the character.
		}
	}

	public void CollisionLeft()
	{
		this.collisionLeft = true;
	}

	public void CollisionRight()
	{
		this.collisionRight = true;
	}

	public void CollisionExitLeft()
	{
		this.collisionLeft = false;
	}

	public void CollisionExitRight()
	{
		this.collisionRight = false;
	}

	public void KnockBack()
	{
		this.knockedBack = true;
	}

	public void Pause()
	{
		prePauseVelocity = thisBody.velocity;
		thisBody.velocity = Vector2.zero;
	}

	public void UnPause()
	{
		thisBody.velocity = prePauseVelocity;
	}

	public bool isKnockedBack(){
		return this.knockedBack;
	}
}
