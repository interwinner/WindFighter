using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for handling the dragging and dropping of limbs in the MoveEditor.
/// </summary>
public class DragAndDrop : MonoBehaviour {

	private bool mouseDown;

	private bool outHardDown;
	private bool outHardUp;

	private bool outFrameDown;
	private bool outFrameUp;

	private bool outHigh;
	private bool outLow;

	private float highFrameTwistLimit;
	private float lowFrameTwistLimit;
	public float frameTwistLimit;

	public float highHardTwistLimit;
	public float lowHardTwistLimit;
	private int pivotRotationOffset;

	private GameObject highFrameLimitMarker;
	private GameObject lowFrameLimitMarker;
	private GameObject highHardLimitMarker;
	private GameObject lowHardLimitMarker;

	private string[] dragPointNames;

	private bool selected;
	private float scrollDelay;

	void Awake()
	{
		this.selected = false;
		highFrameLimitMarker = GameObject.Find ("HighFrameLimitMarker");
		lowFrameLimitMarker = GameObject.Find ("LowFrameLimitMarker");
		highHardLimitMarker = GameObject.Find ("HighHardLimitMarker");
		lowHardLimitMarker = GameObject.Find ("LowHardLimitMarker");
		this.dragPointNames = new string[] {
			"TorsoDragPoint",
			"HeadDragPoint",
			"UpperRightArmDragPoint",
			"LowerRightArmDragPoint",
			"RightHandDragPoint",
			"UpperLeftArmDragPoint",
			"LowerLeftArmDragPoint",
			"LeftHandDragPoint",
			"UpperRightLegDragPoint",
			"LowerRightLegDragPoint",
			"RightFootDragPoint",
			"UperLeftLegDragPoint",
			"LowerLeftLegDragPoint",
			"LeftFootDragPoint"
		};
	}

	void Start()
	{
		UpdateFrameLimits ();
		pivotRotationOffset = 180; // Standard rotation of unity is pivot up. Sprite sheet has pivot point down.
	}

    void OnMouseDown()
	{
		mouseDown = true;
		Select ();
	}

	private void EnableMarker(GameObject marker, float rotation)
	{
		marker.transform.position = transform.parent.position;
		marker.transform.parent = transform.parent.parent;
		marker.transform.localEulerAngles = new Vector3 (0, 0, rotation + pivotRotationOffset);
		marker.SetActive (true);
	}

	void OnMouseUp()
	{
		mouseDown = false;
	}

	/// <summary>
	/// Updates the twist limits to fit the new angle of the moved bodypart.
	/// </summary>
	public void UpdateFrameLimits()
	{
		float previousRotation = transform.parent.localEulerAngles.z;
		//Mod 360 to make sure both limits are within [0 - 360]
		highFrameTwistLimit = (previousRotation + frameTwistLimit) % 360;
		lowFrameTwistLimit = (previousRotation - frameTwistLimit + 360) % 360;
	}

	/// <summary>
	/// Converts the mouse pointer world rotation to the local rotation of the bodypart pivot.
	/// </summary>
	/// <returns>The local rotation of the pointer around the bodypart pivot.</returns>
	float LocalMouseRotation()
	{
		// Position relative to current camera bounds.
		Vector3 mousePos = Input.mousePosition;
		Vector3 parentPos = Camera.main.WorldToScreenPoint(transform.parent.position);

		bool mouseXGreater = mousePos.x > parentPos.x;
		bool mouseYGreater = mousePos.y > parentPos.y;

		float arctan = 0f;
		float newRot = 0f;

		// 1st quadrant
		if(mouseXGreater && mouseYGreater)  
		{
			arctan = Mathf.Atan2(mousePos.y - parentPos.y, mousePos.x - parentPos.x);
			newRot = Mathf.Rad2Deg * arctan + 90;
		}
		// 2nd quadrant
		else if (!mouseXGreater && mouseYGreater) 
		{
			arctan = Mathf.Atan2(parentPos.x - mousePos.x, mousePos.y - parentPos.y);
			newRot = Mathf.Rad2Deg * arctan + 180;
		}
		// 3rd quadrant
		else if (!mouseXGreater && !mouseYGreater) 
		{
			arctan = Mathf.Atan2(parentPos.y - mousePos.y, parentPos.x - mousePos.x);
			newRot = Mathf.Rad2Deg * arctan + 270;
		}
		// 4th quadrant
		else 
		{
			arctan = Mathf.Atan2(mousePos.x - parentPos.x, parentPos.y - mousePos.y);
			newRot = Mathf.Rad2Deg * arctan;
		}
		return newRot;
	}

	/// <summary>
	/// Main method for getting the mouse position, regognizing a click, and moving the relevant bodypart accordingly.
	/// </summary>
	void OnMouseDrag() 
	{
		if (mouseDown) 
		{ 
			float newRot = LocalMouseRotation ();
            //TODO: Look at twist limits based on mouse position and not one test update.

			newRot += pivotRotationOffset;

			UpdateRotation (newRot);
        }
	}

