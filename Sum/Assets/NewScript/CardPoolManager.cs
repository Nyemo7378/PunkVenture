using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPoolManager : MonoBehaviour
{
    public static CardPoolManager Instance { get; private set; }

    public GameObject cardPrefab;
    public int poolSize = 20;

    private Queue<GameObject> availableCards = new Queue<GameObject>();

    public Vector3 returnPoint = new Vector3(100, 100, 0);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject card = Instantiate(cardPrefab);
            card.SetActive(false);
            card.transform.position = returnPoint;
            availableCards.Enqueue(card);
        }
    }

    public GameObject GetCard()
    {
        GameObject card;
        if (availableCards.Count > 0)
        {
            card = availableCards.Dequeue();
        }
        else
        {
            Debug.LogWarning("Card pool exhausted, creating new card.");
            card = Instantiate(cardPrefab);
        }

        card.SetActive(true);

        return card;
    }

    public void ReturnCard(GameObject card)
    {
        if (card == null) return;

        card.SetActive(false);
        card.transform.SetParent(null);
        card.transform.position = returnPoint;

        availableCards.Enqueue(card);
    }
}