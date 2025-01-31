using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("캐릭터 스텟")]
    [SerializeField] private float health = 100f; //체력
    [SerializeField] private float defense = 0f; //방어력
    [SerializeField] private float moveSpeed = 10f; //이동속도
    [SerializeField] private float jumpHeight = 5f; //점프높이
    [SerializeField] private float attackRange = 0f; //공격 범위 값
    [SerializeField] private float visionRange = 0f; //시야 범위 값
    [SerializeField] private int ammoCount = 0; //추가 장탄수 
    [SerializeField] private float criticalChance = 0.2f; // 20% 치명타 확률
    [SerializeField] private float criticalDamageMultiplier = 1.8f; //180% 치명타 데미지

    [Header("키 값")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A; //좌
    [SerializeField] private KeyCode moveRightKey = KeyCode.D; //우
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; //점프

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //컴포넌트 참조
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float moveDirection = 0f;

        if (Input.GetKey(moveLeftKey))
            moveDirection = -1f;
        else if (Input.GetKey(moveRightKey))
            moveDirection = 1f;

        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);

        if ((moveDirection > 0 && !facingRight) || (moveDirection < 0 && facingRight))
        {
            Flip();
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}