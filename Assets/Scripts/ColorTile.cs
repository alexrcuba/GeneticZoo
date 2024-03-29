using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTile : MonoBehaviour
{
	public int currentCol;
	public int currentRow;
	public int targetCol;
	public int targetRow;
	public bool matchCheck = false;
	private MatchFinder findingMatches;
	private BoardBehavior gameBoard;
	private GameObject otherColorTile;
	private Vector2 firstClick;
	private Vector2 lastClick;
	private Vector2 positionUpdate;
	public float clickAngle = 0;
	public float clickChecker = 1f;

	void Start(){
		gameBoard = FindObjectOfType<BoardBehavior>();
		findingMatches = FindObjectOfType<MatchFinder>();
		targetCol = (int)transform.position.x;
		targetRow = (int)transform.position.y;
		currentCol = targetCol;
		currentRow = targetRow;
	}

	void Update(){
		if(this.gameObject.tag != "Background"){
		if(matchCheck){
			SpriteRenderer renderColor = GetComponent<SpriteRenderer>();
			renderColor.color = new Color(1f, 1f, 1f, 0.5f);
		}
		}
	}

	private void OnMouseDown(){
		firstClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	private void OnMouseUp(){
		lastClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		FindAngle();
	}

	private void FindAngle(){
		if(Mathf.Abs(lastClick.y - firstClick.y) > clickChecker || Mathf.Abs(lastClick.x - firstClick.x) > clickChecker){
			clickAngle = Mathf.Atan2(lastClick.y - firstClick.y, lastClick.x - firstClick.x) * Mathf.Rad2Deg;
			PieceMovementCalc();
		}
 	}

	private void PieceMovementCalc(){
		if(clickAngle > -45 && clickAngle <= 45 && currentCol < gameBoard.width){//Right
			PieceMovementConfirm(1,0);
		}
		else if(clickAngle > 45 && clickAngle <= 135 && currentRow < gameBoard.height){//Up
			PieceMovementConfirm(0,1);
		}
		else if(clickAngle < -45 && clickAngle >= -135 && currentRow > 0){//Down
			PieceMovementConfirm(0,-1);
		}
		else if((clickAngle > 135 || clickAngle <= -135) && currentCol > 0){//Left
			PieceMovementConfirm(-1,0);
		}
	}

	private void PieceMovementConfirm(int movementCol, int movementRow){
		otherColorTile = gameBoard.allBoardTiles[currentCol + movementCol, currentRow + movementRow];
		if(otherColorTile != null){
			otherColorTile.GetComponent<ColorTile>().currentCol -= movementCol;
			otherColorTile.GetComponent<ColorTile>().currentRow -= movementRow;
			otherColorTile.GetComponent<ColorTile>().OtherPieceMovementConfirm();
			currentCol += movementCol;
			currentRow += movementRow;
			targetCol = currentCol;
			targetRow = currentRow;
			positionUpdate = new Vector2(targetCol, targetRow);
			transform.position = positionUpdate;
			name = targetCol + "," + targetRow;
			gameBoard.allBoardTiles[currentCol, currentRow] = this.gameObject;
			findingMatches.FindingAllMatches();
			StartCoroutine(ValidMoveCoroutine());
		}
	}

	public void OtherPieceMovementConfirm(){
		targetCol = currentCol;
		targetRow = currentRow;
		positionUpdate = new Vector2(targetCol, targetRow);
		transform.position = positionUpdate;
		name = targetCol + "," + targetRow;
		gameBoard.allBoardTiles[currentCol, currentRow] = this.gameObject;
		findingMatches.FindingAllMatches();
	}

	public void UpdateCurrentLocOnNewRow(){
		targetCol = (int)transform.position.x;
		targetRow = (int)transform.position.y;
		currentCol = targetCol;
		currentRow = targetRow;
	}

	public IEnumerator ValidMoveCoroutine(){
		yield return new WaitForSeconds(.5f);
		if(otherColorTile != null){
			if(!matchCheck && !otherColorTile.GetComponent<ColorTile>().matchCheck){
				if(clickAngle > -45 && clickAngle <= 45 && currentCol <= gameBoard.width){//Right
					PieceMovementConfirm(-1,0);
				}
				else if(clickAngle > 45 && clickAngle <= 135 && currentRow <= gameBoard.height){//Up
					PieceMovementConfirm(0,-1);
				}
				else if(clickAngle < -45 && clickAngle >= -135 && currentRow >= 0){//Down
					PieceMovementConfirm(0,1);
				}
				else if((clickAngle > 135 || clickAngle <= -135) && currentCol >= 0){//Left
					PieceMovementConfirm(1,0);
				}
			}
			else{
				gameBoard.CheckIfDestroy();
			}
			otherColorTile = null;
		}

	}
}
