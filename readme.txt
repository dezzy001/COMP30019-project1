IMPLEMENTATION DESCRIPTION:
-Modelling of fractal landscape (DiamondSquareTerrain.cs):
	- Implemented the diamond square terrain dynamically using a script
	
	-Input Parameters for the algorithm are: maximum number of square divisions, size (side length of terrain), and the maxHeight for the terrain.

	step 1: initialse mesh with a square base for the terrain, 
	 where width = height = (2^n) + 1 , 2^n is the number of divisions of the terrain. (variable names are verts and triangles)
	-loop is used to initialise verts and traingles to model the square base of the terrain, at height = 0, based on the terrain parameters.
	
	step 2:  Initialise random height values to the four corner points of the square in the 1d arrays.
	
	step 3: Perform diamond and square steps to form randomly generated terrain.
	-determine number of iterations required to perform the algorithm, where iterations = log^2(number of divisions)
	-knowing this, we can loop through number of iterations, performing a diamond step and square step for each iteratoin.
	-Diamond square algorithm code implementation:
		1) start off finding the mid point of the square we are working with this iteration.
		2) perform diamond step - assign the height of the squares midpoint to the average of its corners (i.e diamond mid points) + plus a random value
		3) perform square step - assign each diamonds midpoint's height to the average of the 2 ajacent square corners and squares midpoint + plus a random value 
		NOTE: adding a random value to the height for the diamond and square step is necessary to give realism to terrain
		4) after the end of each iteration, double the number of squares hence halving the size of the squares. 
	
	step 4: Add colour to each vertex based on height, and assign the colour,vertex and triangle array and the mesh collider to the mesh object

-Camera motion (flightSim.cs):
	-camera translation   - implemented a method "getTranslateKeyInput()" which gets input from "ASWD" keypress, and returns a unit vector with direction based on key press
			      - unit vector returned is then multiplied by a public speed variable, before camera is translated
	-camera yaw and pitch - refered to a "fly cam" script on unity blog post, which uses the previous mouse position to rotate the screen, using transform.eulerAngles 
				in the x and y direction. The previous mouse position is multiplied by camera sensitivy variable to adjust the rotation speed.
	-camera roll 	      - used method "getRotateKeyInput()" to get "QE" keypress which returns the current magnitude of the roll *multiplied by a rollspeed,
		                then it is passed into transform.Rotate().
	-camera collision     - A rigid body is added to the camera, but in update(), the velocity and angular velocity of the rigid body is constatly equal to zero to 
				prevent any external forces to affect the camera. A mesh collision is applied to the terrain in DiamondSquareTerrain.cs


-Surface Properties
	- Used PhongShader.shader from lab 4, with adjustments to the k (constants) for the diffuse and specular component.
	- Used PointLight.cs codes from lab 4, to keep track of the point light position
	- Used XAxisSpin.cs script from lab 2, to rotate the sun around the terrain, by making the point light a child of a game object which acts as the centre of rotation
	- Colour is based on the max height of the currently generated terrain, which is implemented in (DiamondSquareTerrain.cs)
	- A sphere mesh filter is added to the sun in unity3d component, with a sprites/default shader



CODE REFERENCES FROM OTHER SOURCES:
LAB 2:
-used the XAxisSpin.cs script, to help make the sun rotate around the terrain

LAB 4: 
-used the PhongShader.shader and the PointLight.cs codes from lab 4
-Phong illumination constants changed for the diffuse and specular components
-Gave the Point light an initial position based on terrain size

fly cam:
https://forum.unity3d.com/threads/fly-cam-simple-cam-script.67042/
-used the idea from this blog post for mouse transformations, i.e controlling yaw and pitch, from line 78 - 80 in my flightSim.cs, by using the prevMousePos variable

Diamond-Square Procedural Terrain:
https://www.youtube.com/watch?v=1HV8GbFnCik
-tightly followed the code in this tutorial for understanding the implementation of the diamond square algorithm in unity (took inspiration from the tutorial for use of an 1d array)

