using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all execution of fight moves when playing the game.
/// </summary>
public class FightMoveController : MonoBehaviour {

	private LayerHandler layerHandler;
	string activeBodypartName;
	private Collider2D activeBodypartCollider;
	private Character character;
	private Move currentlyPlayedMove;
	private Rigidbody2D thisBody;
	private MovePlayer characterMovePlayer;
	private GameState gameState;

	// Use this for initialization
	void Awake () {
		thisBody = gameObject.GetComponent<Rigidbody2D> ();
		layerHandler = GameObject.Find("Handler").GetComponent<LayerHandler>();
		this.gameState = GameObject.Find ("Handler").GetComponent<GameState> ();
		characterMovePlayer = gameObject.GetComponent<MovePlayer> ();
	}

    void Start ()
    {
        // Enable hitboxes to make sure no moves are registered before fightmovecontroller is initialized
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.transform.Find("LowerBodyHitbox").GetComponent<BoxCollider2D>().enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
		if (!characterMovePlayer.CheckIsPlaying ()) {
			currentlyPlayedMove = null;
			// Only set the collider to false if we have enabled it once before
			if (activeBodypartCollider != null)
			{
				activeBodypartCollider.enabled = false;
			}
		}
	}

	/// <summary>
	/// Takes a move name and plays it on the character. Also enables collision of the damaging bodypart or shield.
	/// The collisions of thes active bodypart are not handled in this class.
	/// </summary>
	/// <param name="moveName">Move name.</param>
	public void DoMove(string moveName)
	{
		if (character == null) {
			return;
		}

		//Make sure other character is not invunerable when doing a move.
		//Each move makes the character invunerable until next move so no move can do dmg twice.
		int otherCharacterNumber = (this.character.GetNbr () % 2) + 1; // 1 -> 2, 2 -> 1
		this.gameState.SetCharacterVunerable (otherCharacterNumber);

		currentlyPlayedMove = character.GetMove (moveName);
		Move move = character.GetMove (moveName);
		//Make sure the character cannot start playing another animation until this one is finished.
		layerHandler.sendToCharacterLayer(this.gameObject);
		thisBody.velocity = new Vector2(0, thisBody.velocity.y);
		// Sets MovePlayer.isPlaying before calling MovePlayer.PlayMove() to avoid concurrency issues.
		characterMovePlayer.SetIsPlaying ();
		characterMovePlayer.PlayMove (currentlyPlayedMove);
		// Get the name of the move assigned to do damage.
		activeBodypartName = currentlyPlayedMove.GetActiveBodypart();
		if (currentlyPlayedMove.IsBlockMove ()) {
			activeBodypartName = activeBodypartName.Replace (" ", "") + "Shield";
		}
		//Enable the collider of the active bodypart or shield.
		Transform activeBodypart = UnityUtils.RecursiveFind (transform, activeBodypartName);
		activeBodypartCollider = activeBodypart.GetComponent<Collider2D> ();
		activeBodypartCollider.enabled = true;
	}

	public Move GetCurretlyPlayedMove()
	{
		return this.currentlyPlayedMove;
	}

	public bool IsDoingMove()
	{
        return characterMovePlayer.CheckIsPlaying ();
    }

	public void Pause()
	{
		characterMovePlayer.Pause ();
	}

	public void UnPause()
	{
		characterMovePlayer.UnPause ();
	}

	public void SetCharacter(Character character)
	{
		this.character = character;
	}
}
