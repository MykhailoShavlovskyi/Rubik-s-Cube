using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButton : MonoBehaviour 
{
	[SerializeField] Slider slider;
	[SerializeField] CubeGen cubeGen;

	public void Generate()
	{
		cubeGen.Generate ((int)slider.value);
	}
}
