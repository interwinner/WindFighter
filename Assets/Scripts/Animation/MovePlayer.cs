using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add to top empty object of a character in Unity Hierarchy. Makes it possible for that character to play the animation of a <see cref="Move"/>.
/// </summary>
public class MovePlayer : MonoBehaviour
{
	private bool isPlaying;
	private Move moveToPlay;
	private List<Frame> frames;
	int frameIndex = 0;
	private Dictionary<string,Transform> nameBodyPartDict = new Dictionary<string, Transform> ();
	private bool autoLoop;
	private bool paused;
  
	void Awake()
	{
		autoLoop = false;
		frames = new List<Frame> ();
		FindRotatables ();
	}

	void Update()
	{
		if (paused) {
			return;
		}
		//While isPlaying is true, a new frame is displayed each update until animation is done playing.
		if (isPlaying) {
			if (frameIndex < frames.Count)
			{
				Frame frame = frames [frameIndex];
				FrameToCharacter (frame);
				frameIndex++;
			}
			else if (autoLoop)
			{
				frameIndex = 0;
			}
			else
			{
				//Clean up and reset for new move
				frameIndex = 0;
				isPlaying = false;
				frames.Clear ();
				ShowActiveBodypart (false);
			}
		}
	}

	/// <summary>
	/// Calculates all frames between the recorded key frames to create a complete animation.
	/// </summary>
	/// <param name="move">The move to be displayed.</param>
	public void PlayMove(Move move)
	{
		ShowActiveBodypart (false);
		isPlaying = true;
        frames = new List<Frame>();
		moveToPlay = move;
		ShowActiveBodypart (true);

		float speedPercentage = (float)move.GetSpeed () / 100f;
		if (move.IsBlockMove ()) {
			speedPercentage = Parameters.blockSpeedPercentage;
		}
		float fSpeed = Parameters.minSpeed + speedPercentage * (Parameters.maxSpeed - Parameters.minSpeed);
		int speed = (int)Mathf.Round (fSpeed);

		Frame[] moveFrames = move.GetFrames ();

		//Repeat middle frame Parameters.blockTime times for block moves to make them remain extended for a fixed amount of time.
		if (move.IsBlockMove ()) 
		{
			Frame[] blockMoveFrames = new Frame[moveFrames.Length + Parameters.blockTime]; //Make room for extra frames.
			int nbrOfMoveFrames = moveFrames.Length;
			int x = 0;
			for (int i = 0; i < blockMoveFrames.Length; i++)
			{
				blockMoveFrames [i] = moveFrames [x]; //Transfer frames.
				if (i < nbrOfMoveFrames / 2 || i > (nbrOfMoveFrames / 2) - 1 + Parameters.blockTime) { //Make x the same for a blockTime size gap in the middle.
					x++;
				}
			}
			moveFrames = blockMoveFrames;
		}

		for (int i = 0; i < (moveFrames.Length - 1); i++)
		{
			Frame firstFrame = moveFrames [i];
			Frame secondFrame = moveFrames [i + 1];
			int p = 0;
			for (p = 0; p <= 100; p += speed)
			{
				Frame newFrame = BlendFrames (firstFrame, secondFrame, p);
				frames.Add (newFrame);
			}
			//Make sure absolute last frame is included to return character to it's original stance.
			if (i == (moveFrames.Length - 2) && p != 100)
			{
				Frame newFrame = BlendFrames (firstFrame, secondFrame, 100);
				frames.Add (newFrame);
			}
		}
		frameIndex = 0; //Make sure the animation starts from the first frame.
	}

