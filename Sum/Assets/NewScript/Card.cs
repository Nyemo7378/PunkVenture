// Card.cs (수정 버전: 호버 시에만 breathing, 이동 후 자동 breathing 버그 고침)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    public int lastStackKey = -1; // 마지막으로 있던 스택 키 저장

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered = false;
    public float animationDuration = 1.0f;
    private bool isMoving = false;

    private bool interactable = true;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.1f;
    }

    void Update()
    {
        if (!interactable) return;

        // 호버 + 이동 안 할 때만 breathing!
        if (isHovered && !isMoving)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(Time.time * animationDuration, 1));
        }
        else
        {
            // 호버 없거나 이동 중이면 고정 스케일
            transform.localScale = originalScale;
        }
    }

    void OnMouseEnter()
    {
        // 이동 중이거나 interactable 아니면 호버 무시
        if (!interactable || isMoving) return;
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    void OnMouseDown()
    {
        if (!interactable || isMoving) return;  // 이동 중 클릭 무시

        SEManager.Instance.Play("click");
        FindObjectOfType<CardManager>().OnCardClicked(this);
    }

    public void SetMoving(bool value)
    {
        isMoving = value;
        SetInteractable(!value);

        // 핵심: 이동 시작 시 호버 강제 초기화 (자동 breathing 방지)
        if (value) isHovered = false;
    }

    public bool IsMoving()
    {
        return isMoving;
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

    public void SetInteractable(bool value)
    {
        interactable = value;

        // 비활성화 시 호버 초기화
        if (!value) isHovered = false;
    }

    public void Explode(Vector2 forceDir, float forcePower, float torque = 0f)
    {
        // 폭발 시 호버 초기화
        isHovered = false;
        isMoving = true;  // 폭발도 이동으로 취급

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.AddForce(forceDir * forcePower, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);

        Invoke(nameof(DisableSelf), 5f);
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}