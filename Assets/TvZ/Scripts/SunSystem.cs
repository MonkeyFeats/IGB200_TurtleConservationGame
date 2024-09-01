using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SunSystem : MonoBehaviour
{
    public int SunValue;

    private void OnMouseDown()
    {
        GameObject.FindObjectOfType<PvzGameManager>().AddSun(SunValue);

        Destroy(this.gameObject);
    }
}
