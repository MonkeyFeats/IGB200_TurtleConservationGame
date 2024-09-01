using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAccessoriesManager : MonoBehaviour
{
    public SpriteRenderer accessoryRenderer;
    public float accessoryHealth = 10f;
    public float accessoryHealthCurrent = 10f;
    public List<Sprite> accessoryStates;

    public bool isUnbreakable;

    float divisions = 0;

    private void Start()
    {
		if (isUnbreakable)
		{
            return;
		}

        divisions = accessoryStates == null ? 0 : accessoryStates.Count > 0 ? accessoryHealth / accessoryStates.Count : 0;
    }

    private void Update()
    {
        if (accessoryHealthCurrent <= 0 && !isUnbreakable)
        {
            //Remove accessory
            Destroy(accessoryRenderer.gameObject);
        }
    }

    public void TakeDamage(float amnt)
	{
		if (isUnbreakable) { return; }

		accessoryHealthCurrent -= amnt;

        int index = Mathf.CeilToInt((accessoryHealth - accessoryHealthCurrent) / divisions);

        index = index > (accessoryStates.Count - 1) ? (accessoryStates.Count - 1) : index;

        accessoryRenderer.sprite = accessoryStates[index];
    }
}
