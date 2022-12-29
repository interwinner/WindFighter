using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
//using UnityEditor;

/// <summary>
/// Used for manipulating the panels next to each character showing the currently selected moves.
/// </summary>
public class SelectionPanelBahviour : MonoBehaviour
{
	private List<GameObject> panels;
	private Character owner;
	private bool inited;

	/// <summary>
	/// Init this instance. Adds slots for the buttons of the current owner to the list of moves.
	/// This is init and not start because the owner is not set form the start.
	/// </summary>
	void Init ()
	{
		//TODO: In case we ever set owner twice, we would have to delete all children of the panel.
		this.inited = true;
		panels = new List<GameObject> ();
		List<string> character1Buttons = InputSettings.GetCharacterButtons (owner.GetNbr ());
		foreach (string characterButton in character1Buttons)
		{
			string previewPath = "Prefabs" + Path.DirectorySeparatorChar + "MovePanel";
			string previewCharacterPath = "Prefabs" + Path.DirectorySeparatorChar + "Character";

			GameObject previewPanelObject = (GameObject)Resources.Load (previewPath);
			GameObject previewCharacterObject = (GameObject)Resources.Load(previewCharacterPath);
			previewCharacterObject.GetComponent<CharacterCollisionDetector>().enabled = false;
			GameObject previewPanel = Instantiate (previewPanelObject, previewPanelObject.transform.position, previewPanelObject.transform.rotation, transform);

			if (owner.GetNbr () == 1)
			{
				foreach (Transform child in previewPanel.transform)
				{
					//Flip all text fields.
					//The transform of player 1 is flipped. So if all children are not flipped as well, the text is reversed.
					child.GetComponent<RectTransform> ().localScale = new Vector3 (-1, 1, 1);
				}
			}

			//Make selection panel list entries have the default button sprite to make them look less like an interactable list.
			//string defaultButtonPath = "UI/Skin/Background.psd";
			string defaultButtonPath = "Art/Main Menu/button_up_test";
			if (System.IO.File.Exists(defaultButtonPath)){
				Debug.Log("foooound! file ");
			}
			//Sprite defaultButtonSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (defaultButtonPath);
			Sprite defaultButtonSprite = Resources.Load<Sprite>(defaultButtonPath);
			previewPanel.GetComponent<Image> ().sprite = defaultButtonSprite;

			ResetText (previewPanel);

			foreach (Transform child in previewPanel.transform) {
				Text text = child.GetComponent<Text> ();
				if (text != null) {
					text.fontSize = 24;
				}
			}

			GameObject previewCharacter = Instantiate ( previewCharacterObject, previewCharacterObject.transform.position, previewCharacterObject.transform.rotation, previewPanel.transform );

			previewPanel.GetComponent<MovePanelBehaviour>().addPreviewCharacter(previewCharacter);

			float characterPosition;

			if (owner.GetNbr() == 1) 
			{
				characterPosition = 2.4f;
			} else 
			{
				characterPosition = -2.4f;
			}

			previewCharacter.transform.position = new Vector2(characterPosition * previewPanel.transform.localScale.x, 0);
			previewCharacter.transform.localScale = new Vector3(3 * -previewPanel.transform.localScale.x, 3, 3);

			previewCharacter.SetActive(false);

			MovePanelBehaviour panelBehaviour = previewPanel.GetComponent<MovePanelBehaviour> ();
			if (owner != null)
			{
				string characterButtonDisplayName = characterButton;
				if (characterButtonDisplayName.Length > 1) {
					characterButtonDisplayName = UnityUtils.ControllerButtonToDisplayName(characterButtonDisplayName);
				}
				panelBehaviour.AssignButton (characterButtonDisplayName, owner.GetColor (), 2);
			}
		}
	}

	/// <summary>
	/// Adds the move to the list of selected moves in the slot with the correct button.
	/// </summary>
	/// <param name="button">Button.</param>
	/// <param name="move">Move.</param>
	public void AddMove(string button, Move move)
	{
		foreach (Transform child in transform)
		{
			MovePanelBehaviour panelBehaviour = child.GetComponent<MovePanelBehaviour> ();

			//Panel has to be a move panel.
			if (panelBehaviour == null) {
				continue;
			}
			if (panelBehaviour.GetAssignedButton () == null) {
				continue;
			}

			//Add move to correct panel and remove it from any other panel.
			if (panelBehaviour.GetAssignedButton ().Equals (button)) {
				RemovePanelWithMove (move.GetName ()); //Remove the move from any panel currently holding the move.
				panelBehaviour.transform.Find("NameText").GetComponent<Text>().color = new Color(0, 0, 0, 1);
				panelBehaviour.setMove (move);
			}
		}
	}

	/// <summary>
	/// Clears move from any panel that has a specific move.
	/// </summary>
	/// <param name="moveName">Move name.</param>
	public void RemovePanelWithMove(string moveName)
	{
		//Cannot remove items from list while itterating.
		//Only one panel at a time can hold the same move. This fins it if it exists.
		Transform panelWithMove = null;
		foreach(Transform panel in transform)
		{
			MovePanelBehaviour panelBehaviour = panel.GetComponent<MovePanelBehaviour> ();
			Move panelMove = panelBehaviour.getMove ();
			if (panelMove == null) {
				continue; // Skip panel if it does not have a move.
			}
			string panelMoveName = panelMove.GetName ();
			if (panelMoveName.Equals (moveName))
			{
				panelWithMove = panel;
				break; //Quit loop when the correct panel is found.
			}
		}
		if (panelWithMove != null)
		{
			//Clear the panel.
			panelWithMove.GetComponent<MovePanelBehaviour>().setMove(null);
			panelWithMove.GetComponent<MovePanelBehaviour> ().RemoveSpeedText ();
			panelWithMove.GetComponent<MovePanelBehaviour> ().RemoveStrengthText ();
			panelWithMove.GetComponent<MovePanelBehaviour> ().RemoveNameText ();
			ResetText (panelWithMove.gameObject);

			GameObject previewCharacter = panelWithMove.GetComponent<MovePanelBehaviour>().getPreviewCharacter();
			if (previewCharacter != null)
			{
				previewCharacter.SetActive(false);
				previewCharacter.GetComponent<MovePlayer>().ShowActiveBodypart(false);
				previewCharacter.GetComponent<MovePlayer>().setMoveToPlay(null);
			}
		}
	}

	public void SetOwner(Character character)
	{
		this.owner = character;
		//The first time owner is set, add that owners' buttons to the slots of this list.
		if (!inited && character != null && character.GetColor () != null)
		{
			this.Init ();
		}
	}

	public Character GetOwner()
	{
		return this.owner;
	}

	private void ResetText(GameObject panel)
	{
		panel.transform.Find ("NameText").GetComponent<Text> ().text = "Empty";
		panel.transform.Find ("NameText").GetComponent<Text> ().color = new Color (0.5f, 0, 0, 255);
	}
}
