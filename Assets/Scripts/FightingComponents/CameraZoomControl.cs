using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraZoomControl : MonoBehaviour {

    public GameObject character1;
    public GameObject character2;
    private float leftCharPos;
    private float rightCharPos;
    private float extraCanvasSpace = 5.6f;
    private float minCharDistance;
    private float startHeight;
    private Camera gameCamera;
	private float backgroundPosLeftX;
	private float backgroundPosRightX;

	void Start () {
        gameCamera = GetComponent<Camera>();
        startHeight = gameCamera.orthographicSize;
    }


	void Update () {

        //TODO: Check case where characters switch places.
		//TODO: Give camera a speed so there is a delay to the scaling and it always scale at a constant speed.
		//TODO: Find edge of screen dynmiaclly instead of with magic numbers.

		//edge of screen in world coordinates
		backgroundPosLeftX = -8.9f;
		backgroundPosRightX = 8.9f;

		leftCharPos = character1.transform.position.x - Parameters.characterCamMargin;
		rightCharPos = character2.transform.position.x + Parameters.characterCamMargin;

		//Check which of character or edge of background is closest to the center (camera bound)

        // Calculate offsets with which to move the camera from the edges when both characters are close to each other
        float leftOffset = 0;
        float rightOffset = 0;
        if (leftCharPos <= backgroundPosLeftX)
        {
            rightOffset = backgroundPosLeftX - leftCharPos;
        }

        if (rightCharPos >= backgroundPosRightX)
        {
            leftOffset = rightCharPos - backgroundPosRightX;
        }

        float leftX = Math.Max(backgroundPosLeftX, leftCharPos - leftOffset);
        float rightX = Math.Min(backgroundPosRightX, rightCharPos + rightOffset);

        float middleX = leftX + (rightX - leftX) / 2f;
		gameCamera.orthographicSize = ((rightX - leftX) / gameCamera.aspect) / 2f; //orthographicSize is halv of screen height.
		float camPosY = gameCamera.orthographicSize - startHeight; //Make camera lower edge always be at ground level.
		Vector3 camPos = new Vector3 (middleX, camPosY, transform.position.z);
		transform.position = camPos; //Update camera object's position.
    }
}
