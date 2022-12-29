using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour {

	private float height;
	private bool floating; // Floating == true means the shadow follows the character's feet. Floating == false means it is fixed at the specified height.
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		this.spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		this.floating = true;
	}

	void Update ()
	{
		if (!floating && (Mathf.Abs (transform.position.y - height) > 0.01f)) //Corect shadow height if it is too far from ground.
		{
			float x = transform.position.x;
			float z = transform.position.z;
			Vector3 newPosition = new Vector3 (x, height, z);
			transform.position = newPosition;
		}

		// Make sure shadow is not moved to front when character performs a move. Shadows should always be behind both characters.
		if (!spriteRenderer.sortingLayerName.Equals ("Character")) {
			spriteRenderer.sortingLayerName = "Character";
		}
	}

	public void SetHeight(float height)
	{
		this.height = height;
		this.floating = false; // When a height is specified, we assume the shadow is to stay at that height.
	}
	 
	public void SetFloating(bool floating){
		this.floating = floating;
	}
}