	/// <summary>
	/// Blends two frames to create a new frame.
	/// </summary>
	/// <returns>The new blended frame.</returns>
	/// <param name="fromFrame">The first frame.</param>
	/// <param name="toFrame">The second frame.</param>
	/// <param name="percentage">The percentage to add from the second frame to the first frame.
	/// At 0 percent, the blended frame is identical to the first, and 100 it is identical to the secont</param>
	public Frame BlendFrames(Frame fromFrame, Frame toFrame, int percentage)
	{
		List<string> bodyPartNames = fromFrame.getBodyPartNames ();
		Frame newFrame = (Frame) fromFrame.Clone ();

		foreach (string name in bodyPartNames) 
		{
			float newRotation;
			float fromRot = fromFrame.getRotation (name);
			float toRot = toFrame.getRotation (name);
            //Make sure rotation around zero does not result in a loop in the wrong direction.
            //Make the angle right of zero (the smaller angle) bigger.
			if (RotationUtils.ZeroInSmallerLimit (fromRot, toRot)) {
				if (fromRot < toRot) {
					fromRot += 360;
				} else if (fromRot > toRot) {
					toRot += 360;
				}
			//Make sure rotation 0 <=> [180-359] does not rotate the wrong way.
			} else if (fromRot == 0 && toRot > 180) {
				fromRot += 360;
			} else if (toRot == 0 && fromRot > 180) {
				toRot += 360;
			}
			newRotation = fromRot + (float)percentage/(float)100 * (toRot- fromRot);

			newFrame.AddBodyPartRotation (name, newRotation);
		}
		return newFrame;
	}

	/// <summary>
	/// Map a frame to the body parts of a character
	/// </summary>
	/// <param name="frame">The frame to map.</param>
	public void FrameToCharacter(Frame frame)
	{
		List<string> bodyPartNames = frame.getBodyPartNames ();
		foreach (string bodyPartName in bodyPartNames)
		{
			Transform bodyPart = nameBodyPartDict [bodyPartName];
			float rotation = frame.getRotation (bodyPartName);
			float currentXRot = bodyPart.localEulerAngles.x;
			float currentYRot = bodyPart.localEulerAngles.y;
			bodyPart.localEulerAngles = new Vector3 (currentXRot, currentYRot, rotation);
		}
	}

	/// <summary>
	/// Finds all rotatable body parts of the character to which the MovePlayer is attached.
	/// Necessary in order to rotate the correct body parts when a move animation is played.
	/// </summary>
	private void FindRotatables()
	{
		Transform[] children = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform child in children)
		{
			GameObject go = child.gameObject;
			string tag = go.tag;
			if (tag.Equals ("Rotatable"))
			{
				nameBodyPartDict.Add (go.name, child);
			}
		}
	}

	public void SetAutoLoopEnabled(bool enabled)
	{
		autoLoop = enabled;
		if (!autoLoop) {
			isPlaying = false;
		}
	}

	public bool CheckIsPlaying()
	{
		return isPlaying;
	}
    
    public void SetIsPlaying()
    {
        isPlaying = true;
    }

	public void Pause()
	{
		this.paused = true;
	}

	public void UnPause()
	{
		this.paused = false;
	}

	/// <summary>
	/// Shows or hides the active bodypart of the current move to play.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public void ShowActiveBodypart(bool show){
		if(this.moveToPlay == null)
		{
			return; //Do nothing if there is no move to play.
		}
		Transform bodypart = UnityUtils.RecursiveFind (transform, this.moveToPlay.GetActiveBodypart ());
		Transform shield = UnityUtils.RecursiveFind (transform, this.moveToPlay.GetActiveBodypart ().Replace (" ", "") + "Shield");
		if (bodypart != null && bodypart.gameObject.GetComponent<ColorModifier> () != null && !moveToPlay.IsBlockMove ())
		{
			//Highlight damage dealer if move is not a block move and show true, hide otherwise.
			bodypart.gameObject.GetComponent<ColorModifier> ().SetSelected (show);
		}
		if (shield != null && moveToPlay.IsBlockMove ()) 
		{
			//Show shield if move is a block move and show true, hide otherwise.
			shield.gameObject.GetComponent<SpriteRenderer> ().enabled = show;
			ShieldControl shieldControl = shield.GetComponent<ShieldControl> ();
			if (shieldControl != null) {
				shieldControl.UpdateScale (moveToPlay);
			}
		}
	}

    public void reset()
    {
        if(moveToPlay != null)
        {
            FrameToCharacter(moveToPlay.GetFrames()[0]);
        }
        SetAutoLoopEnabled(false);
        isPlaying = false;
        ShowActiveBodypart(false);
        frames = new List<Frame>();
        frameIndex = 0;
    }

	public void setMoveToPlay(Move moveToPlay)
	{
		this.moveToPlay = moveToPlay;
	}
}
