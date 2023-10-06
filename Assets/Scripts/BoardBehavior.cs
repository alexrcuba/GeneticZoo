using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardBehavior : MonoBehaviour
{
	public int width;
	public int height;
	public int startingHeight;
	public int rowTimer;
	public GameObject boardTile;
	public int scoreGoal;
	public List<GameObject> colors;
	public GameObject[,] allBoardTiles;
	public AudioSource music;
	private int currentRowTimer;
	private Image countdownClock;
	private TextMeshProUGUI scoreText;
	private  Vector2 tempPosition;
	private float pitchChange;
	private string[] listofColors = {"Red", "Orange", "Yellow", "Blue", "Green", "Purple"};
	private bool gameOverCheck = false;
	private bool gameWinCheck = false;
	private bool rowLogic = false;
	Dictionary<string, int> TilesInMatch = new Dictionary<string, int>(){{"Red",0}, {"Orange",0}, {"Yellow",0}, {"Blue",0}, {"Green",0}, {"Purple",0}};
	Dictionary<string, int> PointsPerValue = new Dictionary<string, int>(){{"Red",0}, {"Orange",0}, {"Yellow",0}, {"Blue",0}, {"Green",0}, {"Purple",0}};
	int totalScore = 0;

    void Start()
    {
		pitchChange = 1;
        allBoardTiles = new GameObject[width,height];
		currentRowTimer = rowTimer;
		countdownClock = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
		scoreText = this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
		SetUp();
    }
	
	void Update()
	{
		if(!gameWinCheck && !gameOverCheck){
			currentRowTimer--;
			countdownClock.fillAmount = 1-((float)currentRowTimer/(float)rowTimer);
			if(currentRowTimer == 0){
				addNewRow();
				if(rowTimer > 1000){
					rowTimer -= 250;
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
			scoreText.SetText(
			"TOTAL SCORE: " + totalScore + " / " + scoreGoal + 
			"\n\n" + listofColors[0] + " Score: " + PointsPerValue[listofColors[0]] +
			"\n" + listofColors[1] + " Score: " + PointsPerValue[listofColors[1]] +
			"\n" + listofColors[2] + " Score: " + PointsPerValue[listofColors[2]] +
			"\n" + listofColors[3] + " Score: " + PointsPerValue[listofColors[3]] +
			"\n" + listofColors[4] + " Score: " + PointsPerValue[listofColors[4]] +
			"\n" + listofColors[5] + " Score: " + PointsPerValue[listofColors[5]]
			);
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
							scoreText.SetText("GAME OVER!");
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
						pitchChange += 0.0005f;
						music.pitch = pitchChange;
						pitchBendGroup.audioMixer.SetFloat("pitchBend", 1f / pitchChange);
					}
				}
			}
		}
	}

	private bool startingMatches(int col, int row, GameObject color){
		if(rowLogic){
			if(col > 1 &&  allBoardTiles[col-1, row] != null && allBoardTiles[col-1, row].tag == color.tag && allBoardTiles[col-2, row] != null && allBoardTiles[col-2, row].tag == color.tag){
				return true;
			}
			if(allBoardTiles[col, row+1] != null && allBoardTiles[col, row+1].tag == color.tag && allBoardTiles[col, row+2] != null && allBoardTiles[col, row+2].tag == color.tag){
				return true;
			}
		}
		else if(col > 1 && row > 1){
			if(allBoardTiles[col-1, row] != null && allBoardTiles[col-1, row].tag == color.tag && allBoardTiles[col-2, row] != null && allBoardTiles[col-2, row].tag == color.tag){
				return true;
			}
			if(allBoardTiles[col, row-1] != null && allBoardTiles[col, row-1].tag == color.tag && allBoardTiles[col, row-2] != null && allBoardTiles[col, row-2].tag == color.tag){
				return true;
			}
		}
		else if (col > 1)
		{
			if (allBoardTiles[col-1, row] != null && allBoardTiles[col-1, row].tag == color.tag && allBoardTiles[col-2, row] != null && allBoardTiles[col-2, row].tag == color.tag){
				return true;
			}
		}
		else if( row > 1){
			if (allBoardTiles[col, row-1] != null && allBoardTiles[col, row-1].tag == color.tag && allBoardTiles[col, row-2] != null && allBoardTiles[col, row-2].tag == color.tag){
				return true;
			}
		}
        return false;
	}

	private void DestroyMatch(int col, int row){
		if(allBoardTiles[col, row].GetComponent<ColorTile>().matchCheck){
			TilesInMatch[allBoardTiles[col,row].GetComponent<ColorTile>().tag] += 1;
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
		foreach(string color in listofColors){
			//Debug.Log(color + ": " + TilesInMatch[color]);
			while(TilesInMatch[color] > 0){
				if(TilesInMatch[color] > 5){
					PointsPerValue[color] += 4;
					totalScore += 4;
				}
				else if(TilesInMatch[color] == 5){
					PointsPerValue[color] += 3;
					totalScore += 3;
				}
				else if(TilesInMatch[color] == 4){
					PointsPerValue[color] += 2;
					totalScore += 2;
				}
				else if(TilesInMatch[color] <= 3){
					PointsPerValue[color] += 1;
					totalScore += 1;
				}
				TilesInMatch[color] -= 1;
			};
		}
		Debug.Log("Total Score: " + totalScore);
		scoreText.SetText(
		"TOTAL SCORE: " + totalScore + " / " + scoreGoal + 
		"\n\n" + listofColors[0] + " Score: " + PointsPerValue[listofColors[0]] +
		"\n" + listofColors[1] + " Score: " + PointsPerValue[listofColors[1]] +
		"\n" + listofColors[2] + " Score: " + PointsPerValue[listofColors[2]] +
		"\n" + listofColors[3] + " Score: " + PointsPerValue[listofColors[3]] +
		"\n" + listofColors[4] + " Score: " + PointsPerValue[listofColors[4]] +
		"\n" + listofColors[5] + " Score: " + PointsPerValue[listofColors[5]]
		);
		if(totalScore >= scoreGoal){
			scoreText.SetText("YOU WIN!");
			gameWinCheck = true;
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
