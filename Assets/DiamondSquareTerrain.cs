/*
Graphics and Interactions (COMP30019) Project 1
Derek Chen, 766509
Kevin Liu, 766486
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*sources: Unity Tutorials: 
 1) Diamond-Square Procedural Terrain - https://www.youtube.com/watch?v=1HV8GbFnCik
(NOTE: tightly followed this tutorial for understanding of the diamond square algorithm)

2) Diamond-square algorithm - https://en.wikipedia.org/wiki/Diamond-square_algorithm

LAB 4 - used the phong shader script,the pointLight script and took some code snippets
		from cubescript to utilise the phong shader and point light from lab 4

*/

public class DiamondSquareTerrain : MonoBehaviour {
	

	//number of divisions (number of faces for the length or/hence the width) 
	public int numDivisions; //(NOTE: always has to be a power of 2 )
	//size of the terrain
	public float size;


	public float maxHeight; //the maximum height possible for the terrain
	private float height; //current height of the terrain
	private float currMaxHeight; //current maximum height of terrain after the terrain is generated

	//holds all the vertecies in an array
	private Vector3[] verts;
	//the count of verticies in the array
	private int numVerts;

	//mesh collider for the terrain
	MeshCollider meshCollider;

	//reference this to the phong shader
	public Shader shader;
	public PointLight pointLight;

	// Use this for initialization
	void Start () {

		//terrain height is initially equal to max possible height 
		height = maxHeight;

		currMaxHeight = -maxHeight; // initilaise the to be max height of terrain to be the lowest possible height


		/*LAB 4 - cubescript code snippet*/

		// Add a MeshFilter component to this entity. This essentially comprises of a
		// mesh definition, which in this example is a collection of vertices, colours 
		// and triangles (groups of three vertices). 
		MeshFilter terrainMesh = this.gameObject.AddComponent<MeshFilter>();
		terrainMesh.mesh = this.createTerrain();

		// Add a MeshRenderer component. This component actually renders the mesh that
		// is defined by the MeshFilter component.
		MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
		renderer.material.shader = shader;


	}


