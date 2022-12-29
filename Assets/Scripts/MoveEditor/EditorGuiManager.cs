using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorGuiManager : MonoBehaviour {

	private GameObject[] statePanels;
	private int currentStateIndex;
	ProgressBarBehaviour phaseProgressBar;
	private SceneReturnScript returnScript;

	void Start ()
	{
		GameObject moveTypePanel = GameObject.Find ("MoveTypePanel");
		GameObject slidersPanel = GameObject.Find ("SlidersPanel");
		GameObject activeBodypartPanel = GameObject.Find ("ActiveBodypartPanel");
		GameObject recordFramesPanel = GameObject.Find ("RecordFramesPanel");
		GameObject nameAndSavePanel = GameObject.Find ("NameAndSavePanel");
		phaseProgressBar = GameObject.Find ("PhaseProgressBar").GetComponent<ProgressBarBehaviour> ();
		statePanels = new GameObject[] {
			moveTypePanel,
			activeBodypartPanel,
			recordFramesPanel,
			slidersPanel,
			nameAndSavePanel
		};
		this.returnScript = GameObject.FindObjectOfType<SceneReturnScript> ();
	}

	/// <summary>
	/// Init this instance. This is needed because other classes need to register the components of several phases.
	/// If this class hides those components before the other classes can access them, errors are produced.
	/// </summary>
	public void Init()
	{
		this.returnScript.SetPromptEnabled (false); //Don't ask to go back while editing a move if move type hasn't been selected.
		currentStateIndex = 0;
		ShowOnlyCurrentPanel ();
		UpdatePhaseProgressBar ();
	}

	/// <summary>
	/// Shows the only panel related to the current phase.
	/// </summary>
	private void ShowOnlyCurrentPanel()
	{
		for (int i = 0; i < statePanels.Length; i++)
		{
			bool isCurrentPanel = (i == currentStateIndex);
			statePanels [i].SetActive (isCurrentPanel);

			//Select one component of the phase panel to enable keyboard navigation.
			ListKeyboardManager keyBoardEnable = statePanels [i].GetComponent<ListKeyboardManager> ();
			if (keyBoardEnable != null && isCurrentPanel) {
				keyBoardEnable.Activate ();
			}
		}
	}

	/// <summary>
	/// Moves to the next state if not already at the last state.
	/// Hides all components except the ones used in the new state.
	/// </summary>
	public void NextState()
	{
		if (currentStateIndex < statePanels.Length - 1)
		{
			currentStateIndex++;
		}
		ShowOnlyCurrentPanel ();
		UpdatePhaseProgressBar ();
		this.returnScript.SetPromptEnabled (this.currentStateIndex > 0); //Don't ask to go back while editing a move if move type hasn't been selected.
	}

	/// <summary>
	/// Moves to the previous state if not already at the first state.
	/// Hides all components except the ones used in the new state.
	/// </summary>
	public void PreviousState()
	{
		if (currentStateIndex > 0)
		{
			currentStateIndex--;
		}
		ShowOnlyCurrentPanel ();
		UpdatePhaseProgressBar ();
		this.returnScript.SetPromptEnabled (this.currentStateIndex > 0); //Don't ask to go back while editing a move if move type hasn't been selected.
	}

	public void Reset()
	{
		this.currentStateIndex = 0;
		ShowOnlyCurrentPanel ();
		UpdatePhaseProgressBar ();
		this.returnScript.SetPromptEnabled (this.currentStateIndex > 0); //Don't ask to go back while editing a move if move type hasn't been selected.
	}

	/// <summary>
	/// Updates the phase progress bar.
	/// </summary>
	public void UpdatePhaseProgressBar(){
		phaseProgressBar.UpdateFill (0.2f * (currentStateIndex + 1)); // + 1 because we fill up to the next phase (not up to the current).
	}
}
