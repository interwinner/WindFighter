using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script attached to the text telling the user to press a button.
/// </summary>
public class EnterButtonScript : MonoBehaviour {

	private int currentIndex;
	private int currentCharacterNumber;
	private string availableKeys = "qertyuiopfghjklzxcvbnm,.-1234567890"; //All keyboard keys that are not used for movement.

	private string[] availableControllerButtons = new string[]{
		"Controller1Button0", "Controller1Button1", "Controller1Button2", "Controller1Button3",
		"Controller1Button4", "Controller1Button5", "Controller1LeftTrigger", "Controller1RightTrigger",
		"Controller2Button0", "Controller2Button1", "Controller2Button2", "Controller2Button3",
		"Controller2Button4", "Controller2Button5", "Controller2LeftTrigger", "Controller2RightTrigger",
	};
	private InputGuiManager inputGuiManager;

	void Update () {
		for (int i = 0; i < availableKeys.Length; i++) {
			if (Input.GetKeyDown ("" + availableKeys [i])) {
				string newButton = "" + availableKeys [i];
				InputSettings.RemoveButton (newButton); //Remove button where it is currently used so that it is only used once.
				InputSettings.AddButton (newButton, StaticCharacterHolder.characters [currentCharacterNumber - 1], currentIndex); //Add button in its new place.
				this.inputGuiManager.UpdateGUI (); //Update gui to match current settings.
				gameObject.SetActive (false); //Hide text telling user to press a key.
			}
		}
		foreach(string button in availableControllerButtons){
			if(Input.GetButtonDown(button)){
				InputSettings.RemoveButton (button); //Remove button where it is currently used so that it is only used once.
				InputSettings.AddButton (button, StaticCharacterHolder.characters [currentCharacterNumber - 1], currentIndex); //Add button in its new place.
				this.inputGuiManager.UpdateGUI (); //Update gui to match current settings.
				gameObject.SetActive (false); //Hide text telling user to press a key.
			}
		}
	}

	public void SetCurrentIndex(int currentIndex){
		this.currentIndex = currentIndex;
		UpdateText ();
	}

	public void SetCurrentCharacter(int currentCharacter){
		this.currentCharacterNumber = currentCharacter;
		UpdateText ();
	}

	public void SetInputGuiManager(InputGuiManager inputGuiManager){
		this.inputGuiManager = inputGuiManager;
	}

	private  void UpdateText()
	{
		this.GetComponent<Text> ().text = "Enter move " + currentIndex + " button\nfor player " + currentCharacterNumber;
	}
}
