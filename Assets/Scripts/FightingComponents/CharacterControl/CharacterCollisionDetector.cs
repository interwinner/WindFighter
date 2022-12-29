using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects collisions occuring when a character moves, making it unable to push things around.
/// </summary>
public class CharacterCollisionDetector : MonoBehaviour
{
	private InputController inputController;
    private AudioSource audioCenter;
    private int characterPlaced = 3;

    void Start ()
	{
		this.inputController = GetComponent<InputController> ();
        audioCenter = GameObject.Find("AudioCenter").GetComponent<AudioSource>();
    }
		
	//Detect collisions with other non-trigger colliders.
	void OnCollisionEnter2D(Collision2D collision)
	{
		Collider2D otherCollider = collision.collider; //The collider the object this script is attached to collides with.
		GameObject otherCharacterObject = otherCollider.gameObject;
		InputController otherInputController = otherCharacterObject.GetComponent<InputController> ();

        if (otherCollider.transform.tag.Equals("Floor"))
        {
            if (characterPlaced == 0) {
                audioCenter.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/impact");
               // audioCenter.Play(); 
            }
            if (characterPlaced > 0 ) characterPlaced--;
            
            
            inputController.CollisionDown();
        }

        if (otherInputController == null)
		{
			return; //If colliding object's root does not have an InputController, it is not a character.
		}
		//Tell this inputcontroller something is blocking the way in the right direction.
		if (otherCollider.transform.position.x < transform.position.x)
		{
			inputController.CollisionLeft ();
		}
		else
		{
			inputController.CollisionRight ();
		}
	}

    void OnCollisionExit2D(Collision2D collision)
    {
        Collider2D otherCollider = collision.collider; //The collider the object this script is attached to collides with.
        GameObject otherCharacterObject = otherCollider.gameObject;
        InputController otherInputController = otherCharacterObject.GetComponent<InputController>();
        if (otherInputController == null)
        {
            return; //If colliding object's root does not have an InputController, it is not a character.
        }
        //Tell this inputcontroller something is blocking the way in the right direction.
        if (otherCollider.transform.position.x < transform.position.x)
        {
            inputController.CollisionExitLeft();
        }
        else
        {
            inputController.CollisionExitRight();
        }
    }
}
