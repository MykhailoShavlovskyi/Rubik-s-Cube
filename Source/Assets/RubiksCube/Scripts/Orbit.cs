using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

	[SerializeField] private CubeGen target;

	private float distance;
	Vector3 distanceVector = new Vector3 (0.6f, 0.5f, -0.6f);

	float oldX = 45;
	float oldY = -30;

	float currentX = 45;
	float currentY = -30;

	void Start () 
	{
		
	}

	void Update () 
	{
		Move ();

		Debug.Log (currentX);
		Debug.Log (currentY);
		Debug.Log (distanceVector);
	}

	public void Move()
	{
		distance = target.layerCount * 2 + 5;

		Vector3 center = target.transform.position + new Vector3 (1, 1, 1) * ((float)target.layerCount / 2 - 0.5f);
		transform.position = center + distanceVector.normalized * distance;

		if (Input.GetMouseButtonDown (1)) 
		{
			oldX = Input.mousePosition.x;
			oldY = Input.mousePosition.y;
		}

		if (Input.GetMouseButton (1)) 
		{
			float deltaX = Input.mousePosition.x - oldX;
			float deltaY = Input.mousePosition.y - oldY;

			currentX += deltaX/2;
			currentY += deltaY/2;

			if (currentY < -80) 
			{
				currentY = -80;
			}
			else if (currentY > 80)
			{
				currentY = 80;
			}

			distanceVector = Quaternion.Euler (0, 0, -currentY) * new Vector3 (1, 0, 0);
			distanceVector = Quaternion.Euler (0, currentX, 0) * distanceVector;

			oldX = Input.mousePosition.x;
			oldY = Input.mousePosition.y;
		}

		transform.LookAt (center, Vector3.up);
	}
}
