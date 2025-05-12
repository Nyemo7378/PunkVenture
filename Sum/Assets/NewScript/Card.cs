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

    private bool interactable = true; // 👉 카드 클릭 가능 여부

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.1f;
    }

    void Update()
    {
        if (!interactable) return;

        if (isHovered)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(Time.time * animationDuration, 1));
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    void OnMouseEnter()
    {
        if (!interactable) return;
        isHovered = true;
    }

    void OnMouseExit()
    {
        if (!interactable) return;
        isHovered = false;
    }

    void OnMouseDown()
    {
        if (!interactable) return;

        SEManager.Instance.Play("click");

        FindObjectOfType<CardManager>().OnCardClicked(this);
    }

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

    // ✅ 외부에서 클릭 가능 여부 제어
    public void SetInteractable(bool value)
    {
        interactable = value;
    }

    public void Explode(Vector2 forceDir, float forcePower, float torque = 0f)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0; // 중력은 제거
        rb.AddForce(forceDir * forcePower, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }

}
