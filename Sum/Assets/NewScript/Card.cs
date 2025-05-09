using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    private Vector3 originalScale;  // ���� ũ��
    private Vector3 targetScale;    // ��ǥ ũ�� (������ ȿ�� ũ��)
    private bool isHovered = false; // ī�尡 Ŀ�� ���� �ִ��� ����
    public float animationDuration = 1.0f;  // ������ ȿ���� �ӵ�

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.1f;  // ��ǥ ũ�� (1.1��)
    }

    void Update()
    {
        if (isHovered)
        {
            // ������ ȿ��: ũ�Ⱑ ���� ũ��� ��ǥ ũ�� ���̿��� �ֱ������� ����
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(Time.time * animationDuration, 1));
        }
        else
        {
            // Ŀ���� ī�� ���� ������ ���� ũ��� ����
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
