using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsReturnScript : MonoBehaviour {

	private GameObject enterAllButtonsText;
	private bool allButtonsAdded;

	void Start()
	{
		this.enterAllButtonsText = GameObject.Find ("EnterAllButtonsText");
		enterAllButtonsText.SetActive (false);
	}

	void Update ()
	{
		bool escapePressed = Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown("Controller1Button6") || Input.GetButtonDown("Controller2Button6");
		allButtonsAdded = InputSettings.AllButtonsAdded ();

		if (allButtonsAdded)
		{
			enterAllButtonsText.SetActive (false);
		}

		if (escapePressed)
		{
			WarningOrGoBack ();
		}
	}

	public void WarningOrGoBack()
	{
		if (allButtonsAdded)
		{
			//Save buttons to file.
			List<string> player1Buttons = InputSettings.GetCharacterButtons (1);
			List<string> player2Buttons = InputSettings.GetCharacterButtons (2);
			SaveLoad.SaveButtons (player1Buttons, "/player1Buttons.mvs");
			SaveLoad.SaveButtons (player2Buttons, "/player2Buttons.mvs");

			GoBack ();
		}
		else
		{
			enterAllButtonsText.SetActive (true);
		}
	}

	public void GoBack()
	{
		SceneHandler.GoBack ();
	}
}
