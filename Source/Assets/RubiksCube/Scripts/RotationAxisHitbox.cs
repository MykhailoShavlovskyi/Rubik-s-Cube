using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotationAxisHitbox : MonoBehaviour 
{
	public enum Axis
	{
		Pink, Green, Blue
	};
	[SerializeField] private List<Material> colors;
	public Axis axisType;

	public static bool blockRotation = false;

	public CubeGen.Anchor[,] matrix;
	public int layerCount;
	public Vector3 axis;


	private float rotatingTime = 0.2f;
	private bool rotating = false;
	private float angle =0;

	private bool clockwise = false;

	public Vector3 direction;
	public Vector3 mouseDownPointInWorld;
	public Vector2 mouseDownPointOnScreen;
	public Vector2 mouseStartFollwingPointOnScreen;
	public Vector2 mouseMoveVector;

	private bool tryingToRotate = false;

	private bool followMouse = false;
	float currentAngle;
	bool followingClockwise;

	void Start () 
	{
		switch (axisType) 
		{
		case Axis.Pink:
			GetComponent<Renderer> ().material = colors [0];
			break;
		case Axis.Green:
			GetComponent<Renderer> ().material = colors [1];
			break;
		case Axis.Blue:
			GetComponent<Renderer> ().material = colors [2];
			break;
		}
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			MouseDown ();
		}
			
		if (Input.GetMouseButton(0))
		{			
			MouseHold ();
		}

		if (Input.GetMouseButtonUp(0))
		{
			MouseUp ();
		}
			
		//animate rotation
		if (rotating)
		{
			float step = (90f / 100) * (Time.deltaTime / (rotatingTime / 100));
			angle += step;
			if (angle >= 90)
			{
				rotating = false;
				blockRotation = false;

				step = 90 - (angle - step);
			}

			RotatePieces(step, clockwise);
		}

		//follow mouse
		if (followMouse) 
		{
			FollowMouse ();
		}
	}

	//-----------------------------INPUT-----------------------------
	private void MouseDown()
	{
		if (!tryingToRotate && !blockRotation) 
		{
			//get 2 closest hits;
			RaycastHit[] closestHits = new RaycastHit[2];
			float closestDistance1 = Mathf.Infinity;
			float closestDistance2 = Mathf.Infinity;
			foreach(RaycastHit hit in Physics.RaycastAll (Camera.main.ScreenPointToRay(Input.mousePosition), 100.0f))
			{
				float distance = (hit.point - Camera.main.transform.position).magnitude;

				if (distance < closestDistance1) 
				{
					closestHits [1] = closestHits [0];
					closestDistance2 = closestDistance1;

					closestDistance1 = distance;
					closestHits [0] = hit;

				}
				else if(distance < closestDistance2)
				{
					closestDistance2 = distance;
					closestHits [1] = hit;
				}
			}

			//check if we are hitting this hitbox
			foreach (RaycastHit hit in closestHits) 
			{
				if (hit.transform != null) 
				{
					if (hit.transform == transform) // if we hit our hit box
					{ 
						mouseDownPointInWorld = hit.point;
						mouseDownPointOnScreen = Input.mousePosition;
						tryingToRotate = true;
						break;
					}
					else if (hit.transform.GetComponent<RotationAxisHitbox> ().axisType == axisType) 
					{
						break;
					}
				}
			}
		}
	}

	private void MouseHold()
	{
		if(tryingToRotate && !rotating && !blockRotation)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] allHits = Physics.RaycastAll (ray , 100.0f);
			foreach (RaycastHit hit in allHits) 
			{
				if(hit.transform == transform)
				{

					Vector3 mouseMovement = hit.point - mouseDownPointInWorld;

					float dot = Vector3.Dot (direction, mouseMovement);
					if (dot > (float)layerCount/10) 
					{
						//rotate a bit and follow mouse
						clockwise = false;
						tryingToRotate = false;

						mouseMoveVector = mouseDownPointOnScreen - (Vector2)Input.mousePosition;
						mouseStartFollwingPointOnScreen = Input.mousePosition;


						StartFollwingMouse (0, clockwise);
					}
					else if (dot < -(float)layerCount/10) 
					{
						//rotate a bit and follow mouse
						clockwise = true;
						tryingToRotate = false;

						mouseMoveVector = (Vector2)Input.mousePosition - mouseDownPointOnScreen;
						mouseStartFollwingPointOnScreen = Input.mousePosition;


						StartFollwingMouse (0, clockwise);
					}
					break;
				}
			}
		}
	}
		
	private void MouseUp()
	{
		tryingToRotate = false;

		if (followMouse) 
		{
			StopFollowingMouse ();
		}
	}

	//---------------------------------------------------------------

	private void RotatePieces(float step, bool isClockwise)
	{
		//physically rotate cubicles
		for (int i = 0; i < layerCount; i++) 
		{
			for (int j = 0; j < layerCount; j++) 
			{
				if (matrix [i, j] != null) 
				{
					Vector3 point = axis;
					point = point - new Vector3(1,1,1);
					point = point *= -((float)layerCount / 2 - 0.5f);
					if (isClockwise) 
					{
						matrix [i, j].cubicle.transform.RotateAround (point, axis, step);
					}
					else 
					{
						matrix [i, j].cubicle.transform.RotateAround (point, axis, -step);
					}
				}
			}
		}
	}

	private void RotateMatrix(bool isClockwise)
	{
		//clone matrix
		GameObject[,] oldMatrix = new GameObject[layerCount,layerCount];
		for (int i = 0; i < layerCount; i++) 
		{
			for (int j = 0; j < layerCount; j++) 
			{
				if (matrix [i, j] != null) {
					oldMatrix [i,j] = matrix [i,j].cubicle;
				}
			}
		}

		//rotate matrix
		for (int i = 0; i < layerCount; i++) 
		{
			for (int j = 0; j < layerCount; j++) 
			{
				if (matrix [i, j] != null)
				{
					if (isClockwise) 
					{
						matrix [i, j].cubicle = oldMatrix [layerCount - j - 1, i];
					}
					else
					{
						matrix [i, j].cubicle = oldMatrix [j, layerCount - i- 1];
					}
				}
			}
		}
	}

	//-----------------------------------ROTATION STARTING FUNSTIONS---------------------------------------------------
	public void RotateWithAngle(float newAngle, float newRotatingTime, bool isClockWise)
	{
		rotatingTime = 90/newAngle*newRotatingTime;
		clockwise = isClockWise;

		rotating = true;
		blockRotation = true;

		angle = 90-newAngle;
	}

	public void Rotate(float newRotatingTime, bool isClockWise)
	{
		rotatingTime = newRotatingTime;
		clockwise = isClockWise;

		rotating = true;
		blockRotation = true;

		angle =0;

		RotateMatrix (clockwise);

	}

	public void RotateInstantly(bool isClockWise)
	{
		clockwise = isClockWise;
		RotatePieces(90, clockwise);
		RotateMatrix(clockwise);
	}
	//-----------------------------------------------------------------------------------------------------------------

	private void StartFollwingMouse(float newCurrentAngle, bool isFollowingClockwise)
	{
		currentAngle = newCurrentAngle;
		followingClockwise = isFollowingClockwise;
		followMouse = true;
	}

	private void FollowMouse()
	{
		if (!rotating) 
		{
			blockRotation = true;

			Vector2 currentMouseMoveVector = (Vector2)Input.mousePosition - mouseStartFollwingPointOnScreen;

			float newAngle = Vector3.Dot (mouseMoveVector.normalized, currentMouseMoveVector);
			float deltaAngle = newAngle - currentAngle;
			currentAngle = newAngle;

			RotatePieces(deltaAngle, true);
		}
	}

	private void StopFollowingMouse()
	{
		int turns = (int)currentAngle / 90;
		float angleToFinishTurn  = currentAngle - 90*turns;

		if (turns < 0) { turns *= -1;}
		for(int i = 0; i < turns; i++)
		{
			if (currentAngle < 0) 
			{
				RotateMatrix (false);
			} 
			else 
			{
				RotateMatrix (true);
			}
		}


		if (angleToFinishTurn > 45)
		{
			RotateWithAngle (90 - angleToFinishTurn, 0.2f, true);
			RotateMatrix (true);
		} 
		else if (angleToFinishTurn < -45)
		{
			RotateWithAngle (90 + angleToFinishTurn, 0.2f, false);
			RotateMatrix(false);
		} 

		else if(angleToFinishTurn > 0)
		{
			RotateWithAngle (angleToFinishTurn, 0.2f, false);
		}
		else if(angleToFinishTurn < 0)
		{
			RotateWithAngle (-angleToFinishTurn, 0.2f, true);
		}

		followMouse = false;
	}
}
