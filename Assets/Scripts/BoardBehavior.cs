using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardBehavior : MonoBehaviour
{
	public int width;
	public int height;
	public int startingHeight;
	public int rowTimer;
	public GameObject boardTile;
	private GridTile[,] allBoardTiles;
	private int currentRowTimer;
	private Image countdownClock;
	
    void Start()
    {
        allBoardTiles = new GridTile[width,height];
		currentRowTimer = rowTimer;
		countdownClock = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
		SetUp();
    }
	
	void Update()
	{
		currentRowTimer--;
		/*if((currentRowTimer % 1000) == 0){
			Debug.Log(currentRowTimer);
		}*/
		//Debug.Log((float)currentRowTimer/(float)rowTimer);
		countdownClock.fillAmount = 1-((float)currentRowTimer/(float)rowTimer);
		if(currentRowTimer == 0){
			addNewRow();
			currentRowTimer = rowTimer;
		}
	}

    void SetUp()
    {
        for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				Vector2 tempPosition = new Vector2(x,y);
				GameObject tile = Instantiate(boardTile, tempPosition, Quaternion.identity) as GameObject;
				tile.transform.parent = this.transform;
				tile.name = x + "," + y;
				if(y < startingHeight){
					tile.GetComponent<GridTile>().CreateColor();
				}
			}
		}
    }
	
	void addNewRow(){
		for(int x = (width-1); x >= 0; x--){
			for(int y = (height-1); y >= 0; y--){
				var name = x + "," + y;
				var newParent = x + "," + (y+1);
				//Debug.Log(name);
				//Debug.Log(newParent);
				Transform currentPiece = this.transform.Find(name + "/" + name);
				Transform parentPiece = this.transform.Find(newParent);
				if(currentPiece != null){
					if((y+1) >= height){
						Debug.Log("GAME OVER!");
						//Eventually will add game over Logic!
						break;
					}
					currentPiece.parent = parentPiece.transform;
					currentPiece.transform.position = currentPiece.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
					this.transform.Find(newParent + "/" + name).name = newParent;
					if(y == 0){
						this.transform.Find(name).GetComponent<GridTile>().CreateColor();
					}
				}
			}
		}
	}
}
