using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPlayer
{
    [Header("ĳ���� ����")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private float EXP = 0;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Vector2 attackRangeOffset = new Vector2(1.0f, 0.0f); // ���� ���� ��ġ ���� ����
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float animSpeed = 1.0f; // �ִϸ��̼� ��� �ӵ�
    [SerializeField] private bool attacking = false; // ���� ������ Ȯ��

    [Header("���� ����")]
    [SerializeField] private LayerMask enemyLayers; // �� ���� ���̾�

    [Header("Ű ��")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("AttackSpeed", animSpeed); // �ִϸ��̼� �ӵ� ����
    }
    public void AddExp(float expAmount)
    {
        EXP += expAmount; // ����ġ �߰�
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
    }
    public void TakeHit(float damage)
    {
        health -= damage;
        if (health <= 0.0f)
        {
            Destroy(transform.gameObject);
        }
    }

    private void HandleMovement()
    {
        float moveDirection = 0f;

        if (Input.GetKey(moveLeftKey))
            moveDirection = -1f;
        else if (Input.GetKey(moveRightKey))
            moveDirection = 1f;

        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);

        if (isGrounded && moveDirection != 0)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

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
            animator.SetBool("Jump", true);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKey(attackKey) && !attacking)
        {
            attacking = true;
            animator.speed = animSpeed;
            animator.SetTrigger("Attack1"); // ���� �ִϸ��̼� ����
        }
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ���Ͽ� ���� ����
    public void PerformAttack()
    {
        // ���� ��ġ ��� (���⿡ ���� ��/�� ����)
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(
            facingRight ? attackRangeOffset.x : -attackRangeOffset.x,
            attackRangeOffset.y
        );

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable monster = enemy.GetComponent<IDamageable>(); // `IMonster` �������̽��� ����
            if (monster != null)
            {
                monster.TakeHit(attackDamage);
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Run", false);
            animator.SetBool("Jump", true);
        }
    }

    // ������ ������ �ִϸ��̼� �̺�Ʈ���� ȣ���Ͽ� ���� ���� ���·� ����
    public void ResetAttack()
    {
        attacking = false;
        animator.speed = 1.0f; // �⺻ �ӵ��� ����
    }

    private void OnDrawGizmosSelected()
    {
        // ���� ���� �� ��ġ ǥ��
        Gizmos.color = Color.red;
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(
            facingRight ? attackRangeOffset.x : -attackRangeOffset.x,
            attackRangeOffset.y
        );
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}
