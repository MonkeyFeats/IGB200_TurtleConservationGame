using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int value;

    public void OnMouseDown()
	{
		//MatchGameManager.IncrementCoins(value);

		Destroy(this.gameObject);
	}
}
