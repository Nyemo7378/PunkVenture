using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPlayer
{
    [Header("캐릭터 스탯")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private float EXP = 0;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Vector2 attackRangeOffset = new Vector2(1.0f, 0.0f);
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float animSpeed = 1.0f;
    [SerializeField] private bool attacking = false;

    [Header("공격 관련")]
    [SerializeField] private LayerMask enemyLayers;

    [Header("키 값")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("AttackSpeed", animSpeed);
    }

    public void AddExp(float expAmount)
    {
        EXP += expAmount; // 경험치 추가
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
            animator.SetTrigger("Jump");
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKey(attackKey) && !attacking)
        {
            attacking = true;
            animator.speed = animSpeed;
            animator.SetTrigger("Attack1");
        }
    }

    public void PerformAttack()
    {
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(
            facingRight ? attackRangeOffset.x : -attackRangeOffset.x,
            attackRangeOffset.y
        );

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            IMonster monster = enemy.GetComponent<IMonster>();
            if (monster != null)
            {
                monster.ApplyDamage(attackDamage);
            }
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
            animator.SetTrigger("Jump");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Run", false);
        }
    }

    public void ResetAttack()
    {
        attacking = false;
        animator.speed = 1.0f;
    }
}
