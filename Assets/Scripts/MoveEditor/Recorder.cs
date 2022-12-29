using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for recording several Frames and storing them in a Move in the MoveEditor.
/// </summary>
public class Recorder : MonoBehaviour 
{
	private ProgressBarBehaviour progressBarBehaviour;
	private Move move; //The move being built by the recorder.
	private List<GameObject> endPoints;
	public bool reverseOnWayBack = true;
	private MovePlayer movePlayer;
    private MovePlayer onionMovePlayer;
	private bool doneRecording;
	private InputField nameInputField;
    private GameObject onionCharacter;
	private Frame initialPoseFrame;

	public void Init(){
		doneRecording = false;
		endPoints = new List<GameObject> ();
		movePlayer = gameObject.GetComponent<MovePlayer> ();
		FindEndPoints ();
		initialPoseFrame = GetCurrentPoseFrame ();
		progressBarBehaviour = GameObject.Find ("ProgressBar").GetComponent<ProgressBarBehaviour> ();
		onionCharacter = GameObject.Find("Onion Character");
		onionMovePlayer = onionCharacter.GetComponent<MovePlayer>();
	}
		
	public void SetMove(Move move)
	{
		this.move = move;
		RecordFrame (); //Add initial pose as first frame in move.
	}

	/// <summary>
	/// Adds a frame object containng rotations of each limb to the move.
	/// </summary>
	public void RecordFrame ()
	{
		if (move == null)
		{
			return;
		}
		if (move.GetCurrentNbrOfFrames () < move.GetTotalNbrOfFrames ()) 
		{
			Frame frame = GetCurrentPoseFrame ();
			UpdateFrameTwistLimits ();
            onionMovePlayer.FrameToCharacter(frame);
            move.AddFrame (frame);
			UpdateProgressBar ();
			if (move.GetCurrentNbrOfFrames () >= move.GetTotalNbrOfFrames () / 2 && reverseOnWayBack)
			{
				ReverseFrames ();
				FinishMove ();
			}
			else if (move.GetCurrentNbrOfFrames () >= (move.GetTotalNbrOfFrames () - 1) && !reverseOnWayBack)
			{
				move.AddFrame (move.GetFrames () [0]); //Add start frame to end of move as well.
				FinishMove ();
			}
		}
	}

	/// <summary>
	/// Creates a frame object containng current rotations of each limb.
	/// </summary>
	/// <returns>The current pose frame.</returns>
	private Frame GetCurrentPoseFrame()
	{
		Frame frame = new Frame ();
		foreach (GameObject go in endPoints) 
		{
			//Make sure endPoit has a drag and drop script before checking its parent's roation.
			DragAndDrop dragAndDrop = go.GetComponent<DragAndDrop> ();
			if (dragAndDrop == null) 
			{
				continue;
			}
			//Add end point parent (limb) rotation and name to move.
			float rotation = go.transform.parent.localEulerAngles.z;
			string name = go.transform.parent.name;
			frame.AddBodyPartRotation (name, rotation);
		}
		return frame;
	}

	/// <summary>
	/// Updates character frame twist limits to match current pose.
	/// </summary>
	private void UpdateFrameTwistLimits()
	{
		foreach (GameObject go in endPoints) 
		{
			//Make sure endPoit has a drag and drop script.
			DragAndDrop dragAndDrop = go.GetComponent<DragAndDrop> ();
			if (dragAndDrop == null) 
			{
				continue;
			}
			dragAndDrop.UpdateFrameLimits (); //Update drag and drop rotation limit by frame.
		}
	}

	void Update()
	{
		//User is not allowed to record a frame if left mouse button is down.
		//If the user drags a bodypart quickly while pressing space, sometimes the limits are updated without the frame being added.
		bool leftMouseButtonDown = Input.GetMouseButton (0);
		bool recordButtonPressed = Input.GetKeyDown ("space") || Input.GetButtonDown("Controller1Button0") || Input.GetButtonDown("Controller2Button0");
		if (!leftMouseButtonDown && recordButtonPressed && !nameInputField.isFocused)
		{
			RecordFrame ();
		}
	}

	/// <summary>
	/// Adds the reverse of all existing frames to make the move end where it started.
	/// </summary>
	private void ReverseFrames()
	{
		Frame[] frames = move.GetFrames ();
		int halfNbrOfFrames = move.GetTotalNbrOfFrames () / 2;
		for (int i = 0; i < halfNbrOfFrames; i++) 
		{
			int frameIndex = halfNbrOfFrames - 1 - i;
			Frame frame = frames [frameIndex];
			move.AddFrame (frame);
		}
	}

	/// <summary>
	/// Finds the end points of the all the children of the current gameobject.
	/// </summary>
	private void FindEndPoints()
	{
		Transform[] children = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform child in children) 
		{
			GameObject go = child.gameObject;
			DragAndDrop dragAndDrop = go.GetComponent<DragAndDrop> ();
			if (dragAndDrop != null) 
			{
				endPoints.Add (go);
			}
		}
	}

	/// <summary>
	/// Tells the MovePlayer that we are done recording the move and that it should start playing the move
	/// </summary>
	private void FinishMove()
	{
        onionCharacter.SetActive(false);
		movePlayer.SetAutoLoopEnabled(true);
		if (move != null)
		{
			movePlayer.PlayMove (move);
			doneRecording = true;
		}
	}

	public bool IsDoneRecording(){
		return doneRecording;
	}

	/// <summary>
	/// Reset the recorder and the progress bar.
	/// </summary>
	/// <param name="newMove">New move.</param>
	public void Reset(Move newMove)
	{
		if (newMove == null)
		{
			return;
		}
		Destroy (onionCharacter);
		doneRecording = false;
		movePlayer.SetAutoLoopEnabled (false);
		progressBarBehaviour.UpdateFill (0);
		movePlayer.FrameToCharacter (initialPoseFrame); //Reset character pose
		UpdateFrameTwistLimits();
	}

	/// <summary>
	/// Updates the progress bar.
	/// </summary>
	private void UpdateProgressBar()
	{
		//With the first and last frames, some of the bar is filled in on start.
		float nbrOfRecordedFrames = (float)move.GetCurrentNbrOfFrames () - 1; //Player did not record the first frame.
		nbrOfRecordedFrames *= 2; //Player only records first half of frames. Each recorded frame is used twice.
		float totalNbrOfRecordableFrames = (float)move.GetTotalNbrOfFrames () - 2; //Player does not record first or last frame.
		//Player did not record the last frame.
		nbrOfRecordedFrames = nbrOfRecordedFrames > totalNbrOfRecordableFrames ? totalNbrOfRecordableFrames : nbrOfRecordedFrames;

		float progress = nbrOfRecordedFrames / totalNbrOfRecordableFrames;
		progressBarBehaviour.UpdateFill (progress);
	}

	public void SetNameInputField(InputField nameInputField){
		this.nameInputField = nameInputField;
	}
}
