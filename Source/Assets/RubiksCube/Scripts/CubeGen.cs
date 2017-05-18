using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CubeGen : MonoBehaviour 
{
	public class Anchor
	{
		public GameObject cubicle = null;
	}
	[SerializeField] private GameObject cubePiecePrefab = null;
	[SerializeField] private GameObject cubeFacePrefab = null;
	[SerializeField] private GameObject rotationAxisHitboxPrefab = null;
	[SerializeField] private GameObject rotationButton = null;
	[SerializeField] private List<Material> colors = null;
	[SerializeField] private Orbit orbit;

	private List<Anchor[,]> rightCubicleMatrixes = new List<Anchor[,]> ();
	private List<Anchor[,]> upCubicleMatrixes = new List<Anchor[,]> ();
	private List<Anchor[,]> forwardCubicleMatrixes = new List<Anchor[,]>();

	private List<RotationAxisHitbox> rotationButtons = new List<RotationAxisHitbox>();
	private List<GameObject> rotationAxisesHitboxes = new List<GameObject>();

	public int layerCount;

	private bool randomizing = false;
	private List<RotationAxisHitbox> randomRotationButtons = new List<RotationAxisHitbox>();

	void Start()
	{
		
	}

	void Update()
	{
		if (randomizing) {
			Randomize ();
		}
	}


	public void Generate(int newLayerCount)
	{
		layerCount = newLayerCount;
		randomizing = false;
		RotationAxisHitbox.blockRotation = false;

		//clear lists
		rightCubicleMatrixes.Clear();
		upCubicleMatrixes.Clear();
		forwardCubicleMatrixes.Clear();
		rotationButtons.Clear ();
		randomRotationButtons.Clear ();
		rotationAxisesHitboxes.Clear ();

		for (int i = 0; i < layerCount; i++) 
		{
			rightCubicleMatrixes.Add (new Anchor[layerCount, layerCount]);
			upCubicleMatrixes.Add (new Anchor[layerCount, layerCount]);
			forwardCubicleMatrixes.Add (new Anchor[layerCount, layerCount]);
		}

		//delete old cube
		foreach (Transform trans in GetComponentInChildren<Transform>()) 
		{
			if (trans != transform) {
				Destroy (trans.gameObject);
			}
		}
			
		//spawn cube pieces
		for (int x = 0; x < layerCount; x++) {
			for (int y = 0; y < layerCount; y++) {
				for (int z = 0; z < layerCount; z++) {
					// dont spawn cubicles inside the cube
					if (x == 0 || y == 0 || z == 0 || x == layerCount - 1 || y == layerCount - 1 || z == layerCount - 1) 
					{ 
						//spawn cube pieces
						GameObject cubePiece = Instantiate (cubePiecePrefab);
						cubePiece.transform.position = new Vector3(x,y,z);
						cubePiece.transform.parent = transform;

						//spawn anchor and write cube piece to it
						Anchor anchor = new Anchor();
						anchor.cubicle = cubePiece;

						//save anchor to matrixes
						rightCubicleMatrixes [x] [y, z] = anchor;
						upCubicleMatrixes [y] [x, z] = anchor;
						forwardCubicleMatrixes [z] [x, y] = anchor;

						//add color faces to the piece
						AddFaces (x,y,z, cubePiece, layerCount);
					}
				}
			}
		}
			
		//spawn rotation buttons
		SpawnRotationAxisHitboxes();

		//update camera
		orbit.Move();
	}
		
	private void AddFaces(int x, int y, int z, GameObject cubicle, int layerCount)
	{
		if (x == 0) { //green face
			GameObject cubicleFace = Instantiate (cubeFacePrefab);
			cubicleFace.transform.position = cubicle.transform.position + new Vector3 (-0.51f, 0, 0);
			cubicleFace.transform.Rotate (new Vector3 (0,0,90));
			cubicleFace.GetComponent<Renderer> ().material = colors [0];
			cubicleFace.transform.parent = cubicle.transform;
		}
		if (y == 0) { //yellow face
			GameObject cubicleFace = Instantiate (cubeFacePrefab);
			cubicleFace.transform.position = cubicle.transform.position + new Vector3 (0, -0.51f, 0);
			cubicleFace.GetComponent<Renderer> ().material = colors [1];
			cubicleFace.transform.parent = cubicle.transform;
		}
		if (z == 0) { //red face
			GameObject cubicleFace = Instantiate (cubeFacePrefab);
			cubicleFace.transform.position = cubicle.transform.position + new Vector3 (0, 0, -0.51f);
			cubicleFace.transform.Rotate (new Vector3 (90,0,0));
			cubicleFace.GetComponent<Renderer> ().material = colors [2];
			cubicleFace.transform.parent = cubicle.transform;
		}
		if (x == layerCount - 1) { //blue face
			GameObject cubicleFace = Instantiate (cubeFacePrefab);
			cubicleFace.transform.position = cubicle.transform.position + new Vector3 (0.51f, 0, 0);
			cubicleFace.transform.Rotate (new Vector3 (0,0,90));
			cubicleFace.GetComponent<Renderer> ().material = colors [3];
			cubicleFace.transform.parent = cubicle.transform;
		}
		if (y == layerCount - 1) { //white face 
			GameObject cubicleFace = Instantiate (cubeFacePrefab);
			cubicleFace.transform.position = cubicle.transform.position + new Vector3 (0, 0.51f, 0);
			cubicleFace.GetComponent<Renderer> ().material = colors [4];
			cubicleFace.transform.parent = cubicle.transform;
		}
		if (z == layerCount - 1) { //orange face
			GameObject cubicleFace = Instantiate (cubeFacePrefab);
			cubicleFace.transform.position = cubicle.transform.position + new Vector3 (0, 0, 0.51f);
			cubicleFace.transform.Rotate (new Vector3 (90,0,0));
			cubicleFace.GetComponent<Renderer> ().material = colors [5];
			cubicleFace.transform.parent = cubicle.transform;
		}
	}

	private void SpawnRotationAxisHitboxes()
	{
		for (int i = 0; i < layerCount; i++) 
		{
			for (int j = 0; j < 3; j++) 
			{
				switch (j) 
				{
				case 0:
					for (int s = 0; s < 4; s++)
					{
						GameObject rotationAxisHitbox = Instantiate (rotationAxisHitboxPrefab);
						rotationAxisHitbox.transform.localScale = new Vector3 (1, 0.01f, layerCount);
						rotationAxisHitbox.transform.parent = transform;

						RotationAxisHitbox rotationScript = rotationAxisHitbox.GetComponent<RotationAxisHitbox> ();
						rotationScript.axisType = RotationAxisHitbox.Axis.Pink;

						switch (s) {
						case 0:
							rotationButtons.Add (rotationScript);

							rotationAxisHitbox.transform.position = new Vector3 (i, layerCount - 0.4f, (float)layerCount / 2 - 0.5f);
							rotationScript.direction = new Vector3 (0,0,1);
							break;
						case 1:
							rotationAxisHitbox.transform.position = new Vector3 (i, -0.6f, (float)layerCount / 2 - 0.5f);
							rotationScript.direction = new Vector3 (0,0,-1);
							break;
						case 2:
							rotationAxisHitbox.transform.position = new Vector3 (i, (float)layerCount / 2 - 0.5f, -0.6f);
							rotationAxisHitbox.transform.Rotate (new Vector3(1,0,0), 90);
							rotationScript.direction = new Vector3 (0,1,0);
							break;
						case 3:
							rotationAxisHitbox.transform.position = new Vector3 (i, (float)layerCount / 2 - 0.5f, layerCount - 0.4f);
							rotationAxisHitbox.transform.Rotate (new Vector3(1,0,0), 90);
							rotationScript.direction = new Vector3 (0,-1,0);
							break;
						}

						rotationScript.matrix = rightCubicleMatrixes [i];
						rotationScript.axis = -Vector3.right;
						rotationScript.layerCount = layerCount;

						rotationAxisesHitboxes.Add (rotationAxisHitbox);
					}



					break;
				case 1:
					for (int s = 0; s < 4; s++)
					{
						GameObject rotationAxisHitbox = Instantiate (rotationAxisHitboxPrefab);
						rotationAxisHitbox.transform.localScale = new Vector3 ( 0.01f, 1, layerCount);
						rotationAxisHitbox.transform.parent = transform;

						RotationAxisHitbox rotationScript = rotationAxisHitbox.GetComponent<RotationAxisHitbox> ();
						rotationScript.axisType = RotationAxisHitbox.Axis.Green;

						switch (s) {
						case 0:
							rotationButtons.Add (rotationScript);

							rotationAxisHitbox.transform.position = new Vector3 (layerCount- 0.4f, i, (float)layerCount / 2 - 0.5f);
							rotationScript.direction = new Vector3 (0,0,1);
							break;
						case 1:
							rotationAxisHitbox.transform.position = new Vector3 (-0.6f, i, (float)layerCount / 2 - 0.5f);
							rotationScript.direction = new Vector3 (0,0,-1);
							break;
						case 2:
							rotationAxisHitbox.transform.position = new Vector3 ((float)layerCount / 2 - 0.5f, i, layerCount- 0.4f);
							rotationAxisHitbox.transform.Rotate (new Vector3(0,1,0), 90);
							rotationScript.direction = new Vector3 (-1,0,0);
							break;
						case 3:
							rotationAxisHitbox.transform.position = new Vector3 ((float)layerCount / 2 - 0.5f, i, - 0.6f);
							rotationAxisHitbox.transform.Rotate (new Vector3(0,1,0), 90);
							rotationScript.direction = new Vector3 (1,0,0);
							break;
						}

						rotationScript.matrix = upCubicleMatrixes [i];
						rotationScript.axis = Vector3.up;
						rotationScript.layerCount = layerCount;

						rotationAxisesHitboxes.Add (rotationAxisHitbox);
					}

					break;
				case 2:
					for (int s = 0; s < 4; s++)
					{
						GameObject rotationAxisHitbox = Instantiate (rotationAxisHitboxPrefab);
						rotationAxisHitbox.transform.localScale = new Vector3 ( 0.01f, layerCount, 1);
						rotationAxisHitbox.transform.parent = transform;

						RotationAxisHitbox rotationScript = rotationAxisHitbox.GetComponent<RotationAxisHitbox> ();
						rotationScript.axisType = RotationAxisHitbox.Axis.Blue;

						switch (s) {
						case 0:
							rotationButtons.Add (rotationScript);

							rotationAxisHitbox.transform.position = new Vector3 (layerCount- 0.4f, (float)layerCount / 2 - 0.5f, i);
							rotationScript.direction = new Vector3 (0,1,0);
							break;
						case 1:
							rotationAxisHitbox.transform.position = new Vector3 (-0.6f, (float)layerCount / 2 - 0.5f, i);
							rotationScript.direction = new Vector3 (0,-1,0);
							break;
						case 2:
							rotationAxisHitbox.transform.position = new Vector3 ((float)layerCount / 2 - 0.5f, layerCount- 0.4f, i);
							rotationAxisHitbox.transform.Rotate (new Vector3(0,0,1), 90);
							rotationScript.direction = new Vector3 (-1,0,0);
							break;
						case 3:
							rotationAxisHitbox.transform.position = new Vector3 ((float)layerCount / 2 - 0.5f, - 0.6f, i);
							rotationAxisHitbox.transform.Rotate (new Vector3(0,0,1), 90);
							rotationScript.direction = new Vector3 (1,0,0);
							break;
						}

						rotationScript.matrix = forwardCubicleMatrixes [i];
						rotationScript.axis = -Vector3.forward;
						rotationScript.layerCount = layerCount;

						rotationAxisesHitboxes.Add (rotationAxisHitbox);
					}

					break;
				}
			}
		}
	}

	public void StartRandomizing()
	{
		if (rotationButtons.Count > 0)
		{
			randomizing = true;

			for (int i = 0; i < (layerCount)*8; i++) 
			{
				randomRotationButtons.Add(rotationButtons [Random.Range (0, rotationButtons.Count - 1)]);
			}
		}
	}

	private void Randomize()
	{
		if (!RotationAxisHitbox.blockRotation)
		{
			if (randomRotationButtons.Count == 0) 
			{
				randomizing = false;
				return;
			}
			bool clockwise = true;
			if (Random.Range (0, 1) == 1)
			{
				clockwise = false;
			}

			randomRotationButtons [0].Rotate (0.06f, clockwise);
			randomRotationButtons.RemoveAt (0);
		}
	}
}
