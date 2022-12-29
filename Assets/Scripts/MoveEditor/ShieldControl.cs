using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldControl : MonoBehaviour {

	private float initialPosY;

	void Start()
	{
		this.initialPosY = transform.localPosition.y;
	}

	/// <summary>
	/// Updates the scale and position of a shield to match a move.
	/// </summary>
	/// <param name="move">Move.</param>
	public void UpdateScale(Move move)
	{
		Vector3 previousScale = transform.localScale;
		float shieldScalePercentage = move.GetStrength () / 100f;
		float minScale = Parameters.minShieldScale;
		float maxScale = Parameters.maxShieldScale;
		float newScale = minScale + (maxScale - minScale) * shieldScalePercentage; //Makes scale go [min - max].
		transform.localScale = new Vector3 (previousScale.x, newScale, previousScale.z);
	}
}
