using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListSelector : MonoBehaviour {

	private MovePanelListBehaviour attackList;
	private MovePanelListBehaviour blockList;
	GameObject attackViewport;
	GameObject blockViewport;
	GameObject blockMovesArrow;
	GameObject attackMovesArrow;
	MovePanelBehaviour moveListHeader;
	private GameObject deleteMovePrompt;

	void Awake()
	{
		this.attackList = GameObject.Find ("AttackContent").GetComponent<MovePanelListBehaviour> ();
		this.blockList = GameObject.Find ("BlockContent").GetComponent<MovePanelListBehaviour> ();
		this.attackViewport = GameObject.Find ("AttackViewport");
		this.blockViewport = GameObject.Find ("BlockViewport");
		this.blockMovesArrow = GameObject.Find ("BlockMovesArrow");
		this.attackMovesArrow = GameObject.Find ("AttackMovesArrow");
		this.moveListHeader = GameObject.Find ("MoveListHeader").GetComponent<MovePanelBehaviour> ();
		this.deleteMovePrompt = GameObject.Find ("DeleteMovePrompt");
	}

	void Start () {
		blockList.SetBlock (true);
		attackList.SetBlock (false);
		blockList.Init ();
		attackList.Init ();
		EnableAttackList ();
	}

	void Update () {

		bool horizontal1Right = Input.GetAxisRaw("Horizontal") > 0;
		bool horizontal1Left = Input.GetAxisRaw("Horizontal") < 0;
		bool horizontal2Right = Input.GetAxisRaw("Horizontal2") > 0;
		bool horizontal2Left = Input.GetAxisRaw("Horizontal2") < 0;
		bool horizontal1JoystickRight = Input.GetAxisRaw("HorizontalJoystick") > 0;
		bool horizontal1JoystickLeft = Input.GetAxisRaw("HorizontalJoystick") < 0;
		bool horizontal2JoystickRight = Input.GetAxisRaw("HorizontalJoystick2") > 0;
		bool horizontal2JoystickLeft = Input.GetAxisRaw("HorizontalJoystick2") < 0;

		bool leftPressed = (horizontal1Left || horizontal2Left || horizontal1JoystickLeft || horizontal2JoystickLeft);
		bool rightPressed = (horizontal1Right || horizontal2Right || horizontal1JoystickRight || horizontal2JoystickRight);

		if (leftPressed && !rightPressed && !deleteMovePrompt.activeSelf)
		{
			EnableAttackList ();
		}
		else if (rightPressed && !leftPressed && !deleteMovePrompt.activeSelf)
		{
			EnableBlockList ();
		}
	}

	private void EnableAttackList(){
		attackList.SetSelected (true);
		blockList.SetSelected (false);
		blockViewport.SetActive (false);
		attackViewport.SetActive (true);
		attackMovesArrow.SetActive (false);
		blockMovesArrow.SetActive (true);
		moveListHeader.GetComponent<Image> ().color = new Color (1.0f, 0.0f, 0.3f, 1.0f);
		moveListHeader.SetNameText ("Attack move name");
		moveListHeader.SetSpeedText ("Spd");
		moveListHeader.SetStrengthText ("Str");
	}

	private void EnableBlockList(){
		attackList.SetSelected (false);
		blockList.SetSelected (true);
		attackViewport.SetActive (false);
		blockViewport.SetActive (true);
		attackMovesArrow.SetActive (true);
		blockMovesArrow.SetActive (false);
		moveListHeader.GetComponent<Image> ().color = new Color (0.3f, 1.0f, 1.0f, 1.0f);
		moveListHeader.SetNameText ("Block move name");
		moveListHeader.SetSpeedText ("Blk");
		moveListHeader.SetStrengthText ("Cvr");
	}

	public void ClearButton(string button)
	{
		attackList.ClearButton (button);
		blockList.ClearButton (button);
	}

	/// <summary>
	/// Deletes the currently selected move from the list of moves
	///
	public void DeleteMove()
	{
		if (attackList.IsSelected ())
		{
			attackList.DeleteMove ();
		}
		else if (blockList.IsSelected ())
		{
			blockList.DeleteMove ();
		}
	}

	public void CancelDeleteMove() {
		attackList.CancelDeleteMove ();
		blockList.CancelDeleteMove ();
	}
}
