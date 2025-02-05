using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("캐릭터 스텟")]
    [SerializeField] private float health = 100f; // 체력
    [SerializeField] private float defense = 0f; // 방어력
    [SerializeField] private float moveSpeed = 10f; // 이동속도
    [SerializeField] private float jumpHeight = 5f; // 점프높이
    [SerializeField] private float attackRange = 1.5f; // 공격 범위 값
    [SerializeField] private float attackDamage = 20f; // 공격력
    [SerializeField] private float attackSpeed = 1.0f; // 애니메이션 재생속도
    [SerializeField] private bool attacking = false; // 공격 애니메이션 중인지 확인

    [Header("공격 콜라이더")]
    [SerializeField] private GameObject attackColliderPrefab; // 공격 충돌 판정용 프리팹
    private GameObject currentAttackCollider;

    [Header("키 값")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A; // 좌
    [SerializeField] private KeyCode moveRightKey = KeyCode.D; // 우
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // 점프
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0; // 마우스 왼쪽 클릭

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 컴포넌트 참조
        animator = GetComponent<Animator>(); // 애니메이터 참조
        animator.SetFloat("AttackSpeed", attackSpeed); // 애니메이션 속도 설정
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

        // 이동 애니메이션 실행
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

            // 점프 애니메이션 실행
            animator.SetTrigger("Jump");
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKey(attackKey) && !attacking)
        {
            attacking = true;
            animator.speed = attackSpeed; // 공격 중 애니메이션 속도 조절
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

    // 애니메이션 이벤트 - 공격 시작 시 콜라이더 생성
    public void CreateAttackCollider()
    {
        if (attackColliderPrefab != null && currentAttackCollider == null)
        {
            Vector3 spawnPosition = transform.position + (facingRight ? Vector3.right : Vector3.left) * attackRange;
            currentAttackCollider = Instantiate(attackColliderPrefab, spawnPosition, Quaternion.identity);
            currentAttackCollider.transform.parent = transform;
        }
    }

    // 애니메이션 이벤트 - 공격 종료 시 콜라이더 삭제 및 상태 초기화
    public void DestroyAttackCollider()
    {
        if (currentAttackCollider != null)
        {
            Destroy(currentAttackCollider);
        }
    }

    // 애니메이션 이벤트 - 공격 종료 시 공격 가능 상태로 변경
    public void ResetAttack()
    {
        attacking = false;
        animator.speed = 1.0f; // 기본 속도로 복구
    }
}
