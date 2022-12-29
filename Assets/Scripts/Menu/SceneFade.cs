using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFade : MonoBehaviour {

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1; 

    void OnGUI()
    {
        // Fade the alpha using direction, speed and deltatime to convert to seconds
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        // Clamp number between 0 and 1 because GUI.color uses alpha values between 0 and 1
        alpha = Mathf.Clamp01(alpha);
        // Change the alpha of the GUI.
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    // Call fading using direction (-1 to fade in, 1 to fade out)
    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return fadeSpeed;
    }

    /// <summary>
    /// Called when new scene is loaded. 
    /// </summary>
    void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }
}
