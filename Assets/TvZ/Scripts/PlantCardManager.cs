using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantCardManager : MonoBehaviour
{
    [Header("Cards Parameters")]
    public int amtOfCards;
    public PlantCardScriptableObject[] plantCardSO;
    public GameObject cardPrefab;
    public Transform cardHolderTransform;

    [Header("Plant Parameters")]
    public List<GameObject> plantCards;
    public float cooldown;
    public int cost;
    public Texture plantIcon;

    public Transform selectionTransform;
    public GameObject selectionCardPrefab;

    public List<int> selectedIndexes;
    public List<GameObject> selectionCards;

    public int minCardAllowed;

	private void Start()
	{
        amtOfCards = plantCardSO.Length;
        plantCards = new List<GameObject>();

        selectionCards = new List<GameObject>();

        for (int i = 0; i < amtOfCards; i++)
        {
            //AddPlantCardSelection(i);
        }
    }

	public void Update()
	{

	}

	private void Start_Old()
    {
        for (int i = 0; i < amtOfCards; i++)
        {
            AddPlantCard(i);
        }
    }

    public void AddPlantReference(PlantCardScriptableObject plantSO, CardManager parentCard = default)
	{
        AddPlantCard(new List<PlantCardScriptableObject>(plantCardSO).IndexOf(plantSO), parentCard);
	}

    public void AddPlantCard(int index, CardManager parentCard = default)
    {
        if (selectedIndexes.Contains(index))
		{
            //Remove card
            int indexPos = selectedIndexes.IndexOf(index);

            GameObject tempRef = plantCards[indexPos];

            plantCards.Remove(tempRef);

            Destroy(tempRef);

            selectedIndexes.Remove(index);
        }
		else
		{
            selectedIndexes.Add(index);

            GameObject card = Instantiate(cardPrefab, cardHolderTransform);
            CardManager cardManager = card.GetComponent<CardManager>();

            cardManager.plantCardScriptableObject = plantCardSO[index];
            cardManager.plantSprite = plantCardSO[index].plantSprite;
            cardManager.UI = GameObject.FindGameObjectWithTag("Canvas");

            plantCards.Add(card);

            //Getting Variables
            plantIcon = plantCardSO[index].plantIcon;
            cost = plantCardSO[index].cost;
            cooldown = plantCardSO[index].cooldown;

            cardManager.parentCard = parentCard;
            cardManager.plantCardManager = this;

            Debug.Log("Name : " + parentCard.gameObject.name);

            //Updating UI
            card.GetComponentInChildren<RawImage>().texture = plantIcon;
            card.GetComponentInChildren<TMP_Text>().text = "" + cost;
        }
    }

    public void AddPlantCardSelection(int index)
    {
        GameObject card = Instantiate(selectionCardPrefab, selectionTransform);
        CardManager cardManager = card.GetComponent<CardManager>();

        cardManager.plantCardScriptableObject = plantCardSO[index];
        cardManager.plantSprite = plantCardSO[index].plantSprite;
        cardManager.UI = GameObject.FindGameObjectWithTag("Canvas");

        selectionCards.Add(card);

        //Getting Variables
        plantIcon = plantCardSO[index].plantIcon;
        cost = plantCardSO[index].cost;
        cooldown = plantCardSO[index].cooldown;

        cardManager.plantCardManager = this;

        //Updating UI
        card.GetComponentInChildren<RawImage>().texture = plantIcon;
        card.GetComponentInChildren<TMP_Text>().text = "" + cost;
    }
}
