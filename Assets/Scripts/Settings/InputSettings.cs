using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information on which buttons are used in the game.
/// Handles registrations of moves to buttons by delegating the registrations to the <see cref="CharacterInput"/> that uses the button to be registrated.
/// </summary>
public static class InputSettings
{
	private static bool alreadyInitialized;
	private static List<CharacterInput> characterInputs = new List<CharacterInput>();

	//All buttons used in the game. Can be itterated over to check if any of them has been pressed.
	public static List<string> allUsedButtons = new List<string>(); //TODO: This is technical debt. REMOVE

	/// <summary>
	/// Create a CharacterInput object for each character in the game.
	/// Read all buttons from the saved settings.
	/// </summary>
	public static void Init()
	{
		if (!alreadyInitialized)
		{
			alreadyInitialized = true;
			//Create a CharacterInput object for each character in the game.
			foreach (Character character in StaticCharacterHolder.characters) {
				CharacterInput characterInput = new CharacterInput (character, 6);
				characterInputs.Add (characterInput);
			}

			LoadButtons (1, "uiojkl");
			LoadButtons (2, "rtyfgh");

		}
	}

	/// <summary>
	/// Loads buttons for the specified character. If the character does not have saved buttons, use the characters in the backup string.
	/// </summary>
	/// <param name="characterNumber">Character number.</param>
	/// <param name="backUpButtons">Back up buttons.</param>
	private static void LoadButtons(int characterNumber, string backUpButtons){
		List<string> playerButtons = SaveLoad.LoadButtons ("/player" + characterNumber + "Buttons.mvs");
		if (playerButtons != null) {
			for (int i = 0; i < playerButtons.Count; i++) {
				AddButton (playerButtons [i] + "", StaticCharacterHolder.characters [characterNumber - 1], i);
			}
		} else {
			for (int i = 0; i < backUpButtons.Length; i++) {
				AddButton (backUpButtons [i] + "", StaticCharacterHolder.characters [characterNumber - 1], i);
			}
		}
	}

	/// <summary>
	/// Add a button to the list of used buttons for the specified character.
	/// </summary>
	/// <param name="button">The button to be added.</param>
	/// <param name="character">The character which is to use the button.</param>
	public static void AddButton(string button, Character character, int index)
	{
		if (!allUsedButtons.Contains (button)) { //Make sure button is not already in use and that it is just one character long.
			allUsedButtons.Add (button);
			foreach (CharacterInput characterInput in characterInputs)
			{
				if (characterInput.GetCharacter ().Equals (character))
				{
					characterInput.SetButton (button, index);
					break;
				}
			}
		}


		foreach (CharacterInput characterInput in characterInputs)
		{
			foreach(string b in characterInput.GetButtons ()){
			}
		}
	}

	public static void RemoveButton(string button)
	{
		if (button != null) {
			allUsedButtons.Remove (button);
			foreach (CharacterInput characterInput in characterInputs) {
				characterInput.RemoveButton (button);
			}
		}
	}

	/// <summary>
	/// Register the move name to the specified button in the CharacterInput object which handles that button.
	/// </summary>
	/// <param name="button">Button.</param>
	/// <param name="moveName">Move name.</param>
	public static Character Register(string button, string moveName)
	{
		foreach (CharacterInput characterInput in characterInputs)
		{
			if (characterInput.HasButton (button))
			{
				characterInput.RegisterButton (button, moveName);
				return characterInput.GetCharacter();
			}
		}
		return null;
	}

	public static void Deregister(string moveName)
	{
		foreach (CharacterInput characterInput in characterInputs)
		{
			characterInput.Deregister (moveName);
		}
	}

	/// <summary>
	/// Checks if the specified character has a move assigned to the specified button.
	/// </summary>
	/// <returns><c>true</c> if the character has the specified button; otherwise, <c>false</c>.</returns>
	/// <param name="characterIndex">The characterIndex to check.</param>
	/// <param name="button">The button that was pressed</param>
	public static bool HasButton(int characterIndex, string button)
	{
		foreach(CharacterInput charInput in characterInputs)
		{
			if (charInput.HasButton(button) && charInput.GetCharacter().GetNbr() == characterIndex)
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// Returns a move linked to the specified button.
	/// </summary>
	/// <returns>The move.</returns>
	/// <param name="button">The pressed button.</param>
	public static string GetMoveName(string button)
	{
		foreach (CharacterInput charInput in characterInputs) 
		{
			if(charInput.HasButton(button))
			{
				return charInput.GetMoveName(button);
			}
		}
		return null;
	}

	/// <summary>
	/// Checks if all buttons have a move assigned to them.
	/// </summary>
	/// <returns><c>true</c>, if each button of every character has a move assigned to it, <c>false</c> otherwise.</returns>
	public static bool AllButtonsAssigned()
	{
		bool allButtonsAssigned = true;
		//Check each character input individually
		foreach (CharacterInput characterInput in characterInputs)
		{
			allButtonsAssigned &= characterInput.AllButtonsAssigned ();
		}
		return allButtonsAssigned;
	}

	public static bool AllButtonsAdded(){
		bool allButtonsAssigned = true;
		//Check each character input individually
		foreach (CharacterInput characterInput in characterInputs)
		{
			allButtonsAssigned &= characterInput.AllButtonsAdded ();
		}
		return allButtonsAssigned;
	}

	/// <summary>
	/// Reset the assigned move of all slots of all characters.
	/// </summary>
	public static void ClearRegisteredMoves()
	{
		foreach (CharacterInput characterInput in characterInputs)
		{
			characterInput.ClearAssignedButtons ();
		}
	}

	/// <summary>
	/// Returns the largest amount of moves any player has.
	/// </summary>
	/// <returns>The maximum number of moves.</returns>
	public static int MaxNrOfMoves()
	{
		int maxNrOfMoves = 0;
		foreach (CharacterInput characterInput in characterInputs)
		{
			if (characterInput.GetNrOfButtons () > maxNrOfMoves) {
				maxNrOfMoves = characterInput.GetNrOfButtons ();
			}
		}
		return maxNrOfMoves;
	}

	/// <summary>
	/// Returns a list of all buttons used by the specified character.
	/// </summary>
	/// <returns>The character buttons.</returns>
	/// <param name="characterNumber">Character number.</param>
	public static List<string> GetCharacterButtons(int characterNumber){
		return characterInputs [characterNumber - 1].GetButtons (); // -1 to get from [1-2] -> [0-1].
	}

	/// <summary>
	/// Returns the button of a character in the slot at the specified index.
	/// </summary>
	/// <returns>The character button.</returns>
	/// <param name="characterNumber">Character number.</param>
	/// <param name="index">Index of the slot to get button from.</param>
	public static string GetCharacterButton(int characterNumber, int index)
	{
		return characterInputs [characterNumber - 1].getButton (index); // -1 to get from [1-2] -> [0-1].
	}
}
