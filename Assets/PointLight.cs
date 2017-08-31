/*
Graphics and Interactions (COMP30019) Project 1
Derek Chen, 766509
Kevin Liu, 766486
*/

using UnityEngine;
using System.Collections;

//NOTE: used the PointLight script from lab 4, adjusted initial position of the point light

public class PointLight : MonoBehaviour {

    public Color color;


	void Start(){
		//get the terrain component and parameters
		GameObject terrainObject = GameObject.Find("terrain"); //see if the terrain object exists
		DiamondSquareTerrain terrain = terrainObject.GetComponent<DiamondSquareTerrain>(); // get the script

		//offset of the sun away from the terrain boundaries
		float sizeOffset = 10.0f;

		transform.position = new Vector3 (0.0f, 0.0f, terrain.size/2 + sizeOffset);


	}

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }
}
