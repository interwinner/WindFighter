using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to highlight a bodypart when it is selected.
/// </summary>
public class ActiveBodypartSelector : MonoBehaviour {

	private string activeBodypart;
	private bool blockMove;
	private GameObject headToggleObject;

	void Awake()
	{
		headToggleObject = GameObject.Find ("HeadToggleButton");
	}

	/// <summary>
	/// When a new bodypart is selected, deselect the currrent active bodypart on the character and select the new one.
	/// If move is a block, show the shield.
	/// </summary>
	/// <param name="newBodypartName">New bodypart name.</param>
	public void BodypartChanged(string newBodypartName)
	{
		if (newBodypartName.Equals (activeBodypart))
		{
			return; //Do nothing if the mouse moves out and back in of the same label.
		}
		if (activeBodypart != null)
		{
			GameObject.Find (activeBodypart).GetComponent<ColorModifier> ().DeSelect (); //Deselect any previous attacking bodypart.
			GameObject previousShield = GameObject.Find (activeBodypart.Replace (" ", "") + "Shield"); //Shields use camel case naming without spaces.
			if (previousShield != null)
			{
				previousShield.GetComponent<SpriteRenderer> ().enabled = false; //Hide any previous shield.
			}
		}
		if (blockMove)
		{
			string noSpaceText = newBodypartName.Replace (" ", ""); //Shields use camel case naming without spaces.
			GameObject newShield = GameObject.Find (noSpaceText + "Shield");
			if (newShield != null)
			{
				newShield.GetComponent<SpriteRenderer> ().enabled = true; //Show the new shield.
				//Keep the scale.
				Vector3 shieldScale = GameObject.Find (activeBodypart.Replace (" ", "") + "Shield").transform.localScale;
				newShield.transform.localScale = shieldScale;
			}
		}
		else
		{ //If move is not a block move, change color of attacking bodypart.
			GameObject.Find (newBodypartName).GetComponent<ColorModifier> ().Select ();
		}
		activeBodypart = newBodypartName;
	}

	public string GetActiveBodypart()
	{
		return this.activeBodypart;
	}

	public void SetBlockMove(bool blockMove)
	{
		this.blockMove = blockMove;
	}

	public void Reset(){
		//Deactivate all buttons that are not the head. Head is default.
		Transform buttonHolder = transform.Find("Panel").Find("Buttons");
		foreach (Transform child in buttonHolder) {
			if (!child.name.Equals ("HeadToggleButton")) {
				Toggle toggle = child.GetComponent<Toggle> ();
				if (toggle != null) {
					toggle.isOn = false;
				}
			}
		}
		headToggleObject.GetComponent<Toggle> ().isOn = true; //Activate head. Head is default.
	}
}
