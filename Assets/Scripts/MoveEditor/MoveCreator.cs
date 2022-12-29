using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCreator : MonoBehaviour
{
	private Move move;
	private SliderScript sliders;
	private Recorder recorder;
	private InputField nameInputField;
	private NameValidator nameValidator;
	private ActiveBodypartSelector activeBodypartSelector;
	private Button saveButton;
	private MovePlayer movePlayer;
	private EditorGuiManager editorGuiManager;
	private bool doneRecordingFrames;
	private List<GameObject> twistLimitMarkers;


	GameObject character;

	void Awake(){
		sliders = GameObject.Find ("SlidersPanel").GetComponent<SliderScript> ();
		nameValidator = GameObject.Find ("NamePanel").GetComponent<NameValidator> ();
		activeBodypartSelector = GameObject.Find("ActiveBodypartPanel").GetComponent<ActiveBodypartSelector>();
		saveButton = GameObject.Find ("SaveButton").GetComponent<Button> ();
		nameInputField = GameObject.Find ("NameInputField").GetComponent<InputField> ();
		move = new Move ();
		move.SetActiveBodypart ("Head");
		twistLimitMarkers = new List<GameObject> ();
		twistLimitMarkers.Add (GameObject.Find ("HighFrameLimitMarker"));
		twistLimitMarkers.Add (GameObject.Find ("LowFrameLimitMarker"));
		twistLimitMarkers.Add (GameObject.Find ("HighHardLimitMarker"));
		twistLimitMarkers.Add (GameObject.Find ("LowHardLimitMarker"));
	}

	void Start(){
		foreach (GameObject marker in twistLimitMarkers) {
			marker.SetActive (false);
		}
		this.editorGuiManager = GameObject.FindObjectOfType<EditorGuiManager> ();
		editorGuiManager.Init ();
	}


	public void CharacterSpawned()
	{
		character = GameObject.Find ("Character");
		movePlayer = character.GetComponent<MovePlayer> ();
		activeBodypartSelector.SetBlockMove (move.IsBlockMove ());
		activeBodypartSelector.BodypartChanged ("Head");
	}

	public void ActivateRecorder(){
		foreach (GameObject marker in twistLimitMarkers) {
			marker.SetActive (true);
		}
		foreach (GameObject dragPoint in UnityUtils.RecursiveContains(character.transform, "DragPoint"))
		{
			dragPoint.SetActive(true);
		}
		foreach (GameObject marker in twistLimitMarkers) {
			marker.SetActive (false);
		}

		recorder = character.GetComponent<Recorder> ();
		recorder.enabled = true;
		recorder.SetNameInputField (nameInputField);
		recorder.Init ();
		recorder.SetMove (move);

		Transform torsoDragPoint = UnityUtils.RecursiveFind (character.transform, "TorsoDragPoint");
		torsoDragPoint.GetComponent<DragAndDrop> ().Select ();

	}

	void Update ()
	{
		//Update strength and speed values if they have changed. Don't need to update twice since the sliders update each other.
		if (sliders.GetSpeed () != move.GetSpeed () || sliders.GetStrength () != move.GetStrength ()) 
		{
			updateStrengthAndSpeed (sliders.GetSpeed (), sliders.GetStrength ());
			UpdateShieldScale ();
		}
		//Update active bodypart.
		if (activeBodypartSelector.GetActiveBodypart () != null) {
			if (!activeBodypartSelector.GetActiveBodypart ().Equals (move.GetActiveBodypart ())) {
				move.SetActiveBodypart (activeBodypartSelector.GetActiveBodypart ());
				UpdateShieldScale ();
			}
		}
		if (recorder != null) {
			if (!doneRecordingFrames && recorder.IsDoneRecording ()) {
				doneRecordingFrames = true;
				foreach (GameObject dragPoint in UnityUtils.RecursiveContains(character.transform, "DragPoint"))
				{
					dragPoint.SetActive (false);
				}
				foreach (GameObject marker in twistLimitMarkers) {
					marker.SetActive (false);
				}
				editorGuiManager.NextState ();
			}
			//All frames recorded and a new, non-empty, move name has been entered.
			if (recorder.IsDoneRecording () && nameValidator.IsNameValid ()) {
				move.SetName (nameValidator.GetName ());
				saveButton.interactable = true;
			} else {
				saveButton.interactable = false; //hide button again if name is no longer valid.
			}
		}
	}

	private void UpdateShieldScale()
	{
		if (move.IsBlockMove () && GameObject.Find (move.GetActiveBodypart ().Replace (" ", "") + "Shield") != null)
		{
			GameObject shield = GameObject.Find (move.GetActiveBodypart ().Replace (" ", "") + "Shield");
			ShieldControl shieldControl = shield.GetComponent<ShieldControl> ();
			if (shieldControl != null) {
				shieldControl.UpdateScale (move);
			}
		}
	}

	/// <summary>
	/// Updates the strength and speed values to the values of the sliders.
	/// </summary>
	private void updateStrengthAndSpeed(int speed, int strength)
	{
		move.SetSpeed(speed);
		move.SetStrength(strength);
		//If animation is already playing, replay it as speed changes.
		if (movePlayer != null && movePlayer.CheckIsPlaying ())
		{
			movePlayer.PlayMove (this.move);
		}
	}

	/// <summary>
	/// Resets the MoveEditor by restarting the progressBar and the movePlayer.
	/// </summary>
	public void ResetMoveEditor()
	{
		foreach (GameObject marker in twistLimitMarkers) {
			marker.transform.parent = transform;
		}

		activeBodypartSelector.Reset ();
		Destroy (GameObject.Find ("Character"));

		nameInputField.text = nameValidator.GenerateValidName ();
		sliders.ResetSlider(); //Reset slider to 50/50
		SetBlockMove (false);
		move = new Move ();
		recorder.Reset (move);
        saveButton.interactable = false;
		this.doneRecordingFrames = false;
		this.editorGuiManager.Reset ();
    }

	public void SaveMove()
	{
		if (!AvailableMoves.ContainsName (move.GetName ()) && !move.GetName().Equals(string.Empty))
		{
			AvailableMoves.AddMove ((Move)move.Clone ());
			SaveLoad.Save(AvailableMoves.GetMoves()); //Save every time save button is clicked in case game crashes while still in scene.
			ResetMoveEditor ();
		}
		else
		{
            saveButton.interactable = false;
        }
	}

	public void SpawnCharacter(bool blockMove){
		gameObject.GetComponent<EditorCharacterSpawner> ().SpawnCharacter ();
		SetBlockMove (blockMove);
	}

	/// <summary>
	/// Sets whether the move is a block move or not.
	/// </summary>
	/// <param name="blockMove">If set to <c>true</c> move is a block.</param>
	public void SetBlockMove(bool blockMove)
	{

		this.move.SetBlockMove (blockMove);
		activeBodypartSelector.SetBlockMove (blockMove);
		if (blockMove)
		{
			//Change gui labels to match move type.
			sliders.SetSliderStrings ("Coverage", "Block");
		}
		else
		{
			//Change gui labels to match move type.
			sliders.SetSliderStrings ("Strength", "Speed");
		}

		string activeBodyPartName = move.GetActiveBodypart ();
		if (activeBodyPartName != null) {
			GameObject activeBodyPart = GameObject.Find (move.GetActiveBodypart ());
			if (activeBodyPart != null) {
				activeBodyPart.GetComponent<ColorModifier> ().SetSelected (!blockMove);
			}
			GameObject shield = GameObject.Find (move.GetActiveBodypart ().Replace (" ", "") + "Shield");
			shield.GetComponent<SpriteRenderer> ().enabled = blockMove;
		}
		UpdateShieldScale ();
	}
}
