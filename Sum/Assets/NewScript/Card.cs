using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered = false;
    public float animationDuration = 1.0f;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.1f;
    }

    void Update()
    {
        if (isHovered)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(Time.time * animationDuration, 1));
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    void OnMouseEnter() => isHovered = true;
    void OnMouseExit() => isHovered = false;

    public int GetNumber() => cardNumber;

    public void SetNumber(int num)
    {
        cardNumber = num;
        Sprite[] sprites = Resources.LoadAll<Sprite>("cardsheet");
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("cardsheet not found or empty in Resources.");
            return;
        }

        string targetName = $"cardsheet_{num - 1}";
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == targetName)
            {
                GetComponent<SpriteRenderer>().sprite = sprite;
                return;
            }
        }

        Debug.LogWarning($"Sprite '{targetName}' not found in cardsheet.");
    }

    void OnMouseDown()
    {
        FindObjectOfType<CardManager>().OnCardClicked(this);
    }
}
