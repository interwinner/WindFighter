using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorCharacterSpawner : MonoBehaviour
{

	public float x1;
	public float y1;
	public float xScale;
	public float yScale;

    private MoveCreator moveCreator;

    /// <summary>
    /// Method to spawn a version of the character prefab with the necessary components to record moves in the editor scene.
    /// See google document for more information about what parts should be active in the edtior scene.
    /// </summary>
    public void SpawnCharacter()
    {
        GameObject preInitCharacter = Resources.Load("Prefabs/Character", typeof (GameObject)) as GameObject;
        GameObject character = Instantiate(preInitCharacter);

        character.transform.name = "Character";
		moveCreator = gameObject.GetComponent<MoveCreator>();
        moveCreator.enabled = true;
        
        Destroy(character.GetComponent<Rigidbody2D>());

        // Destroy all rigid bodies since dragpoints does not work with them attached.
        foreach (Transform child in character.GetComponentsInChildren<Transform>(true))
        {
            Destroy(child.transform.GetComponent<Rigidbody2D>());
        }

		character.transform.position = new Vector3(x1, y1, 0);
		character.transform.localScale = new Vector3 (xScale, yScale, 1f);

		SpawnOnionCharacter();

		moveCreator.CharacterSpawned ();
    }

    public void SpawnOnionCharacter()
    {
        GameObject preInitCharacter = Resources.Load("Prefabs/Character", typeof(GameObject)) as GameObject;
        GameObject character = Instantiate(preInitCharacter);

        foreach (SpriteRenderer sprite in character.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.sortingOrder = 0;
            sprite.color = new Color(0, 0, 0, 0.5f);
        }

        character.transform.name = "Onion Character";

        Destroy(character.GetComponent<Rigidbody2D>());

        // Destroy all rigid bodies since dragpoints does not work with them attached.
        foreach (Transform child in character.GetComponentsInChildren<Transform>(true))
        {
            Destroy(child.transform.GetComponent<Rigidbody2D>());
        }
		character.transform.Find ("undercharacter").gameObject.SetActive (false); // Remove shadow of character onion skin.
        character.transform.position = new Vector3(x1, y1, 0);
        character.transform.localScale = new Vector3 (xScale, yScale, 1f);
    }
}
