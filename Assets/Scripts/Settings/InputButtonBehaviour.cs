using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour of buttons for selecting player input of a specific move.
/// </summary>
public class InputButtonBehaviour : MonoBehaviour {

	private GameObject enterButtonText; //The text saying "Press button for move of player" when waiting for the user to press a button.
	private EnterButtonScript enterButtonScript;
	private int characterNumber;
	private int index; //The index of the move. The index is the same in the list and in the slots of the chracter move set.

	/// <summary>
	/// Called when the button is pressed. Activates the text saying to press a button and the script which  registers the next key stroke.
	/// </summary>
	public void SetInputOfMove(){
		if (enterButtonText != null && enterButtonScript != null)
		{
			enterButtonScript.SetCurrentIndex (index);
			enterButtonScript.SetCurrentCharacter (characterNumber);
			enterButtonText.SetActive (true);
		}
	}

	public void SetEnterButtonText(GameObject enterButtonText){
		if (enterButtonText != null) {
			this.enterButtonText = enterButtonText;
			this.enterButtonScript = enterButtonText.GetComponent<EnterButtonScript> ();
		}
	}

	public void SetIndex(int index){
		this.index = index;
	}

	public void SetCharacterNumber(int characterNumber){
		this.characterNumber = characterNumber;
	}
}
