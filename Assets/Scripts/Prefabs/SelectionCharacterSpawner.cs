using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCharacterSpawner : MonoBehaviour
{

  //  public float x1, x2, y1, y2;

    void Start()
    {
        SetCharacter2Color();
      //  SpawnCharacters();
    }

    public void SetCharacter2Color() 
    {
        GameObject character2 = GameObject.Find("Character 2");

        Object[] blueSprites = Resources.LoadAll("Art/newguy 1_blue");

        UnityUtils.RecursiveFind(character2.transform, "Torso").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [6];
		UnityUtils.RecursiveFind(character2.transform, "Head").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [2];
		UnityUtils.RecursiveFind(character2.transform, "Right Hand").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [3];
		UnityUtils.RecursiveFind(character2.transform, "Left Hand").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [5];
    }
/* 
    public void SpawnCharacters()
    {
        GameObject character1 = Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;
        GameObject character2 = Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;

        Object[] blueSprites = Resources.LoadAll("Art/newguy 1_blue");
        print(blueSprites);
        print(UnityUtils.RecursiveFind(character2.transform, "Torso"));
        UnityUtils.RecursiveFind(character2.transform, "Torso").GetComponent<SpriteRenderer>().sprite = (Sprite)blueSprites [5];

        character1.transform.name = "Character 1";
        character2.transform.name = "Character 2";

        character1.SetActive(true);
        character2.SetActive(true);

        Destroy(character1.GetComponent<Rigidbody2D>());
        Destroy(character2.GetComponent<Rigidbody2D>());

        character1.transform.position = new Vector3(x1, y1, 0);
        character2.transform.position = new Vector3(x2, y2, 0);
        character2.transform.localScale = new Vector3(-character2.transform.localScale.x, character2.transform.localScale.y, character2.transform.localScale.z);

        List<GameObject> char1DragPoints = UnityUtils.RecursiveContains(character1.transform, "DragPoint");
        List<GameObject> char2DragPoints = UnityUtils.RecursiveContains(character2.transform, "DragPoint");

        foreach(GameObject child in char1DragPoints)
        {
            Destroy(child);
        }
        foreach (GameObject child in char2DragPoints)
        {
            Destroy(child);
        }
    }   
*/
}
