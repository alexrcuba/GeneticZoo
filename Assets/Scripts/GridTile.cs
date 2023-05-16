using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
	public GameObject[] colors;
	
    public void CreateColor(){
		int colorToUse = Random.Range(0, colors.Length);
		GameObject color = Instantiate(colors[colorToUse], transform.position, Quaternion.identity);
		color.transform.parent = this.transform;
		color.name = this.name;
	}
}
