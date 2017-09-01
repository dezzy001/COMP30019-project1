/*
Graphics and Interactions (COMP30019) Project 1
Derek Chen, 766509
Kevin Liu, 766486
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*sources: utilised sections of code to help with the mouse movements - https://forum.unity3d.com/threads/fly-cam-simple-cam-script.67042/
 * 			-lines 70 - 72 (mentioned again below)
 * */



public class flightSim : MonoBehaviour {

	//variable that references to the terrain public parameters
	private GameObject terrainObject;

	public float speed; // speed of the camera
	public float camSensitivity; //How sensitive the camera is with mouse movements

	private Vector3 prevMousePos;

	private float rollPos = 0.0f; //position of the camera's current roll
	public float rollSpeed;




	private float cameraOFFSET; // camera offset from the maximum height of the terrain

	private float boundarySize; // the boundary which camera resides in (based on the terrain)

	// have an offset for the boundary, to 100% make sure that camera can not go around it
	float BOUNDARYOFFSET = 0.5f; 


	//rigid body to add collision detection to camera, external physics will be removed from the camera later on the code
	public Rigidbody rb;

	void Start(){


		//get the terrain component and parameters
		GameObject terrainObject = GameObject.Find("terrain"); //see if the terrain object exists
		DiamondSquareTerrain terrain = terrainObject.GetComponent<DiamondSquareTerrain>(); // get the script

		boundarySize = terrain.size/2; //initialise boundarySize to half of the terrain size (since terrain is centered at origin)

		//cameras rigid body
		rb = GetComponent<Rigidbody>();

		cameraOFFSET = terrain.maxHeight * 2;
		//Initial camera view - position and rotation
		transform.position = new Vector3 (boundarySize, terrain.maxHeight+cameraOFFSET, boundarySize);
		transform.LookAt(new Vector3(15,0,0) );

		//make sure that the previous mouse position is initilaised to the current mouse position
		//ensures that camera view is synced with mouse position
		prevMousePos =  Input.mousePosition;
	}

	void Update () {


		//NOTE: use Time.deltaTime to make sure all movements have consistent frames per second

		/*mouse transformation - yaw and pitch*/
		//previous mouse coordinate = current mouse position - the previous mouse position

		/* https://forum.unity3d.com/threads/fly-cam-simple-cam-script.67042/
		 * Used code from the website above, to perform the previous mouse position calculations*/



	
		prevMousePos = Input.mousePosition - prevMousePos ;
		prevMousePos = new Vector3(-prevMousePos.y * camSensitivity, prevMousePos.x * camSensitivity, 0 );
		prevMousePos = new Vector3(transform.eulerAngles.x + prevMousePos.x ,transform.eulerAngles.y + prevMousePos.y , 0);

			//prevent the camera to fully rotate downwards, to prevent the camera to glitch out and flip the view 
		if (prevMousePos.x > 85 && prevMousePos.x > 0 && prevMousePos.x < 90) {
			prevMousePos.x = 85;
		}
				

			//do the actual rotations, based on the previous mouse movements
			transform.eulerAngles = prevMousePos;
			//get the new mouse position
			prevMousePos =  Input.mousePosition;


		/*keyboard transformation- roll and camera translation*/

		//camera translation (cannot go out of bounds)
		Vector3 keyboardDirection = getTranslateKeyInput(); //get the keyboard inputs

		keyboardDirection = keyboardDirection * Time.deltaTime *speed;

		//only allow translation if camera is within boundary size of the terrain
		transform.position = new Vector3(
			Mathf.Clamp (transform.position.x, -boundarySize+BOUNDARYOFFSET, boundarySize-BOUNDARYOFFSET),
			transform.position.y,
			Mathf.Clamp (transform.position.z, -boundarySize + BOUNDARYOFFSET, boundarySize - BOUNDARYOFFSET));

		//move (translate) the camera by the keyboard direction
		transform.Translate(keyboardDirection);


		//roll, rotate around x axis according to key press
		rollPos = getRotateKeyInput(rollPos);
		transform.Rotate(Vector3.forward, rollPos);


		// prevent any external forces affecting the rigid body (only want the camera to be affected by user controlls)
		rb.angularVelocity = Vector3.zero; //prevents external forces i.e collision to cause rotation
		rb.velocity = Vector3.zero; //prevents external forces i.e collision to cause translation

	}




	//returns the rotation position based on key press, adjusted for constant fps
	private float getRotateKeyInput(float rollPos) {
		//Roll
		if(Input.GetKey(KeyCode.Q))
		{
			//left roll
			rollPos += (rollSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.E))
		{
			//right roll
			rollPos -= (rollSpeed * Time.deltaTime);
		}
		return rollPos;
	}



	//returns the unit vector for camera velocity values based on keypress, or just a (0,0,0) vector if no key presses
	private Vector3 getTranslateKeyInput() {


		Vector3 velocity = new Vector3(0.0f , 0.0f, 0.0f);

		if (Input.GetKey (KeyCode.W)){ //move up
			velocity += new Vector3(0.0f, 0.0f , 1.0f);
		}
		if (Input.GetKey (KeyCode.S)){ //move down
			velocity += new Vector3(0.0f, 0.0f , -1.0f);
		}
		if (Input.GetKey (KeyCode.A)){ //move left
			velocity += new Vector3(-1.0f, 0.0f , 0.0f);
		}
		if (Input.GetKey (KeyCode.D)){ //move right
			velocity += new Vector3(1.0f,  0.0f , 0.0f);
		}

		return velocity;
	}



}
