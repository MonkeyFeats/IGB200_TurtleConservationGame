using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagsManager : MonoBehaviour
{
    public Sprite expandedSprite;


    public void Expand()
	{
		this.GetComponent<Image>().sprite = expandedSprite;

		this.GetComponent<Image>().SetNativeSize();
	}
}