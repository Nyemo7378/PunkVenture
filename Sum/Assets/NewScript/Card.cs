using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    private Vector3 originalScale;  // 원래 크기
    private Vector3 targetScale;    // 목표 크기 (숨쉬는 효과 크기)
    private bool isHovered = false; // 카드가 커서 위에 있는지 여부
    public float animationDuration = 1.0f;  // 숨쉬는 효과의 속도

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.1f;  // 목표 크기 (1.1배)
    }

    void Update()
    {
        if (isHovered)
        {
            // 숨쉬는 효과: 크기가 원래 크기와 목표 크기 사이에서 주기적으로 변함
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(Time.time * animationDuration, 1));
        }
        else
        {
            // 커서가 카드 위에 없으면 원래 크기로 유지
            transform.localScale = originalScale;
        }
    }

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    public int GetNumber()
    {
        return cardNumber;
    }

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
