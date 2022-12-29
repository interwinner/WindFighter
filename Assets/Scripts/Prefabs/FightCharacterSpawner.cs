using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCharacterSpawner : MonoBehaviour
{

	public float x1, x2, y1, y2; // (x1,y1) is position of character 1. (x2,y2) is position of character 2. 

	void Start()
	{
		SpawnCharacters();
	}



	public void SpawnCharacters()
	{
		GameObject character1 = Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;
		GameObject character2 = Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;

		Object[] blueSprites = Resources.LoadAll("Art/newguy 1_blue");

        UnityUtils.RecursiveFind(character2.transform, "Torso").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [6];
		UnityUtils.RecursiveFind(character2.transform, "Head").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [2];
		UnityUtils.RecursiveFind(character2.transform, "Right Hand").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [3];
		UnityUtils.RecursiveFind(character2.transform, "Left Hand").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [5];

		character1.transform.localScale = new Vector3 (character1.transform.localScale.x, character1.transform.localScale.y, character1.transform.localScale.z);
		// - to change direction of character 2.
		character2.transform.localScale = new Vector3 (-character2.transform.localScale.x, character2.transform.localScale.y, character2.transform.localScale.z);

		AndAndRemoveComponents (character1, 1);
		AndAndRemoveComponents (character2, 2);

        GameObject.Find("Main Camera").GetComponent<CameraZoomControl>().character1 = character1;
        GameObject.Find("Main Camera").GetComponent<CameraZoomControl>().character2 = character2;
        character2.GetComponent<InputController>().horizontalAxis = "Horizontal2";
		character2.GetComponent<InputController>().verticalAxis = "Vertical2";

		character2.GetComponent<InputController>().horizontalAxisJoystick = "HorizontalJoystick2";
		character2.GetComponent<InputController>().verticalAxisJoystick = "VerticalJoystick2";

        character2.GetComponent<BoxCollider2D>().enabled = true;
        character2.GetComponent<BoxCollider2D>().enabled = true;

        character1.transform.Find("LowerBodyHitbox").GetComponent<BoxCollider2D>().enabled = true;
        character2.transform.Find("LowerBodyHitbox").GetComponent<BoxCollider2D>().enabled = true;

        character1.transform.position = new Vector3(x1, y1, 0);
		character2.transform.position = new Vector3(x2, y2, 0);

		GameObject ground = GameObject.Find ("Ground");
		BoxCollider2D groundCollider = ground.GetComponent<BoxCollider2D> ();
		float groundPosition = groundCollider.bounds.max.y; // Shadows should be at top of ground.
		character1.transform.Find ("undercharacter").GetComponent<ShadowScript> ().SetHeight (groundPosition);
		character2.transform.Find ("undercharacter").GetComponent<ShadowScript> ().SetHeight (groundPosition);
	}

	private void AndAndRemoveComponents(GameObject characterObject, int index)
	{
		Transform characterTransform = characterObject.transform;
		characterObject.transform.name = "Character " + index;

        characterObject.GetComponent<Animator>().enabled = true;
		characterObject.GetComponent<JumpController> ().enabled = true;
		characterObject.GetComponent<MovementController> ().enabled = true;
		characterObject.GetComponent<FightMoveController> ().enabled = true;
        characterObject.GetComponent<InputController>().enabled = true;
        characterObject.GetComponent<Rigidbody2D>().simulated = true;

		//Turn on hitbox colliders (character = legs & torso = upper body)
        characterObject.GetComponent<BoxCollider2D>().enabled = true;
        Transform torsoTransform = characterTransform.Find("Torso");
        torsoTransform.GetComponent<BoxCollider2D>().enabled = true;

        characterObject.GetComponent<CharacterCollisionDetector>().enabled = true;
        characterObject.transform.Find("LowerBodyHitbox").GetComponent<DamageTriggerDetector>().enabled = true;

        characterObject.GetComponent<InputController>().characterIndex = index;
        
        foreach (Rigidbody2D body in characterObject.GetComponentsInChildren<Rigidbody2D>())
        {
			body.simulated = true; //Turn on rigidbody simlutaion (No "enabled" bool in rigidbody).
        }

        foreach (DamageTriggerDetector damageDetector in characterObject.GetComponentsInChildren<DamageTriggerDetector>())
		{
			damageDetector.enabled = true;
            damageDetector.characterIndex = index; //Make sure no damage is dealt on collision with self.
        }
    }
}
