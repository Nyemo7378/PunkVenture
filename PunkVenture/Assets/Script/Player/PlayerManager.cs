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
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode switchWeaponKey = KeyCode.Q;

    [Header("대시 관련")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;
    private bool canDash = true;

    [Header("설정 UI")]
    [SerializeField] private GameObject settingPrefab;
    private GameObject settingInstance;

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
        animator.SetFloat("AttackSpeed", animSpeed);

        // 설정 UI 프리팹을 UICanvas에 추가
        GameObject uiCanvas = GameObject.Find("UICanvas");
        if (uiCanvas != null && settingPrefab != null)
        {
            settingInstance = Instantiate(settingPrefab, uiCanvas.transform);
            settingInstance.SetActive(false);
        }
    }

    public void AddExp(float expAmount)
    {
        EXP += expAmount;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
        HandleDash();
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingInstance != null)
        {
            settingInstance.SetActive(!settingInstance.activeSelf);
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(dashKey) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dashDirection = (mousePosition - (Vector2)transform.position).normalized;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
            animator.SetTrigger("Attack1");
        }
    }

    public void ResetAttack()
    {
        attacking = false;
        animator.speed = 1.0f;
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
}