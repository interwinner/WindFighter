using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Data class holding information of a Frame. Consists of all rotatable body parts and their rotation at a certain moment.
/// </summary>
[System.Serializable]
public class Frame : ICloneable
{

	private Dictionary<string,float> bodyPartRotations;
	private float defaultRotation = 0;
    private string name = "";

	/// <summary>
	/// Initializes a new instance of the <see cref="Frame"/> class.
	/// </summary>
	public Frame()
	{
		bodyPartRotations = new Dictionary<string, float> ();
	}

	/// <summary>
	/// Adds the rotation a bodypart.
	/// </summary>
	/// <param name="bodyPart">Name of the bodypart.</param>
	/// <param name="rotation">Rotation of specified bodypart.</param>
	public void AddBodyPartRotation(string bodyPart, float rotation)
	{
		//If bodypart already has a rotation, remove it before adding it again.
		if (bodyPartRotations.ContainsKey (bodyPart))
		{
			bodyPartRotations.Remove (bodyPart);
		}
		bodyPartRotations.Add (bodyPart, rotation);
	}

	/// <summary>
	/// Returns the rotation of a specific bodypart.
	/// </summary>
	/// <returns>The rotation.</returns>
	/// <param name="bodyPart">Name of the bodypart.</param>
	public float getRotation(string bodyPart)
	{
        if (bodyPartRotations.ContainsKey(bodyPart))
        {
            return bodyPartRotations[bodyPart];
        }
		//If frame does not have any specified rotation for the bodypart, return the default rotation.
        else
        {
            return defaultRotation; //TODO: Is zero always the desired default? Might cause problems in the future.
        }
	}

	/// <summary>
	/// Returns a list containing the names of all bodyparts in the frame.
	/// </summary>
	/// <returns>The body part names.</returns>
	public List<string> getBodyPartNames()
	{
		List<string> names = new List<String> ();

		foreach (string key in bodyPartRotations.Keys) 
		{
			names.Add (key);
		}
		return names;
	}

	/// <summary>
	/// Clone this instance.
	/// </summary>
	public object Clone()
	{
		Frame clone = new Frame ();
		foreach (string key in bodyPartRotations.Keys) 
		{
			clone.AddBodyPartRotation ((string)key.Clone (), bodyPartRotations [key]);
		}
		return clone;
	}
}
