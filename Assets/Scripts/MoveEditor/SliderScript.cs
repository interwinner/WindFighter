using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class containing functionality and help functions for controlling the slider
/// in the moveeditor.
/// </summary>
public class SliderScript : MonoBehaviour
{
	private Text strengthText;
	private Text speedText;

	private Slider speedStrengthSlider;

    private int strengthValue;
    private int speedValue;

	private string topSliderString;
	private string botSliderString;

    void Start ()
    {
		speedStrengthSlider = GameObject.Find("SpeedStrengthSlider").GetComponent<Slider>();

		strengthText = GameObject.Find ("StrengthText").GetComponent<Text>();
		speedText = GameObject.Find("SpeedText").GetComponent<Text>();

		UpdateSpeedValue (50);

		SetSliderStrings ("Strength", "Speed");
	}


    /// <summary>
    /// When the value of the slider changes, update the value of the text fields.
    /// </summary>
    /// <param name="newValue">The new value of the speedslider</param>
    public void UpdateSpeedValue(float newValue)
    {
        speedValue = (int)newValue;
		strengthValue = 100 - speedValue;

		speedText.text = botSliderString + ":" + speedValue.ToString();
		strengthText.text = topSliderString + ":" + strengthValue.ToString();
    }


    /// <summary>
    /// Disables the GameObject holding the slider
    /// </summary>
    public void DisableSliders()
    {
        gameObject.SetActive(false);
    }

    public void EnableSliders()
    {
        gameObject.SetActive(false);
    }


    public int GetStrength()
    {
        return strengthValue;
    }

    public int GetSpeed()
    {
        return speedValue;
    }

	/// <summary>
	/// Reset slider to 50/50 strength/speed.
	/// </summary>
	public void ResetSlider()
	{
		UpdateSpeedValue (50); //Automatically updates strength as well.
		speedStrengthSlider.value = speedValue;
	}

	public void SetSliderStrings(string top, string bot)
	{
		this.topSliderString = top;
		this.botSliderString = bot;
		strengthText.text = topSliderString + ": " + strengthValue.ToString();
		speedText.text = botSliderString + ": " + speedValue.ToString();
	}
}
