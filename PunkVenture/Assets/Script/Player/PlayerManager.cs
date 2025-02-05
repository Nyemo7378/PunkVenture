using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("ĳ���� ����")]
    [SerializeField] private float health = 100f; // ü��
    [SerializeField] private float defense = 0f; // ����
    [SerializeField] private float moveSpeed = 10f; // �̵��ӵ�
    [SerializeField] private float jumpHeight = 5f; // ��������
    [SerializeField] private float attackRange = 1.5f; // ���� ���� ��
    [SerializeField] private float attackDamage = 20f; // ���ݷ�
    [SerializeField] private float attackSpeed = 1.0f; // �ִϸ��̼� ����ӵ�
    [SerializeField] private bool attacking = false; // ���� �ִϸ��̼� ������ Ȯ��

    [Header("���� �ݶ��̴�")]
    [SerializeField] private GameObject attackColliderPrefab; // ���� �浹 ������ ������
    private GameObject currentAttackCollider;

    [Header("Ű ��")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A; // ��
    [SerializeField] private KeyCode moveRightKey = KeyCode.D; // ��
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // ����
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0; // ���콺 ���� Ŭ��

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // ������Ʈ ����
        animator = GetComponent<Animator>(); // �ִϸ����� ����
        animator.SetFloat("AttackSpeed", attackSpeed); // �ִϸ��̼� �ӵ� ����
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    private void HandleMovement()
    {
        float moveDirection = 0f;

        if (Input.GetKey(moveLeftKey))
            moveDirection = -1f;
        else if (Input.GetKey(moveRightKey))
            moveDirection = 1f;

        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);

        // �̵� �ִϸ��̼� ����
        animator.SetBool("Run", moveDirection != 0);

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

            // ���� �ִϸ��̼� ����
            animator.SetTrigger("Jump");
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKey(attackKey) && !attacking)
        {
            attacking = true;
            animator.speed = attackSpeed; // ���� �� �ִϸ��̼� �ӵ� ����
            animator.SetTrigger("Attack1");
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

    // �ִϸ��̼� �̺�Ʈ - ���� ���� �� �ݶ��̴� ����
    public void CreateAttackCollider()
    {
        if (attackColliderPrefab != null && currentAttackCollider == null)
        {
            Vector3 spawnPosition = transform.position + (facingRight ? Vector3.right : Vector3.left) * attackRange;
            currentAttackCollider = Instantiate(attackColliderPrefab, spawnPosition, Quaternion.identity);
            currentAttackCollider.transform.parent = transform;
        }
    }

    // �ִϸ��̼� �̺�Ʈ - ���� ���� �� �ݶ��̴� ���� �� ���� �ʱ�ȭ
    public void DestroyAttackCollider()
    {
        if (currentAttackCollider != null)
        {
            Destroy(currentAttackCollider);
        }
    }

    // �ִϸ��̼� �̺�Ʈ - ���� ���� �� ���� ���� ���·� ����
    public void ResetAttack()
    {
        attacking = false;
        animator.speed = 1.0f; // �⺻ �ӵ��� ����
    }
}
