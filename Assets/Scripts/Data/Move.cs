using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class holding information of a Move. Consists of a collection of <see cref="Frame"/>'s and stats.
/// </summary>
[System.Serializable]
public class Move
{
	private int speed;
	private int strength;
	private Frame[] frames;
	private int nextIndex; //At which index to place the next recorded frame. Adding or removing frames updates the index.
	private bool done;
	private int defaultNrOfFrames = 12; //C# requires default constructor. In default constructor, this number is used to create the frame array.
	private string name;
	private string activeBodypart; //Name of the body part dealing damage or blocking when move is performed.
	private int currentNbrOfFrames;
	private bool blockMove;

	/// <summary>
	/// Initializes a new instance of the <see cref="Move"/> class.
	/// </summary>
	public Move()
	{
		this.currentNbrOfFrames = 0;
		this.frames = new Frame[defaultNrOfFrames];
		this.speed = 50;
		this.strength = 50;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Move"/> class.
	/// </summary>
	/// <param name="nrOfFrames">Number of frames total in the move.</param>
	public Move(int nrOfFrames)
	{
		frames = new Frame[nrOfFrames]; //TODO: Should the length of a move be adjustable? Maybe it is always the same?
		this.speed = 50;
		this.strength = 50;
	}

	/// <summary>
	/// Add a frame to the list. Increases nextIndex.
	/// </summary>
	/// <param name="frame">Frame.</param>
	public void AddFrame(Frame frame)
	{
		if (nextIndex < frames.Length)
		{
			frames [nextIndex] = frame;
			nextIndex++;
			currentNbrOfFrames++;
		}
	}

	/// <summary>
	/// Remove a frame from the list. Decreases nextIndex.
	/// </summary>
	public void RemoveFrame()
	{
		if (nextIndex > 0)
		{
			nextIndex--;
			frames [nextIndex] = null;
			currentNbrOfFrames--;
		}
	}

	public void SetSpeed(int newSpeed)
	{
		if (newSpeed >= 0)
		{
			speed = newSpeed;
		}
	}

	public void SetStrength(int newStrength)
	{
		if (newStrength >= 0)
		{
			strength = newStrength;
		}
	}

	public Frame[] GetFrames()
	{
		return frames;
	}

	/// <summary>
	/// Returns the total number of frames this move has when completed.
	/// </summary>
	/// <returns>The number of frames.</returns>
	public int GetTotalNbrOfFrames()
	{
		return frames.Length;
	}

    /// <summary>
    /// Get the speed of this instance
    /// </summary>
    /// <returns>The Speed of this instance</returns>
	public int GetSpeed()
	{
		return speed;
	}

    public int GetStrength()
    {
        return strength;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
	}

	public void SetActiveBodypart(string newActiveBodypart)
	{
		this.activeBodypart = newActiveBodypart;
	}

	public string GetActiveBodypart()
	{
		return this.activeBodypart;
	}

	public int GetCurrentNbrOfFrames()
	{
		return currentNbrOfFrames;
	}

	public object Clone()
	{
		Move clone = new Move ();
		clone.speed = this.speed;
		clone.strength = this.strength;
		clone.frames = (Frame[])this.frames.Clone ();
		clone.nextIndex = this.nextIndex;
		clone.done = this.done;
		clone.defaultNrOfFrames = 12;
		clone.name = (string)this.name.Clone ();
		clone.activeBodypart = (string)this.activeBodypart.Clone ();
		clone.currentNbrOfFrames = this.currentNbrOfFrames;
		clone.blockMove = this.blockMove;
		return clone;
	}

	public void SetBlockMove(bool blockMove){
		this.blockMove = blockMove;
	}

	public bool IsBlockMove(){
		return this.blockMove;
	}
}
