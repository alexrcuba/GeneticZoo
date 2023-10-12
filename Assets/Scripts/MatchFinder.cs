using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{   
    private BoardBehavior gameBoard;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<BoardBehavior>();
    }

	public void FindingAllMatchesBoard(){
		StartCoroutine(lookForMatches((matchFindValue) => {
			if(matchFindValue){
				Debug.Log("MATCHFIND: " + matchFindValue);
				gameBoard.GetComponent<BoardBehavior>().foundMatch = true;
			}
		}));
	}

	public void FindingAllMatches(){
		StartCoroutine(lookForMatches((matchFindValue) => {}));
	}

    private IEnumerator lookForMatches(System.Action<bool> callback){
		bool matchFind = false;
		for(int x = 0; x < gameBoard.width; x++){
			for(int y = 0; y < gameBoard.height; y++){
				GameObject currentColor = gameBoard.allBoardTiles[x,y];
				if(currentColor != null){
					if(x > 0 && x < gameBoard.width - 1){
						GameObject leftColor = gameBoard.allBoardTiles[x - 1, y];
						GameObject rightColor = gameBoard.allBoardTiles[x + 1, y];
						if(leftColor != null && rightColor != null && currentColor.GetComponent<ColorTile>().currentCol - 1 == leftColor.GetComponent<ColorTile>().currentCol && currentColor.GetComponent<ColorTile>().currentCol + 1 == rightColor.GetComponent<ColorTile>().currentCol && leftColor.tag == currentColor.tag && rightColor.tag == currentColor.tag){
							leftColor.GetComponent<ColorTile>().matchCheck = true;
							rightColor.GetComponent<ColorTile>().matchCheck = true;
							currentColor.GetComponent<ColorTile>().matchCheck = true;
							matchFind = true;
						}
					}
					if(y > 0 && y < gameBoard.height - 1){
						GameObject aboveColor = gameBoard.allBoardTiles[x, y + 1];
						GameObject belowColor = gameBoard.allBoardTiles[x, y - 1];
						if(aboveColor != null && belowColor != null && currentColor.GetComponent<ColorTile>().currentRow + 1 == aboveColor.GetComponent<ColorTile>().currentRow && currentColor.GetComponent<ColorTile>().currentRow - 1 == belowColor.GetComponent<ColorTile>().currentRow && aboveColor.tag == currentColor.tag && belowColor.tag == currentColor.tag){
							aboveColor.GetComponent<ColorTile>().matchCheck = true;
							belowColor.GetComponent<ColorTile>().matchCheck = true;
							currentColor.GetComponent<ColorTile>().matchCheck = true;
							matchFind = true;
						}
					}
				}
			}
		}
		callback(matchFind);
		yield return new WaitForSeconds(.2f);	
	}
}
