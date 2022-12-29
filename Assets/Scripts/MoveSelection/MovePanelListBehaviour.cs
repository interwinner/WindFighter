using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
///  Class for handling the list of available moves in the MoveSelectionScene.
/// </summary>
public class MovePanelListBehaviour : MonoBehaviour 
{
	private int selectedListIndex = 0; //The index of the currently selected list item.
	private float listItemHeight; //The height of one list element.
	private int nbrOfVisiblePanels;
	private List<Move> moves;
	private MovePanelBehaviour[] listItems; //Object used for interacting with the underlying list items.
	private SelectionPanelBahviour[] selectedMovesPanels;
	private MovePlayer character1; //used for displaying the selected move on one of the visible characters.
    private MovePlayer character2;
    private GameObject playText;
    private float scrollDelay;
	private bool hasDeleted = false;
    private SceneButtonBehaviour playTextBehaviour;

	private bool selected;
	private bool containsBlockMoves;
	private ListSelector listSelector; 

	private GameObject deleteMovePrompt;
	private Move moveToBeDeleted;
	private bool controlsActive = true;

	void Awake ()
	{
		this.deleteMovePrompt = GameObject.Find ("DeleteMovePrompt");
		playText = GameObject.Find ("PlayText");
		character1 = GameObject.Find("Character 1").GetComponent<MovePlayer>(); //Choose the character to use for animating move previews.
		character2 = GameObject.Find("Character 2").GetComponent<MovePlayer>();
		playTextBehaviour = GameObject.Find("Handler").GetComponent<SceneButtonBehaviour>(); //Used to switch scene by pressing Enter.
		listSelector = GameObject.Find("Handler").GetComponent<ListSelector>();
	}

    public void Init () 
	{
		deleteMovePrompt.SetActive(false);
        scrollDelay = 0;
		//Hide the play button untill each character has a move assigned to each of its used buttons.
		playText.SetActive (false);
		//Create a list item for each available move.
		moves = AvailableMoves.GetMoves ();

		//Make list items as high as the MeasuingPanel which is 1/7 of the viewport.
		RectTransform measuringPanel = GameObject.Find ("MeasuringPanel").GetComponent<RectTransform> ();
		listItemHeight = measuringPanel.rect.height;
		nbrOfVisiblePanels = 7;

		//Update viewport content size to make room for all moves.
		GameObject listViewContent = gameObject;
		RectTransform listViewContentRect = listViewContent.GetComponent<RectTransform> ();
		Vector2 viewContentSize = listViewContentRect.sizeDelta;
		Vector2 viewContentNewSize = new Vector2 (viewContentSize.x, (moves.Count + 1) * listItemHeight); // +1 for margin
		listViewContentRect.sizeDelta = viewContentNewSize;

		int nbrOfRightMoveType = 0;
		for (int i = 0; i < moves.Count; i++)
		{
			Move move = moves [i];
			if(move.IsBlockMove() == containsBlockMoves)
			{
				nbrOfRightMoveType++;
			}
		}
		listItems = new MovePanelBehaviour[nbrOfRightMoveType];
		int j = 0;
        for (int i = 0; i < moves.Count; i++)
		{
			Move move = moves [i];
			if(move.IsBlockMove() == containsBlockMoves)
			{
				GameObject previewPanel = CreateMovePanel (move, transform);
				listItems[j] = previewPanel.GetComponent<MovePanelBehaviour> (); //Get the interaction script from the created list item.
				j++;
			}
        }

		//Get a reference to the interaction script of each panel viewing the currently selected moves of each character and place them in a list.
		selectedMovesPanels = new SelectionPanelBahviour[2];
		for (int i = 0; i < selectedMovesPanels.Length; i++) {
			// (Panel(i+1) is used becuase the panels are named Panel1 and Panel2, not Panel0 and Panel1)
			selectedMovesPanels[i] = GameObject.Find ("SelectedMovesPanel" + (i+1)).GetComponent<SelectionPanelBahviour> ();
			Character character = StaticCharacterHolder.characters [i];
			//Make sure there is actually a characters present for each selected moves panel.
			if (character == null) {
				continue;
			}
			selectedMovesPanels [i].SetOwner (character);
		}

		if (listItems.Length > 0) {

			listItems [selectedListIndex].Select ();
			if (selectedListIndex > nbrOfVisiblePanels / 2 && hasDeleted && listItems.Length >= nbrOfVisiblePanels) {
				ScrollList (-1);
				hasDeleted = false;
			}

			ReselectMoves ();
		}

		if (selected) {
			PlayAnimation (1);
		}
    }