	void Update()
	{


		/*LAB 4 - cubescript code snippet*/
		// Get renderer component (in order to pass params to shader)
		MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

		// Pass updated light positions to shader
		renderer.material.SetColor("_PointLightColor", this.pointLight.color);
		renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());


	}


	//terrain is created and initialised in this method
	Mesh createTerrain(){


		/*Step 1: initialse terrain base, i.e a 2D square array of width and height (2^n) + 1 - i.e number of faces (divisons) + 1 */

		numVerts = (numDivisions+1)*(numDivisions+1); //initialise total number of divisions
		//initilaise the verticies, using just an 1d array to represent a 2d square (as we know the square parameters)
		verts = new Vector3[numVerts];
		//number of uv coordinates is equal to number of vertices
		Vector2[] uvs = new Vector2[numVerts];

		//number verticies for triangles = (number of divisions ^ 2) * 2 (two triangles in a square) * 3 (each triangle is defined by 3 verticies)
		int[] triangles = new int[numDivisions*numDivisions*2*3];

		//variables needed to generate terrain 
		float halfSize = size * 0.5f;
		float divisionSize = size / numDivisions;
		int triOffset = 0; // keep track of where we are in the loop when building the triangles

		//initialise a new mesh for the terrain 
		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		/*Loop to insert the verticies (triangles) for the terrain base, step through the verticies row by row.
		i represents the columns and j represents the rows*/
		for(int i = 0; i<= numDivisions ; i++){

			for(int j = 0; j <= numDivisions ;j++){

				//step through verticies row by row, represented in a 1d array. Set up the square base of the entire terrain (i.e when height = 0)
				verts[i*(numDivisions+1)+j] = new Vector3(-halfSize+j*divisionSize, 0.0f, halfSize-i*divisionSize);
				//spreads out one texture to the whole terrain
				uvs[i*(numDivisions+1)+j] = new Vector2((float)i/numDivisions,(float)j/numDivisions);

				//only need to make traingles when we are less than numDivisions, for the rows and columns
				if(i < numDivisions && j < numDivisions){

					//coordinates of each triangles relative to each square face we are working with
					int topLeft = i * (numDivisions+1) + j; 
					int botLeft = (i+1) * (numDivisions+1) + j;
					int topRight = topLeft + 1;
					int botRight = botLeft + 1;

					//define the vertices for triangles in clockwise fashion
					//definition for the top right triangle in the square
					triangles[triOffset] = topLeft;
					triangles[triOffset+1] = topRight;
					triangles[triOffset+2] = botRight;


					//definition for the bottom left triangle in the square
					triangles[triOffset+3] = topLeft;
					triangles[triOffset+4] = botRight;
					triangles[triOffset+5] = botLeft;

					triOffset+=6;
				}

			}
		}


		/*step 2: set initial values to the four corner points of the square in the array*/

		verts[0].y = Random.Range(-height,height); //top left corner
		verts[numDivisions].y = Random.Range(-height,height);//top right corner
		verts[verts.Length-1-numDivisions].y = Random.Range(-height,height);//bot left corner
		verts[verts.Length-1].y = Random.Range(-height,height);//bot right corner


		/*step 3: diamond and square steps*/

		/*the number of times (iterations) which you have to perform a diamond-square algorithms 
		(note: log^2(num of divisons) gives the number of times you have to perform, since there are 2^n faces)
		*/
		int iterations = (int)Mathf.Log(numDivisions,2);
		int numSquares = 1; //variable that holds number of squares for each iteration
		int squareSize = numDivisions;

		//loop i is for the current iteration you are on
		for(int i = 0; i < iterations;i++){

			int row = 0;

			//loop j and k are for the square steps
			for(int j =0; j < numSquares; j++){

				int col = 0;

				for(int k = 0; k < numSquares; k++){

					//do the diamond square algorithm to each square
					diamondSquare(row, col, squareSize, height);

					// jump through each column in a row by a square size
					col += squareSize;

				}

				//go down a row by a squareSize
				row += squareSize;

			}

			//the number of squares doubles each iteration, while the size of the square halves (e.g 4x4 --(square step)--> 2x2)
			numSquares *= 2;
			squareSize /= 2;
			height *= 0.5f;

		}



		//assign all the variables to the mesh to be rendered
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		//find the maximum height of the generated terrain
		for (int i = 0; i < verts.Length; i++) {
			if (verts [i].y > currMaxHeight) {
				currMaxHeight = verts [i].y;
			}
		}

		//Debug.Log (currMaxHeight);
		
		Color[] terrainColor = new Color[verts.Length];

		for (int i = 0; i < verts.Length; i++) {
			if (verts [i].y > currMaxHeight *0.75) {
				
				terrainColor [i] = Color.white; //snow

			} else if (verts [i].y > currMaxHeight *0.55 && verts [i].y < currMaxHeight *0.75) {
				
				terrainColor [i] = Color.grey; //rock

			}else if (verts [i].y > currMaxHeight*0.1 && verts [i].y < currMaxHeight*0.55) {
				
				terrainColor [i] = Color.green; // grass

			}else if (verts [i].y > 0  && verts [i].y < currMaxHeight*0.1) {

				terrainColor [i] = Color.yellow; // beach

			}else if (verts [i].y > -currMaxHeight*0.1  && verts [i].y < 0){
				
				terrainColor [i] = Color.cyan; //shallow water

			}else if (verts [i].y < currMaxHeight * 0){

				terrainColor [i] = Color.blue; //ocean

			}

		}

		mesh.colors = terrainColor;

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();


		//get the mesh collider from the game object, which is later applied to the terrains mesh 	
		meshCollider = GetComponent<MeshCollider>();
		// used to apply mesh collider to the terrain
		meshCollider.sharedMesh = mesh;

		return mesh;
	}


	//the actual diamond square algorithm implementation
	void diamondSquare(int row, int col, int squareSize, float heightOffset){
		
		int halfSize = (int)(squareSize*0.5f);//half of the square size

		int topLeft = row * (numDivisions + 1) + col; //top left of a square
		int botLeft = (row+squareSize) * (numDivisions+1) + col; //bottom left of a square (note: row+squareSize will give the bottom row)
		int topRight = topLeft+squareSize;//top right of a square
		int botRight = botLeft+squareSize;//bottom right of a square



		//find the mid point of the square by obtaining half of the row size and add it onto half of the column size
		int midPoint = (int)(row + halfSize) * (numDivisions + 1) +(int)(col+halfSize); 

		//step 1: diamond step
		//assign the midpoint vertex to the average of the corner verticies plus a random value (to give realism to the terrain)
		verts [midPoint].y = (verts [topLeft].y + verts [topRight].y + verts [botLeft].y + verts [botRight].y) * 0.25f + Random.Range(-heightOffset,heightOffset);

		//step 2: square step
		//assign each diamond corners by averaging the previous 2 ajacent square corners and the mid point, then plus a random value to it
		verts[topLeft+halfSize].y = (verts[topLeft].y + verts[topRight].y + verts[midPoint].y)/3 + Random.Range(-heightOffset,heightOffset) ; //top corner point
		verts[midPoint-halfSize].y = (verts[topLeft].y + verts[botLeft].y + verts[midPoint].y)/3 + Random.Range(-heightOffset,heightOffset) ; //left corner point
		verts[midPoint+halfSize].y = (verts[topRight].y + verts[botRight].y + verts[midPoint].y)/3 + Random.Range(-heightOffset,heightOffset) ;//right corner point
		verts[botLeft+halfSize].y = (verts[botLeft].y + verts[botRight].y + verts[midPoint].y)/3 + Random.Range(-heightOffset,heightOffset) ;//bottom corner point


	}





}

