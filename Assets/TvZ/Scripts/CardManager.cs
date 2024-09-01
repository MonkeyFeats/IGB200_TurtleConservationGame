using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Match3;

public class CardManager : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public GameObject UI;
    public SlotsManagerCollider colliderName;
    SlotsManagerCollider prevName;
    public PlantCardScriptableObject plantCardScriptableObject;
    public Sprite plantSprite;
    public GameObject plantPrefab;
    public bool isOverCollider = false;
    GameObject plant;
    bool isHoldingPlant;

    public Image refreshImage;

    [Tooltip("X: Max Height, Y: Min Height")]
    public Vector2 height;

    public bool isCoolingDown;

    public bool isSelection;

    public bool isSelected;

    public static bool isGameStart = false;

    public PlantCardManager plantCardManager;

    public CardManager parentCard;

    public void OnDrag(PointerEventData eventData)
    {
		if (isSelection)
		{
            return;
		}

        if (isCoolingDown)
        {
            return;
        }

        if (isHoldingPlant)
        {
            //Take a gameObject
            plant.GetComponent<SpriteRenderer>().sprite = plantSprite;

            if (prevName != colliderName || prevName == null)
            {
                if (!colliderName.isOccupied)
                {
                    plant.transform.position = new Vector3(0, 0, -1);
                    plant.transform.localPosition = new Vector3(0, 0, -1);
                    isOverCollider = false;
                    if (prevName != null)
                    {
                        prevName.plant = null;
                    }
                    prevName = colliderName;
                }
            }
            else
            {
                if (!colliderName.isOccupied)
                {
                    plant.transform.position = new Vector3(0, 0, -1);
                    plant.transform.localPosition = new Vector3(0, 0, -1);
                }
            }

            if (!isOverCollider)
            {
                plant.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
		if (isSelection)
		{
			//Click on this, add reference to bar
            isSelected = true;
            plantCardManager.AddPlantReference(plantCardScriptableObject, this.gameObject.GetComponent<CardManager>());
        }
        else
		{
			if (!isGameStart)
			{
                //Deselect card
                parentCard.isSelected = isSelected = false;
                plantCardManager.AddPlantReference(plantCardScriptableObject);
            }
			else
			{
                if (isCoolingDown)
                {
                    return;
                }

                if (GameObject.FindObjectOfType<PvzGameManager>().SunAmount >= plantCardScriptableObject.cost)
                {
                    isHoldingPlant = true;
                    Vector3 pos = new Vector3(0, 0, -1);
                    plant = Instantiate(plantPrefab, pos, Quaternion.identity);
                    plant.GetComponent<PlantManager>().thisSO = plantCardScriptableObject;
                    plant.GetComponent<PlantManager>().isDragging = true;
                    plant.transform.localScale = plantCardScriptableObject.size;
                    plant.GetComponent<SpriteRenderer>().sprite = plantSprite;

                    plant.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {
                    Debug.Log("Not enough sun!");
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
		if (isSelection)
		{
            return;
		}

        if (isCoolingDown)
        {
            return;
        }

        if (isHoldingPlant)
        {
            if (colliderName != null && !colliderName.isOccupied)
            {
                GameObject.FindObjectOfType<PvzGameManager>().DeductSun(plantCardScriptableObject.cost);
                isHoldingPlant = false;
                colliderName.isOccupied = true;
                plant.tag = "Untagged";
                plant.transform.SetParent(colliderName.transform);
                plant.transform.position = new Vector3(0, 0, -1);
                plant.transform.localPosition = new Vector3(0, 0, -1);
                plant.name = plantCardScriptableObject.name;

                BoxCollider2D boxColl = plant.AddComponent<BoxCollider2D>();
                boxColl.size = plantCardScriptableObject.colliderSize;
                
                CircleCollider2D circleColl = plant.AddComponent<CircleCollider2D>();
                circleColl.radius = plantCardScriptableObject.radius;

                plant.GetComponent<CircleCollider2D>().isTrigger = true;
                plant.transform.localScale = plantCardScriptableObject.size;

                plant.GetComponent<PlantManager>().isDragging = false;
                if (plantCardScriptableObject.isSunFlower)
                {
                    SunSpawner sunSpawner  = plant.AddComponent<SunSpawner>();
                    sunSpawner.isSunFlower = true;
                    sunSpawner.minTime = plantCardScriptableObject.sunSpawnerTemplate.minTime;
                    sunSpawner.maxTime = plantCardScriptableObject.sunSpawnerTemplate.maxTime;
                    sunSpawner.sun = plantCardScriptableObject.sunSpawnerTemplate.sun;
                }

                //Disable plant before cooldown has finished
                StartCoroutine(cardCooldown(plantCardScriptableObject.cooldown));
            }
            else
            {
                isHoldingPlant = false;
                Destroy(plant);
            }
        }
    }

    public IEnumerator cardCooldown(float cooldownDuration)
    {
        isCoolingDown = true;

        for (float i = height.x; i <= height.y; i++)
        {
            refreshImage.rectTransform.anchoredPosition = new Vector3(0, i, 0);

            yield return new WaitForSeconds(cooldownDuration / height.y);
        }

        isCoolingDown = false;
    }

    public void StartRefresh()
    {
        StartCoroutine(cardCooldown(plantCardScriptableObject.cooldown));
    }
}
