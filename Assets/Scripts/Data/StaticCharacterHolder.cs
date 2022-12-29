using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCharacterHolder{

	private static bool alreadyInitialized = false;
	public static List<Character> characters;
	public static Character character1;
	public static Character character2;

	public static void Init()
	{
		if (!alreadyInitialized)
		{
			alreadyInitialized = true;
			characters = new List<Character> ();
			character1 = new Character (Color.red, 1);
			characters.Add (character1);
			character2 = new Character (Color.blue, 2);
			characters.Add (character2);
		}
	}

	public static void ResetCharacters()
	{
		foreach (Character character in characters)
		{
			character.ClearMoves ();
			character.SetHealth (character.GetMaxHealth ());
		}
	}
}
