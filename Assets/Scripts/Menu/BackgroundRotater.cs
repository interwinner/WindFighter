using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundRotater : MonoBehaviour {

    private int frameCountdown = 20;
    private int currentImage = 1;
    private string[] images;
    private Image imageObject;

    // Use this for initialization
    void Start() {
        images = new string[] {"horizon1", "horizon2", "horizon3", "horizon4", "horizon5", "horizon6", "horizon7", "horizon8"};
        imageObject = gameObject.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        frameCountdown--;
        if (frameCountdown == 0)
        {
            currentImage += 1;
            currentImage %= images.Length - 1;
            frameCountdown = 20;
            imageObject.sprite = Resources.Load <Sprite> ("Art/Main Menu/" + images[currentImage]);
        }
	}
}
