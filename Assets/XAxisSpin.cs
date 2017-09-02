/*
Graphics and Interactions (COMP30019) Project 1
Derek Chen, 766509
Kevin Liu, 766486
*/


//NOTE: used the x axis rotation script from LAB2, used to rotate sun around the origin (a game object)

using UnityEngine;
using System.Collections;

//
public class XAxisSpin : MonoBehaviour {
    
    public float spinSpeed;
        	
	// Update is called once per frame
	void Update () {
        this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, Vector3.right);
	}
}
