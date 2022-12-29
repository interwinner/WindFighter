using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class with method to call the SceneHandler and load the previous scene.
/// If a promt is to be displayed before going back, it can be set in the editor.
/// </summary>
public class SceneReturnScript : MonoBehaviour 
{
	public GameObject promptPanel; //Set in editor if exists for the current scene. Leave null otherwise.
	public bool promptInsteadOfReturn = true;

	void Start()
	{
		if (promptPanel != null)
		{
			promptPanel.SetActive (false);
		}
	}

	void Update () 
	{
		//Press Escape to go back
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown("Controller1Button6") || Input.GetButtonDown("Controller2Button6")) 
		{
			PromptOrGoBack ();
		}
	}

	public void PromptOrGoBack()
	{
		if (promptPanel == null || !promptInsteadOfReturn) 
		{
			GoBackOrHidePrompt ();
		}
		else
		{
			promptPanel.SetActive (!promptPanel.activeSelf); //Hide panel again if pressing escape while it is active.
			// If prompt is active, select a button in it to enable keyboard navigation.
			if (promptPanel.activeSelf) {
				Button button = promptPanel.GetComponentInChildren<Button> ();
				if (button != null) {
					button.Select ();
				}
			} else {
				//If panel is hidden, make sure a selectable in the scene is reselected to enable keyboard navigation.
				Selectable selectable = GameObject.FindObjectOfType<Selectable> ();
				if (selectable != null) {
					selectable.Select ();
				}
			}
		}
	}

	public void GoBackOrHidePrompt()
	{
		if (promptPanel != null) {
			if (!promptPanel.activeSelf) {
				GoBack ();
			} else {
				HidePrompt ();
			}
		}
	}

	public void GoBack()
	{
		HidePrompt ();
		SceneHandler.GoBack ();
	}

	public void HidePrompt()
	{
		if (promptPanel != null)
		{
			promptPanel.SetActive (false);
			//If panel is hidden, make sure a selectable in the scene is reselected to enable keyboard navigation.
			Selectable selectable = GameObject.FindObjectOfType<Selectable> ();
			if (selectable != null) {
				selectable.Select ();
			}
		}
	}

	public void SetPromptEnabled(bool promptEnabled)
	{
		this.promptInsteadOfReturn = promptEnabled;
	}
}
