using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information on the buttons used to controll a character.
/// Handles registration of moves to each button.
/// </summary>
public class CharacterInput
{
	private Character character;
	private KeyValuePair<string,string>[] buttonMovePairs;

	/// <summary>
	/// Initializes a new instance of the <see cref="CharacterInput"/> class.
	/// </summary>
	/// <param name="character">Character.</param>
	/// <param name="size">The number of slots that links a button to a move.</param>
	public CharacterInput(Character character, int size)
	{
		this.buttonMovePairs = new KeyValuePair<string, string>[size];
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			buttonMovePairs [i] = new KeyValuePair<string,string> ("", "");
		}
		this.character = character;
	}

	/// <summary>
	/// Sets the button of a slot at a specified index.
	/// Button has to be of length 1.
	/// Will throw an exception if index is too large or negative.
	/// </summary>
	/// <param name="index">The index of the slot the button should be registered to.</param>
	/// <param name="button">The button to be registered.</param>
	public void SetButton(string button, int index)
	{
		SetKey (index, button);
	}

	/// <summary>
	/// Removes a button from the slot it is registered to if it is currently in use.
	/// If button is not in use, nothing happens.s
	/// </summary>
	/// <param name="button">The button to be removed.</param>
	public void RemoveButton(string button)
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Key.Equals (button))
			{
				SetKey (i, "");
			}
		}
	}

	/// <summary>
	/// Register the name of a move to a button used by the character.
	/// </summary>
	/// <param name="button">Button.</param>
	/// <param name="moveName">Move name.</param>
	public void RegisterButton(string button, string moveName)
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Value.Equals (moveName)) {
				SetValue (i, "");
			}

			if (buttonMovePairs [i].Key.Equals (button)) {
				SetValue (i, moveName);
			}
		}
	}

	/// <summary>
	/// Determines whether this instance has the specified button.
	/// </summary>
	/// <returns><c>true</c> if this instance has the specified button; otherwise, <c>false</c>.</returns>
	/// <param name="button">The button.</param>
	public bool HasButton(string button)
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Key.Equals (button)) {
				return true;
			}
		}
		return false;
	}

	public void SetCharacter(Character character)
	{
		this.character = character;
	}

	public Character GetCharacter()
	{
		return this.character;
	}

	/// <summary>
	/// Returns the move registered to the specified button.
	/// </summary>
	/// <returns>The move name.</returns>
	/// <param name="button">Button.</param>
	public string GetMoveName(string button)
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Key.Equals (button)) {
				return buttonMovePairs [i].Value;
			}
		}
		return null;
	}

	/// <summary>
	/// Clears the assigned button of every slot.
	/// </summary>
	public void ClearAssignedButtons()
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			SetValue (i, "");
		}
	}

	public void Deregister(string moveName){
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Value.Equals (moveName)) {
				SetValue (i, "");
			}
		}
	}

	/// <summary>
	/// Checks if all buttons have a move assigned.
	/// </summary>
	/// <returns><c>true</c>, if the number of assigned moves is the same as the total number of used buttons, <c>false</c> otherwise.</returns>
	public bool AllButtonsAssigned()
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Value == null || buttonMovePairs [i].Value.Equals ("")) {
				return false;
			}
		}
		return true;
	}

	public bool AllButtonsAdded()
	{
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			if (buttonMovePairs [i].Key == null || buttonMovePairs [i].Key.Equals ("")) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Returns the number of buttons used by this character.
	/// </summary>
	/// <returns>The nr of buttons.</returns>
	public int GetNrOfButtons()
	{
		return this.buttonMovePairs.Length;
	}

	/// <summary>
	/// Returns all buttons used by this character.
	/// </summary>
	/// <returns>The buttons.</returns>
	public List<string> GetButtons(){
		List<string> buttons = new List<string> ();
		for (int i = 0; i < buttonMovePairs.Length; i++) {
			buttons.Add (buttonMovePairs [i].Key);
		}
		return buttons;
	}

	public string getButton(int index){
		return buttonMovePairs [index].Key;
	}

	/// <summary>
	/// Set the value of a keyvaluepair at a certain index.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="value">Value.</param>
	private void SetValue(int index, string value){
		string oldKey = buttonMovePairs [index].Key;
		KeyValuePair<string,string> newPair = new KeyValuePair<string,string> (oldKey,value);
		buttonMovePairs [index] = newPair;
	}

	/// <summary>
	/// Set the key of a keyvaluepair at a certain index.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="key">Key.</param>
	private void SetKey(int index, string key){
		string oldValue = buttonMovePairs [index].Value;
		KeyValuePair<string,string> newPair = new KeyValuePair<string,string> (key,oldValue);
		buttonMovePairs [index] = newPair;
	}
}