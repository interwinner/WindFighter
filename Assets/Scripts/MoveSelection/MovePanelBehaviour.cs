using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for manipulating each item in the list of moves in the MoveSelectionScreen.
/// </summary>
public class MovePanelBehaviour : MonoBehaviour
{
	// UI Text fields holding move information.
    private Text speedText;
    private Text strengthText;
	private Text nameText;

	private Text[] assignedButtonTexts; //One text field for each character so that they both can select the same move.
	private Move move;

	private Color32 defaultColor = new Color32 (150, 255, 255, 200); //Used to reset panel color on deselection.
	private Color32 selectedColor = new Color32 (150, 255, 255, 255);

	private GameObject previewCharacter;

	private Sprite deselectedSprite;
	private Sprite selectedSprite;

    void Awake()
    {
		this.deselectedSprite = Resources.Load <Sprite> ("Art/Main Menu/button_up_test");
		this.selectedSprite = Resources.Load <Sprite> ("Art/Main Menu/button_highlight_test");
        speedText = transform.Find("SpeedText").GetComponent<Text>();
        strengthText = transform.Find("StrengthText").GetComponent<Text>();
		nameText = transform.Find("NameText").GetComponent<Text>();
		assignedButtonTexts = new Text[2];
		assignedButtonTexts[0] = transform.Find("AssignedButton1Text").GetComponent<Text>();
		assignedButtonTexts[1] = transform.Find("AssignedButton2Text").GetComponent<Text>();
	}

	public void setMove(Move move)
	{
		this.move = move;
		if (move != null) {
			nameText.text = move.GetName ();
			speedText.text = "" + move.GetSpeed ();
			strengthText.text = "" + move.GetStrength ();
		}

		if (this.previewCharacter != null && move != null) 
		{
			this.previewCharacter.SetActive(true);
			this.previewCharacter.GetComponent<MovePlayer>().FrameToCharacter(move.GetFrames()[(move.GetTotalNbrOfFrames() - 1) / 2]);
			this.previewCharacter.GetComponent<MovePlayer>().setMoveToPlay(move);
			this.previewCharacter.GetComponent<MovePlayer>().ShowActiveBodypart(true);
		}
	}

    public void Select()
    {
		gameObject.GetComponent<Image> ().sprite = selectedSprite; //.color = Color.yellow;
		gameObject.GetComponent<Image>().color = selectedColor;
    }

    public void DeSelect()
	{
		gameObject.GetComponent<Image> ().sprite = deselectedSprite;
        gameObject.GetComponent<Image>().color = defaultColor;
    }

	/// <summary>
	/// Mark the panel with button string in player color at the column representing the playerNumber.
	/// </summary>
	/// <param name="button">Button.</param>
	/// <param name="color">Color.</param>
	/// <param name="playerNumber">Player number.</param>
	public void AssignButton(string button, Color32 color, int playerNumber)
	{
		playerNumber--; //Decrement to make player1 have index 0 etc.
		assignedButtonTexts[playerNumber].color = color;
		assignedButtonTexts[playerNumber].text = button;
	}

	/// <summary>
	/// Clears the assigned button from either player column.
	/// </summary>
	/// <param name="button">Button.</param>
	public void ClearAssignedButton(string button)
	{
		//Search all columns. Since no two players can use the same button, the button should be removed if it is in either column.
		foreach (Text assignedButtonText in assignedButtonTexts) {
			if (assignedButtonText.text.Equals (button)) {
				assignedButtonText.color = Color.black;
				assignedButtonText.text = "";
			}
		}
	}

	/// <summary>
	/// Clears the assigned button of a specific player in this panel.
	/// </summary>
	/// <param name="playerNumber">Player number.</param>
	public void ClearAssignedButton(int playerNumber)
	{
		playerNumber--; //Decrement to make player1 have index 0 etc.
		assignedButtonTexts[playerNumber].color = Color.black;
		assignedButtonTexts[playerNumber].text = "";
	}

	public Move getMove()
	{
		return this.move;
	}

	public void RemoveSpeedText()
	{
		this.speedText.text = "";
	}

	public void RemoveStrengthText()
	{
		this.strengthText.text = "";
	}

	public void RemoveNameText()
	{
		this.nameText.text = "";
	}

	public void SetSpeedText(string text)
	{
		this.speedText.text = text;
	}

	public void SetStrengthText(string text)
	{
		this.strengthText.text = text;
	}

	public void SetNameText(string text)
	{
		this.nameText.text = text;
	}

	public string GetAssignedButton(int playerNumber)
	{
		playerNumber--; //Decrement to make player1 have index 0 etc.
		return assignedButtonTexts [playerNumber].text;
	}

	public string GetAssignedButton()
	{
		for (int i = 0; i < assignedButtonTexts.Length; i++) {
			if (assignedButtonTexts [i] == null) {
				continue;
			}
			if (assignedButtonTexts [i].text == null) {
				continue;
			}
			if (assignedButtonTexts [i].text.Equals("")) {
				continue;
			}
			return assignedButtonTexts [i].text;
		}
		return null;
	}

	public void addPreviewCharacter(GameObject previewCharacter)
	{
		this.previewCharacter = previewCharacter;
	}

	public GameObject getPreviewCharacter() 
	{
		return this.previewCharacter;
	}
}
