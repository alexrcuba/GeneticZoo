using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTile : MonoBehaviour
{
	private Vector2 firstClick;
	private Vector2 lastClick;
	public float clickAngle = 0;

	private void OnMouseDown(){
		firstClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	private void OnMouseUp(){
		lastClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		FindAngle();
	}

	void FindAngle(){
		clickAngle = Mathf.Atan2(lastClick.y - firstClick.y, lastClick.x - firstClick.x) * Mathf.Rad2Deg;
		Debug.Log(firstClick);
		Debug.Log(lastClick);
		Debug.Log(clickAngle);
 	}
}
