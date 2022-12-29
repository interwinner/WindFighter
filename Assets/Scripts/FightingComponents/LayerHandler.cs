using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHandler : MonoBehaviour {

    private GameObject character;
    private string movePlayingLayer = "MovePlaying";
    private string characterLayer = "Character";
    private const int ORDER_NR = 0;

    /// <summary>
    /// Class for handling changing layers of sprite in the Fight Scene.
    /// </summary>
    public void sendToCharacterLayer(GameObject go)
    {
        if (!go.Equals(character) && character != null)
        {
            foreach (SpriteRenderer sprite in character.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.sortingLayerName = characterLayer;
            }
        }
        character = go;
        foreach (SpriteRenderer sprite in go.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.sortingLayerName = movePlayingLayer;
        }

    }
}
