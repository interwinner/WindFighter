using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes input from the player and delegates it to the corresponding controller. Delegates input to FightMoveController, MovementController and JumpController.
/// </summary>
public class InputController : MonoBehaviour
{
	// Unity specific strings related to movement keys in our case.
    public string horizontalAxis;
    public string verticalAxis;
    public string horizontalAxisJoystick;
    public string verticalAxisJoystick;

	public int characterIndex;
	private Character character;

    private float pauseTime;
	private bool pausedForTime;
	private bool paused;

    // Booleans for coordinating transitions between animations
    private bool isRunning = false;
    private bool isCrouching = false;
    private bool collisionDown = true;
    private float leftDoubleClickCooldown = 0;
    private float rightDoubleClickCooldown = 0;
    private float leftDashCoolDown = 0;
    private float rightDashCoolDown = 0;
    private bool pressedRight = false;
    private bool pressedLeft = false;
    private bool pressedLeftAndReleased = false;
    private bool pressedRightAndReleased = false;

	// isPlayingMove exists in addition to the MovePlayer.CheckIsPlaying() method to avoid concurrency issues.
	bool isPlayingMove = false;
	Animator animator;

	private JumpController jumpController;
	private MovementController movementController;
	private FightMoveController fightMoveController;
    
    void Awake ()
    {   
        // initialize all controllers in Awake() to make sure no collisions occur before controllers can handle them
        this.jumpController = gameObject.GetComponent<JumpController>();
        this.movementController = gameObject.GetComponent<MovementController>();
        this.fightMoveController = gameObject.GetComponent<FightMoveController>();
    }

	void Start () {
		// characterIndex-1 to make character 1 have index 1 etc.
		this.character = StaticCharacterHolder.characters[characterIndex - 1];
		this.fightMoveController.SetCharacter (this.character);
		this.pauseTime = 0;
		this.pausedForTime = false;
		this.paused = false;
		this.animator = GetComponent<Animator>();

        // Pause inputcontroller to let fightscene animations finish playing
        PauseSeconds(2.0f);
	}

