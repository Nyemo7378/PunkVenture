using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("캐릭터 스텟")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackSpeed = 1.0f;
    [SerializeField] private bool attacking = false;

    [Header("공격 콜라이더")]
    [SerializeField] private GameObject attackColliderPrefab;
    private GameObject currentAttackCollider;

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
        animator.SetFloat("AttackSpeed", attackSpeed);
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

        // 이동 애니메이션 실행 (Ground와 충돌 중일 때만)
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
            animator.speed = attackSpeed;
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
            animator.ResetTrigger("Jump"); // 점프 애니메이션 취소
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Run", false); // 지상에서 벗어나면 Run 비활성화
        }
    }

    public void CreateAttackCollider()
    {
        if (attackColliderPrefab != null && currentAttackCollider == null)
        {
            Vector3 spawnPosition = transform.position + (facingRight ? Vector3.right : Vector3.left) * attackRange;
            currentAttackCollider = Instantiate(attackColliderPrefab, spawnPosition, Quaternion.identity);
            currentAttackCollider.transform.parent = transform;
        }
    }

    public void DestroyAttackCollider()
    {
        if (currentAttackCollider != null)
        {
            Destroy(currentAttackCollider);
        }
    }

    public void ResetAttack()
    {
        attacking = false;
        animator.speed = 1.0f;
    }
}
