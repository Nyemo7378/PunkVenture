using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("ĳ���� ����")]
    [SerializeField] private float health = 100f; //ü��
    [SerializeField] private float defense = 0f; //����
    [SerializeField] private float moveSpeed = 10f; //�̵��ӵ�
    [SerializeField] private float jumpHeight = 5f; //��������
    [SerializeField] private float attackRange = 0f; //���� ���� ��
    [SerializeField] private float visionRange = 0f; //�þ� ���� ��
    [SerializeField] private int ammoCount = 0; //�߰� ��ź�� 
    [SerializeField] private float criticalChance = 0.2f; // 20% ġ��Ÿ Ȯ��
    [SerializeField] private float criticalDamageMultiplier = 1.8f; //180% ġ��Ÿ ������

    [Header("Ű ��")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A; //��
    [SerializeField] private KeyCode moveRightKey = KeyCode.D; //��
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; //����

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //������Ʈ ����
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