    void Update() {
		// Reduce pause time or unpause if pause time has run out.
        if (pauseTime > 0)
        {
            pauseTime -= Time.deltaTime;
        }

        if (pauseTime <= 0 && pausedForTime)
        {
            UnPause();
        }

		if (paused) {
			return; //End method if game is paused.
		}
		// If previous move finished, reset isPlayingMove and enable the animator.
		if (!fightMoveController.IsDoingMove ())
		{
			isPlayingMove = false;
			SetAnimatorEnabled (true);
		}

		//Check fight move input.
		if (Input.anyKeyDown)
		{
			string pressedButton = "";
			foreach (string button in InputSettings.allUsedButtons)
			{
                try {
                    if (Input.GetKeyDown (button))
                    {
                        pressedButton = button;
                    }
                }
                catch
                {

                }
                try {
                    if (Input.GetButtonDown (button))
                    {
                        pressedButton = button;
                    }
                }
                catch
                {
                    
                }
				
			}
			if (InputSettings.HasButton (characterIndex, pressedButton) && !isPlayingMove && !isCrouching && !animator.GetCurrentAnimatorStateInfo(0).IsName("crouch"))
			{
				isPlayingMove = true;
				SetAnimatorEnabled (false);
				string moveName = InputSettings.GetMoveName (pressedButton);
				fightMoveController.DoMove (moveName);
			}
		}

        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float horizontalJoystick = Input.GetAxisRaw(horizontalAxisJoystick);
        float vertical = Input.GetAxisRaw(verticalAxis);
        float verticalJoystick = Input.GetAxisRaw(verticalAxisJoystick);
        
		//isPlaying is whether a fight move is being played. If it is, don't move the character.
		if (!isPlayingMove)
		{
			// Move sideways
			if ((horizontal < 0 || horizontalJoystick < 0) && !this.isCrouching && !this.animator.GetBool("CrouchWalking"))
	        { 
				//Time left on doubleclick cooldown means this button press is within time frame of a doubleclick.
				//PressedLeftAndReleased means button has been pressed once, this press is the doubleclick.
                if (leftDoubleClickCooldown > 0 && pressedLeftAndReleased && this.collisionDown) {
					// If dashcooldown is below zero, do a dash, otherwise, do nothing and reset doubleclick cooldown.
					if (leftDashCoolDown <= 0)
					{
						this.movementController.DashLeft ();
						pressedLeftAndReleased = false;
						pressedLeft = false;
						leftDoubleClickCooldown = 0.0f;
						leftDashCoolDown = 0.7f;
					}
					else
					{
						pressedLeft = true;
						leftDoubleClickCooldown = 0.0f;
						this.movementController.MoveLeft ();
					}                    
                }
                else 
                {
                    pressedLeft = true;
                    leftDoubleClickCooldown = 0.5f;
                    this.movementController.MoveLeft ();
                }
	        }
			else if ((horizontal > 0 || horizontalJoystick > 0) && !this.isCrouching && !this.animator.GetBool("CrouchWalking"))
	        {
				//Time left on doubleclick cooldown means this button press is within time frame of a doubleclick.
				//PressedRightAndReleased means button has been pressed once, this press is the doubleclick.
				if (rightDoubleClickCooldown > 0 && pressedRightAndReleased && this.collisionDown) {
					// If dashcooldown is below zero, do a dash, otherwise, do nothing and reset doubleclick cooldown.
					if (rightDashCoolDown <= 0) {
						this.movementController.DashRight ();
						pressedRightAndReleased = false;
						pressedRight = false;
						rightDoubleClickCooldown = 0.0f;
						rightDashCoolDown = 0.7f;
					} else {
						pressedRight = true;
						rightDoubleClickCooldown = 0.0f;
						this.movementController.MoveRight ();
					}
                }
                else 
                {
                    pressedRight = true;
                    rightDoubleClickCooldown = 0.5f;
                    this.movementController.MoveRight ();
                }
	        }
            else if ((horizontal < 0 || horizontalJoystick < 0) && this.isCrouching)
            {
                this.movementController.CrouchLeft();
            }
            else if ((horizontal > 0 || horizontalJoystick > 0) && this.isCrouching)
            {
                this.movementController.CrouchRight();
            }
            else if (horizontal == 0 || horizontalJoystick == 0)
	        {
				this.movementController.Stop ();
	        }
            
            if ((vertical > 0 || verticalJoystick < 0) && !movementController.isKnockedBack())
            {
				bool jumpSucessful = jumpController.Jump ();

				//Only show jump animation if jump is successful.
				if (jumpSucessful) {
					SetAnimatorBool("Crouching", false);
					SetAnimatorBool("Jumping", true);
					this.collisionDown = false;
				}               
                
            }
            else if ((Mathf.Abs(horizontal) > 0 || Mathf.Abs(horizontalJoystick) > 0) && this.collisionDown && !this.isCrouching)
            {
                SetAnimatorBool("Running", true);
                this.isRunning = true;
            }
            else if ((Mathf.Abs(horizontal) > 0 || Mathf.Abs(horizontalJoystick) > 0) && this.collisionDown && this.isCrouching)
            {
                SetAnimatorBool("CrouchWalking", true);
                this.isRunning = true;
            }
            else if (vertical < 0 || verticalJoystick > 0)
            {
                SetAnimatorBool("Crouching", true);
                this.isCrouching = true;
            }

			if (this.collisionDown) // If character touches the ground, it is not jumping.
            {
                SetAnimatorBool("Jumping", false);
                this.jumpController.jumping = false;
            }
            
            if (vertical == 0 && verticalJoystick == 0 && collisionDown) {
                SetAnimatorBool("CrouchWalking", false);
                SetAnimatorBool("Crouching", false);
                this.isCrouching = false;
            }

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(horizontalJoystick) == 0)
            {
                SetAnimatorBool("Running", false);
                SetAnimatorBool("CrouchWalking", false);

                if (pressedLeft) {
                    pressedLeftAndReleased = true;
                    pressedLeft = false;
                }

                if (pressedRight) {
                    pressedRightAndReleased = true;
                    pressedRight = false;
                }
            }

            if ( leftDoubleClickCooldown > 0) {
                leftDoubleClickCooldown -= 1 * Time.deltaTime;
            }
            else 
            {
                pressedLeftAndReleased = false;
                pressedLeft = false;
            }

            if ( rightDoubleClickCooldown > 0) {
                rightDoubleClickCooldown -= 1 * Time.deltaTime;
            }
            else 
            {
                pressedRightAndReleased = false;
                pressedRight = false;
            }

            if (leftDashCoolDown > 0) {
                leftDashCoolDown -= 1 * Time.deltaTime;
            }
            if (rightDashCoolDown > 0) {
                rightDashCoolDown -= 1 * Time.deltaTime;
            }
		}
    }

    public Character GetCharacter()
    {
        return character;
    }

    public Move GetCurretlyPlayedMove()
    {
		return this.fightMoveController.GetCurretlyPlayedMove ();
    }

	/// <summary>
	/// Pauses this instance, freezing all animation and disable buttons.
	/// </summary>
	public void Pause(bool stopAnimator){
		this.movementController.Pause ();
		this.paused = true;
		SetAnimatorEnabled (!stopAnimator);
		fightMoveController.Pause ();
	}

    /// <summary>
    /// Calls the pause method of this class for a set amount of time
    /// </summary>
    public void PauseSeconds(float ms)
    {
        pausedForTime = true;
        Pause(false);
        pauseTime = ms;
    }
    

	/// <summary>
	/// Unpauses this instance, enabling buttons and resuming the animation that is currently playing.
	/// </summary>
	public void UnPause()
	{
		pausedForTime = false;
		animator.SetBool("Stunned", false);
		this.movementController.UnPause ();
		this.paused = false;
		SetAnimatorEnabled (!fightMoveController.IsDoingMove ());
		fightMoveController.UnPause ();
	}

	public void CollisionLeft()
	{
		this.movementController.CollisionLeft ();
	}

	public void CollisionRight()
	{
		this.movementController.CollisionRight ();
	}

    public void CollisionExitLeft()
    {
		this.movementController.CollisionExitLeft ();
    }

    public void CollisionExitRight()
    {
		this.movementController.CollisionExitRight ();
    }

    public void CollisionDown()
    {
        this.collisionDown = true;
    }

    public void KnockBack()
	{
		this.movementController.KnockBack ();
	}
    
    public void SetHitColor()
    {
        foreach(ColorModifier colorModifier in gameObject.GetComponentsInChildren<ColorModifier>())
        {
            colorModifier.SetColor(Color.red);
        }

        StartCoroutine(WaitAndRemoveColor());
    }

    private void RemoveHitColor()
    {
        foreach (ColorModifier colorModifier in gameObject.GetComponentsInChildren<ColorModifier>())
        {
            colorModifier.DeSelect();

        }
    }


    /// <summary>
    /// Wait for a parameterized amount of time before changing the color of the hit character back to the default.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitAndRemoveColor()
    {
        yield return new WaitForSeconds(Parameters.hitColorTime);
        RemoveHitColor();
    }

	private void SetAnimatorEnabled(bool enabled)
	{
		if (this.animator != null) {
			this.animator.enabled = enabled;
		}
	}

	private void SetAnimatorBool(string boolName, bool enabled)
	{
		if (this.animator != null) {
			this.animator.SetBool (boolName, enabled);
		}
	}

}
