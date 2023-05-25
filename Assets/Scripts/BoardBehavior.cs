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
	public GameObject[] colors;
	public GameObject[,] allBoardTiles;
	private int currentRowTimer;
	private Image countdownClock;
	private  Vector2 tempPosition;

	
    void Start()
    {
        allBoardTiles = new GameObject[width,height];
		currentRowTimer = rowTimer;
		countdownClock = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
		SetUp();
    }
	
	void Update()
	{
		currentRowTimer--;
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
				tempPosition = new Vector2(x,y);
				GameObject tile = Instantiate(boardTile, tempPosition, Quaternion.identity) as GameObject;
				tile.transform.parent = this.transform;
				tile.name = "(" + x + "," + y + ")";
				if(y < startingHeight){
					CreateColor(x,y);
				}
			}
		}
    }
	
	void CreateColor(int x, int y){
		int colorToUse = Random.Range(0, colors.Length);
		GameObject color = Instantiate(colors[colorToUse], tempPosition, Quaternion.identity);
		color.transform.parent = this.transform;
		color.name = x + "," + y;
		allBoardTiles[x,y] = color;
	}

	void addNewRow(){
		for(int x = (width-1); x >= 0; x--){
			for(int y = (height-1); y >= 0; y--){
				var name = x + "," + y;
				var newName = x + "," + (y+1);
				GameObject currentPiece = GameObject.Find(name);
				if(currentPiece != null && currentPiece.tag == "Color"){
					if((y+1) >= height){
						Debug.Log("GAME OVER!");
						//Eventually will add game over Logic!
						break;
					}
					currentPiece.transform.Translate(0,1,0);
					currentPiece.name = newName;
					allBoardTiles[x,(y+1)] = currentPiece;
					currentPiece.GetComponent<ColorTile>().UpdateCurrentLocOnNewRow();
					if(y == 0){
						tempPosition = new Vector2(x,y);
						CreateColor(x,y);
					}
				}
			}
		}
	}
}
