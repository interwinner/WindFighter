using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all recorded moves that can be chosen in the Move Selection Screen.
/// </summary>
public class AvailableMoves
{
	private static List<Move> moves = new List<Move>();

	//TODO: store moves as they are completed in the move editor.

	/// <summary>
	/// Returns a list of all the available moves.
	/// </summary>
	/// <returns>The moves.</returns>
	public static List<Move> GetMoves()
	{
		return moves;
	}

    public static void AddMove(Move move)
    {
        moves.Add(move);
    }

    public static void DeleteMove(Move move) 
    {
        moves.Remove(move);
    }

    /// <summary>
    /// Prints the names of all currently available moves
    /// </summary>
    /// <returns>String containing names of all available moves</returns>
    public static string PrintMoveNames()
    {
        string str = "";
        foreach (Move move in moves)
        {
            str = str + move.GetName() + ", ";
        }
        return str;
    }

    /// <summary>
    /// Checks if the list of available moves contains a name or not
    /// </summary>
    /// <param name="name">Name to be looked for in the list of available moves.</param>
    /// <returns>True if name already exists, false otherwise.</returns>
    public static bool ContainsName(string name)
    {
        foreach (Move move in moves)
        {
            if (move.GetName().Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    public static void SetMoves(List<Move> newMoves)
    {
        moves = newMoves;
    }
}
