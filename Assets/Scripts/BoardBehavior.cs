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
	public List<GameObject> colors;
	public GameObject[,] allBoardTiles;
	public AudioSource music;
	private int currentRowTimer;
	private Image countdownClock;
	private  Vector2 tempPosition;
	private float pitchChange;
	private string[] listofColors = {"Red", "Orange", "Yellow", "Blue", "Green", "Purple"};
	private bool gameOverCheck = false;
	private bool rowLogic = false;
	
    void Start()
    {
		pitchChange = 1;
        allBoardTiles = new GameObject[width,height];
		currentRowTimer = rowTimer;
		countdownClock = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
		SetUp();
    }
	
	void Update()
	{
		if(!gameOverCheck){
			currentRowTimer--;
			countdownClock.fillAmount = 1-((float)currentRowTimer/(float)rowTimer);
			if(currentRowTimer == 0){
				addNewRow();
				if(rowTimer > 1000){
					rowTimer -= 1000;
				}
				currentRowTimer = rowTimer;
			}
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
		List<GameObject> tempColors = new List<GameObject>(colors);
		int colorToUse = Random.Range(0, tempColors.Count);
		while(startingMatches(x,y,colors[colorToUse])){
			tempColors.RemoveAt(colorToUse);
			colorToUse = Random.Range(0, tempColors.Count);
		}
		GameObject color = Instantiate(colors[colorToUse], tempPosition, Quaternion.identity);
		color.transform.parent = this.transform;
		color.name = x + "," + y;
		allBoardTiles[x,y] = color;
	}

	void addNewRow(){
		rowLogic = true;
		var pitchBendGroup = Resources.Load<UnityEngine.Audio.AudioMixerGroup>("SpeedUpSlowDown");
		music.outputAudioMixerGroup = pitchBendGroup;
		for(int x = 0; x < width; x++){
			for(int y = (height-1); y >= 0; y--){
				var name = x + "," + y;
				var newName = x + "," + (y+1);
				GameObject currentPiece = GameObject.Find(name);
				foreach( string currentColor in listofColors){
					if(currentPiece != null && currentPiece.tag == currentColor && !gameOverCheck){
						if((y+1) >= height){
							Debug.Log("GAME OVER!");
							//Eventually will add game over Logic!
							pitchChange = 0.5f;
							music.pitch = pitchChange;
							pitchBendGroup.audioMixer.SetFloat("pitchBend", 1f / pitchChange);
							gameOverCheck = true;
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
						pitchChange += 0.0015f;
						music.pitch = pitchChange;
						pitchBendGroup.audioMixer.SetFloat("pitchBend", 1f / pitchChange);
					}
				}
			}
		}
	}

	private bool startingMatches(int col, int row, GameObject color){
		if(rowLogic){
			if(col > 1 && allBoardTiles[col-1, row].tag == color.tag && allBoardTiles[col-2, row].tag == color.tag){
				return true;
			}
			if(allBoardTiles[col, row+1].tag == color.tag && allBoardTiles[col, row+2].tag == color.tag){
				return true;
			}
		}
		else if(col > 1 && row > 1){
			if(allBoardTiles[col-1, row].tag == color.tag && allBoardTiles[col-2, row].tag == color.tag){
				return true;
			}
			if(allBoardTiles[col, row-1].tag == color.tag && allBoardTiles[col, row-2].tag == color.tag){
				return true;
			}
		}
		else if (col > 1)
		{
			if (allBoardTiles[col-1, row].tag == color.tag && allBoardTiles[col-2, row].tag == color.tag){
				return true;
			}
		}
		else if( row > 1){
			if (allBoardTiles[col, row-1].tag == color.tag && allBoardTiles[col, row-2].tag == color.tag){
				return true;
			}
		}
        return false;
	}

	private void DestroyMatch(int col, int row){
		if(allBoardTiles[col, row].GetComponent<ColorTile>().matchCheck){
			Destroy(allBoardTiles[col, row]);
			allBoardTiles[col, row] = null;
		}
	}

	public void CheckIfDestroy(){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				if(allBoardTiles[x, y] != null){
					DestroyMatch(x,y);
				}
			}
		}
		StartCoroutine(RemoveOldRow());
	}

	private IEnumerator RemoveOldRow(){
		int emptyCount = 0;
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				if(allBoardTiles[x, y] == null){
					emptyCount++;
				}
				else if(emptyCount > 0){
					var name = x + "," + y;
					var newName = x + "," + (y-emptyCount);
					GameObject currentPiece = GameObject.Find(name);
					if(currentPiece != null){
						currentPiece.transform.Translate(0,-emptyCount,0);
						currentPiece.name = newName;
						allBoardTiles[x,(y-emptyCount)] = currentPiece;
						currentPiece.GetComponent<ColorTile>().UpdateCurrentLocOnNewRow();
					}
				}
			}
			emptyCount = 0;
		}
		yield return new WaitForSeconds(.4f);
	}

}