	/// <summary>
	/// Goes through all previously selected moves from InputSettings and selects them in the GUI.
	/// </summary>
	public void ReselectMoves()
	{
		List<string> usedButtons = InputSettings.allUsedButtons;
		//For every used button, if the move asigned to it matches that of a panel list item. Mark that list item in GUI.
		foreach (string button in usedButtons) {
			string buttonMoveName = InputSettings.GetMoveName (button);
			for (int i = 0; i < listItems.Length; i++) {
				MovePanelBehaviour listItem = listItems [i];
				Move panelMove = listItem.getMove ();
				if (panelMove == null)
				{
					continue;
				}
				if (panelMove.GetName ().Equals (buttonMoveName))
				{
					RegisterPlayerMoveToButton (button, i);
				}
			}
		}
	}
    
	void Update () 
	{
		if (controlsActive) {
			bool vertical1Up = Input.GetAxisRaw("Vertical") > 0 && selected;
			bool vertical1Down = Input.GetAxisRaw("Vertical") < 0 && selected;
			bool vertical2Up = Input.GetAxisRaw("Vertical2") > 0 && selected;
			bool vertical2Down = Input.GetAxisRaw("Vertical2") < 0 && selected;
			bool verticalControllerUp = Input.GetAxisRaw("VerticalJoystick") < 0 && selected;
			bool verticalControllerDown = Input.GetAxisRaw("VerticalJoystick") > 0 && selected;
			bool verticalController2Up = Input.GetAxisRaw("VerticalJoystick2") < 0 && selected;
			bool verticalController2Down = Input.GetAxisRaw("VerticalJoystick2") > 0 && selected;

			if (scrollDelay > 0)
			{
				scrollDelay -= Time.deltaTime;
			}
			else
			{
				if (scrollDelay <= 0 && listItems != null) {
					if (vertical1Up || vertical2Up || verticalControllerUp || verticalController2Up) { //Up pressed
						scrollDelay = Parameters.scrollDelay;
						bool movedOutOfTopPanels = selectedListIndex <= (nbrOfVisiblePanels / 2);
						MoveSelection (-1);
						bool movedIntoBotPanels = selectedListIndex >= listItems.Length - (nbrOfVisiblePanels / 2) - 1;
						if (!movedOutOfTopPanels && !movedIntoBotPanels) {
							ScrollList (-1);
						}
					} else if (vertical1Down || vertical2Down | verticalControllerDown || verticalController2Down) { //Down pressed
						scrollDelay = Parameters.scrollDelay;
						bool movedOutOfBotPanels = selectedListIndex >= listItems.Length - (nbrOfVisiblePanels / 2) - 1;
						MoveSelection (1);
						bool moveIntoTopPanels = selectedListIndex <= nbrOfVisiblePanels / 2;
						if (!moveIntoTopPanels && !movedOutOfBotPanels) {
							ScrollList (1);
						}
					}
				}

				if (vertical1Up || vertical1Down || verticalControllerUp || verticalControllerDown)
				{
					PlayAnimation(1);
				}
				else if (vertical2Up || vertical2Down || verticalController2Down || verticalController2Up)
				{
					PlayAnimation(2);
				}
			}

			bool startPressed = Input.GetButtonDown("Controller1Button7") || Input.GetButtonDown("Controller2Button7");
			bool enterPressed = Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return);
			bool playButtonPressed = startPressed || enterPressed;

			if (playButtonPressed && InputSettings.AllButtonsAssigned())
			{
				playTextBehaviour.SwitchScene("FightScene");
			}
			if (Input.GetKeyDown("delete") && selected) {
				ShowDeleteMovePanel();
			}
			if (Input.anyKeyDown && selected)
			{
				//Check if any button used in the game has been pressed.
				foreach (string button in InputSettings.allUsedButtons)
				{
					try {
						if (Input.GetKeyDown (button))
						{
							RegisterPlayerMoveToButton (button);
						}
					}
					catch
					{

					}
					try {	
						if (Input.GetButtonDown (button))
						{
							RegisterPlayerMoveToButton (button);
						}
					}
					catch
					{
						
					}
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown("Controller1Button1") || Input.GetButtonDown("Controller2Button1")) 
		{
			controlsActive = true;
		}
        
	}

	/// <summary>
	/// Moves the selection a number of steps up or down.
	/// </summary>
	/// <param name="steps"> Number of steps to move. Positive for down and negative for up.</param>
	private void MoveSelection(int steps)
	{
		int newIndex = selectedListIndex + steps;
		if (newIndex >= 0 && newIndex < listItems.Length)
		{
			listItems [selectedListIndex].DeSelect ();
			selectedListIndex = newIndex;
			listItems [newIndex].Select ();
		}
	}

	/// <summary>
	/// Scrolls the list a number of steps up or down by moving the content of the scroll view (which this script is place on) up or down.
	/// </summary>
	/// <param name="steps">Number of steps to scroll. Positive for up and negative for down.</param>
	private void ScrollList(int steps)
	{
		Vector3 currentPosition = GetComponent<RectTransform>().localPosition;
		float newPositionY = currentPosition.y + steps * listItemHeight;
		Vector3 newPosition = new Vector3 (currentPosition.x, newPositionY, currentPosition.z);
		GetComponent<RectTransform>().localPosition = newPosition;
	}

	/// <summary>
	/// Plays the move animation of the currently selected list item on one of the characters in the scene.
	/// </summary>
	private void PlayAnimation(int characterNumber)
	{
		character2.reset();
		character1.reset();
		if (moves == null || listItems == null)
		{
			return;
		}
		if (selectedListIndex >= moves.Count || selectedListIndex >= listItems.Length)
		{
			return;
		}
		if (moves[selectedListIndex] == null || listItems [selectedListIndex] == null)
		{
			return;
		}
        if (characterNumber == 1)
        {
            character1.SetAutoLoopEnabled(true);
			character1.PlayMove (listItems [selectedListIndex].getMove ());
        }
        else if (characterNumber == 2)
        {
            character2.SetAutoLoopEnabled(true);
			character2.PlayMove (listItems [selectedListIndex].getMove ());
        }
		
	}

	/// <summary>
	/// Creates a move panel.
	/// </summary>
	/// <returns>The move panel.</returns>
	/// <param name="move">Move.</param>
	/// <param name="parent">Parent.</param>
	public GameObject CreateMovePanel(Move move, Transform parent)
	{
		string previewPath = "Prefabs" + Path.DirectorySeparatorChar + "MovePanel";
		GameObject previewPanelObject = (GameObject)Resources.Load (previewPath);
		GameObject previewPanel = Instantiate (previewPanelObject, previewPanelObject.transform.position, previewPanelObject.transform.rotation, parent);
		previewPanel.GetComponent<LayoutElement> ().preferredHeight = listItemHeight; //Set the preferred height to 1/7 the height of the viewport.
		MovePanelBehaviour panelBehaviour = previewPanel.GetComponent<MovePanelBehaviour> ();
		panelBehaviour.setMove (move);
		return previewPanel;
	}

	/// <summary>
	/// Registers the move of the currently selected list item to a button.
	/// </summary>
	/// <param name="button">The button to assign the move to.</param>
	private void RegisterPlayerMoveToButton(string button)
	{
		RegisterPlayerMoveToButton (button, selectedListIndex);
	}

	/// <summary>
	/// Registers the move of a list item at a specific index to a button.
	/// </summary>
	/// <param name="button">The button to assign the move to.</param>
	/// <param name="index">The index of the list item.</param>
	private void RegisterPlayerMoveToButton(string button, int index)
	{
		Move selectedMove = listItems [index].getMove ();
		Character registeredCharacter = InputSettings.Register (button, selectedMove.GetName ());
		//The character returned by InputSettings.Register is the character that uses the button.
		//If the returned character is null, no character uses the button.
		if (registeredCharacter != null)
		{
			registeredCharacter.AddMove (selectedMove);
			Color characterColor = registeredCharacter.GetColor ();
			int characterNbr = registeredCharacter.GetNbr ();

			string buttonDisplayName = button;
			if (buttonDisplayName.Length > 1){
				buttonDisplayName = UnityUtils.ControllerButtonToDisplayName(buttonDisplayName);
			} 

			listSelector.ClearButton (buttonDisplayName);
			listItems [index].AssignButton (buttonDisplayName, characterColor, characterNbr); //Mark the selected list item with button and player color.
			//listSelector
			AddPanelToCharacterMoves (registeredCharacter, buttonDisplayName, index);
			ShowOrHideplayText ();
		}
	}

	public void ClearButton(string button)
	{
		if (listItems != null) {
			//Remove the marking from any list item of a move previously registered to the button.
			for (int i = 0; i < listItems.Length; i++)
			{
				listItems [i].ClearAssignedButton (button);
			}
		}
	}

	private void ShowDeleteMovePanel() {
		if (listItems != null && listItems.Length > 0) {
			moveToBeDeleted = listItems [selectedListIndex].getMove ();
			controlsActive = false;
			deleteMovePrompt.SetActive (true);
			Button button = deleteMovePrompt.GetComponentInChildren<Button> ();
			if (button != null) {
				button.Select ();
			}
		}
	}

	/// <summary>
	/// Deletes the currently selected move from the list of moves
	///
	public void DeleteMove() {
		foreach(SelectionPanelBahviour panel in selectedMovesPanels)
		{
			panel.RemovePanelWithMove(moveToBeDeleted.GetName());
		}

		AvailableMoves.DeleteMove(moveToBeDeleted);
		foreach(MovePanelBehaviour panel in listItems)
		{
			Destroy(panel.gameObject);
		}
		if (selectedListIndex < listItems.Length - 1)
		{
			selectedListIndex = Mathf.Max (selectedListIndex, 0);
		}
		else
		{
			selectedListIndex = Mathf.Max (selectedListIndex - 1, 0);
		}

		InputSettings.Deregister (moveToBeDeleted.GetName ());
		
		hasDeleted = true;
		controlsActive = true;

		SaveLoad.Save(moves);
		Init ();
	}

	public void CancelDeleteMove() {
		controlsActive = true;
		deleteMovePrompt.SetActive(false);
		ShowOrHideplayText ();
	}

	/// <summary>
	/// Adds a panel to the selected moves of a character.
	/// </summary>
	/// <param name="character">Character.</param>
	/// <param name="button">Button.</param>
	private void AddPanelToCharacterMoves(Character character, string button, int index)
	{
		//Find the panel of the character.
		foreach (SelectionPanelBahviour selectionPanel in selectedMovesPanels)
		{
			if (selectionPanel.GetOwner ().Equals (character))
			{
				MovePanelBehaviour movePanel = listItems [index];
				selectionPanel.AddMove (button , movePanel.getMove ());
				break;
			}
		}
	}

	/// <summary>
	/// Shows the or hide play button depending on wether all characters have registered all moves.
	/// </summary>
	private void ShowOrHideplayText()
	{
		bool showplayText = InputSettings.AllButtonsAssigned ();
		playText.SetActive (showplayText);
	}

	public void SetSelected(bool selected){
		this.selected = selected;

		if (selected) {
			PlayAnimation (1);
		}
	}

	public void SetBlock(bool blockMoves){
		this.containsBlockMoves = blockMoves;
	}

	public bool IsSelected()
	{
		return this.selected;
	}
}