	void Update()
	{
		float horizontal1 = Input.GetAxisRaw ("Horizontal");
		float vertical1 = Input.GetAxisRaw ("Vertical");
		float horizontal2 = Input.GetAxisRaw ("Horizontal2");
		float vertical2 = Input.GetAxisRaw ("Vertical2");

		if (Input.GetMouseButtonDown (0) && !mouseDown) {
			Deselect ();
		}

		if (scrollDelay > 0) {
			scrollDelay -= Time.deltaTime;
		} else {
			if (selected && !mouseDown) {
				if (UnityUtils.AnyInputUp()) {
					//scrollDelay = Parameters.scrollDelay;
					AddRotation (1);
				} else if (UnityUtils.AnyInputDown()) {
					//scrollDelay = Parameters.scrollDelay;
					AddRotation (-1);
				} else if (horizontal1 > 0 || horizontal2 > 0 || Input.GetButtonDown("Controller1Button5") || Input.GetButtonDown("Controller2Button5")) {
					SelectNextDragPoint ();
				} else if (horizontal1 < 0 || horizontal2 < 0 || Input.GetButtonDown("Controller1Button4") || Input.GetButtonDown("Controller2Button4")) {
					SelectPreviousDragPoint ();
				}
			}
		}
	}

	public void SelectNextDragPoint()
	{
		SelectOtherDragPoint (1);
	}

	public void SelectPreviousDragPoint()
	{
		SelectOtherDragPoint (-1);
	}

	public void SelectOtherDragPoint(int offset)
	{
		string nextDragpointName = null;
		for (int i = 0; i < dragPointNames.Length; i++) {
			if (dragPointNames [i].Equals (transform.name)) {
				int nextIndex = (i + offset);
				while (nextIndex < 0) {
					nextIndex += dragPointNames.Length;
				}
				nextIndex %= dragPointNames.Length;
				nextDragpointName = dragPointNames [nextIndex];
			}
		}

		if (nextDragpointName != null) {
			GameObject nextDragpoint = GameObject.Find (nextDragpointName);
			DragAndDrop dragAndDrop = nextDragpoint.GetComponent<DragAndDrop> ();
			if (dragAndDrop != null) {
				this.Deselect ();
				dragAndDrop.Select ();
			}
		}
	}

	public void AddRotation(float degrees)
	{
		float rotation = transform.parent.eulerAngles.z;
		float newRot = rotation + degrees;
		UpdateRotation (newRot);
	}

	public void UpdateRotation(float newRot)
	{
		transform.parent.eulerAngles = new Vector3(0, 0, newRot); //Update rotation previous to checking limits

		//Find biggest of low limits and smallest of high limis to create the smallest allowed intervall
		float tmpLowLimit = RotationUtils.InCounterClockwiseLimits (lowFrameTwistLimit, lowHardTwistLimit, highHardTwistLimit) ? lowFrameTwistLimit : lowHardTwistLimit;
		float tmpHighLimit = RotationUtils.InCounterClockwiseLimits (highFrameTwistLimit, lowHardTwistLimit, highHardTwistLimit) ? highFrameTwistLimit : highHardTwistLimit;

		float rotation = transform.parent.localEulerAngles.z;

		if (RotationUtils.InCounterClockwiseLimits (rotation, tmpLowLimit, tmpHighLimit)) 
		{ 
			//Angle in limit
			outHigh = false;
			outLow = false;	
		}
		else 
		{
			if (outHigh) //Rotation is still outside limits to one side
			{
				transform.parent.localEulerAngles = new Vector3 (0, 0, tmpHighLimit);
			}
			else if (outLow) //Rotation is still outside limits to the other side
			{ 
				transform.parent.localEulerAngles = new Vector3 (0, 0, tmpLowLimit);
			}
			// Check to which side rotation has exited the limits intervall.
			// If it is not yet in one of the halves of the outside of the twist limits, wait until it is.
			else if (RotationUtils.InCounterClockwiseLimits(rotation, RotationUtils.MiddleOfRotations (tmpHighLimit, tmpLowLimit), tmpLowLimit))
			{
				outLow = true;
				transform.parent.localEulerAngles = new Vector3 (0, 0, tmpLowLimit);
			}
			else if (RotationUtils.InCounterClockwiseLimits(rotation, tmpLowLimit, RotationUtils.MiddleOfRotations (tmpHighLimit, tmpLowLimit)))
			{
				outHigh = true;
				transform.parent.localEulerAngles = new Vector3 (0, 0, tmpHighLimit);
			}
		}
	}

	public void Select()
	{
		this.selected = true;
		this.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, 255);
		scrollDelay = Parameters.scrollDelay;
		EnableMarker (highFrameLimitMarker, highFrameTwistLimit);
		EnableMarker (lowFrameLimitMarker, lowFrameTwistLimit);
		EnableMarker (highHardLimitMarker, highHardTwistLimit);
		EnableMarker (lowHardLimitMarker, lowHardTwistLimit);
	}

	public void Deselect()
	{
		this.selected = false;
		this.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, 255);
		scrollDelay = Parameters.scrollDelay;
	}
}
