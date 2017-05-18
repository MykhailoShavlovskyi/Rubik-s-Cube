using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerCountText : MonoBehaviour 
{
	public void UpdateText(Text text)
	{
		text.text = "" + GetComponent<Slider>().value;
	}
}
