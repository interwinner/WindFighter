using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlacementHandler : MonoBehaviour {

	void Awake()
	{
		//Make characters start at a certain percentage of screen size horizontally and vertically instead of fixed positions
		Camera gameCamera = GameObject.FindObjectOfType<Camera> ();
		float camHeight = gameCamera.orthographicSize;
		float camWidth = camHeight * gameCamera.aspect;
		GameObject character1 = GameObject.Find ("Character 1");
		GameObject character2 = GameObject.Find ("Character 2");
		character1.transform.position = new Vector3 (camWidth * -0.325f, camHeight * 0.35f, 0f);
		character2.transform.position = new Vector3 (camWidth * 0.325f, camHeight * 0.35f, 0f);
	}
}
