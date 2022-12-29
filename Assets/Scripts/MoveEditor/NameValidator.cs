using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NameValidator : MonoBehaviour
{
	private GameObject alreadyUsedText;
	private InputField inputField;
	private string name;
	private bool validName;

	void Start ()
	{
		alreadyUsedText = transform.Find ("AlreadyUsedText").gameObject;
		alreadyUsedText.SetActive (false);
		inputField = transform.Find ("NameInputField").gameObject.GetComponent<InputField> ();
		inputField.text = GenerateValidName ();
	}


	/// <summary>
	/// Generates a valid name. Names take the form Move0, Move1, etc. The first one that is not used is returned.
	/// </summary>
	/// <returns>A valid name.</returns>
	public string GenerateValidName()
	{
		//Start from 0 and go to the highest possible value. Break loop and return as soon as a valid name is found.
		for (int i = 0; i < Int32.MaxValue; i++)
		{
			string name = "Move" + i;
			if (!AvailableMoves.ContainsName (name))
			{
				return name;
			}
		}
		return "NO_VALID_NAME";
	}

	public void ValidateName()
	{
		name = inputField.text;
		validName = !AvailableMoves.ContainsName (name);
		validName &= !name.Equals (string.Empty);
		alreadyUsedText.SetActive (AvailableMoves.ContainsName (name));
	}

	public bool IsNameValid()
	{
		return validName;
	}

	public string GetName()
	{
		return this.name;
	}
}
