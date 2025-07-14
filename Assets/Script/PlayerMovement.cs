using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private float mobileInputX = 0f;
    private Vector2 moveInput;
    private bool isJumping = false;
    private bool isKnockedBack = false;
    public CoinManager cm;

    private enum MovementState
    {
        idle = 0,
        run = 1,
        jump = 2,
        attack1 = 3,
        attack2 = 4,
        attack3 = 5,
        death = 6
    }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;
    public TextMeshProUGUI healthText;

    [Header("Coin System")]
    private int currentCoin = 0;
    public TextMeshProUGUI coinText;

    [Header("Chest UI")]
    public TextMeshProUGUI chestText;

    [Header("Knockback Settings")]
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private float knockBackThrust = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerController = new PlayerController();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void OnEnable()
    {
        playerController.Enable();

        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
        playerController.Movement.Jump.performed += ctx => Jump();
        playerController.Movement.Attack1.performed += HandleAttack1;
        playerController.Movement.Attack2.performed += HandleAttack2;
        playerController.Movement.Attack3.performed += HandleAttack3;
    }

    private void OnDisable()
    {
        playerController.Movement.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled -= ctx => moveInput = Vector2.zero;
        playerController.Movement.Jump.performed -= ctx => Jump();
        playerController.Movement.Attack1.performed -= HandleAttack1;
        playerController.Movement.Attack2.performed -= HandleAttack2;
        playerController.Movement.Attack3.performed -= HandleAttack3;
        playerController.Disable();
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileInputX, 0f);
        }
        else
        {
            moveInput = playerController.Movement.Move.ReadValue<Vector2>();
        }

        UpdateChestUI();
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;

        Vector2 targetVelocity = new Vector2((moveInput.x + mobileInputX) * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;

        UpdateAnimation();

        if (isGrounded() && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            isJumping = false;
        }
    }

    public void TakeDamage(int damage, Vector2 direction)
    {
        if (isKnockedBack) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player Mati");
        }

        StartCoroutine(HandleKnockback(direction.normalized));
        UpdateHealthUI();
    }

    private IEnumerator HandleKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;

        Vector2 force = direction * knockBackThrust * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    public void addCoin(int amount)
    {
        currentCoin += amount;
        if (coinText != null)
        {
            coinText.text = "Coin : " + currentCoin.ToString();
        }
    }


    private void UpdateChestUI()
    {
        if (chestText != null && cm != null)
            chestText.text = "Coin: " + cm.coinCount;
    }

    private void UpdateAnimation()
    {
        MovementState state;

        // Jangan ubah animasi kalau sedang knockback
        if (isKnockedBack) return;

        float horizontal = moveInput.x != 0 ? moveInput.x : mobileInputX;

        // Deteksi animasi attack yang sedang berlangsung
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("attackmc1") ||
            currentState.IsName("attackmc2") ||
            currentState.IsName("attackmc3") ||
            currentState.IsName("deathmc"))
        {
            // Biarkan animasi attack/death selesai sendiri (Has Exit Time)
            return;
        }

        // Pilih animasi gerak
        if (horizontal != 0f)
        {
            state = MovementState.run;
            sprite.flipX = horizontal < 0f;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f || rb.velocity.y < -0.1f)
        {
            state = MovementState.jump;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void Jump()
    {
        if (isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    public void MoveRight(bool isPressed)
    {
        if (isPressed)
            mobileInputX = 1f;
        else if (mobileInputX == 1f)
            mobileInputX = 0f;
    }

    public void MoveLeft(bool isPressed)
    {
        if (isPressed)
            mobileInputX = -1f;
        else if (mobileInputX == -1f)
            mobileInputX = 0f;
    }

    public void MobileJump()
    {
        if (isGrounded())
        {
            Jump();
        }
    }

    public void PlayAttack1() => anim.SetInteger("state", (int)MovementState.attack1);
    public void PlayAttack2()
    {
        Debug.Log("PlayAttack2: Setting state = 4");
        anim.SetInteger("state", (int)MovementState.attack2);
    }

    public void PlayAttack3()
    {
        Debug.Log("PlayAttack3: Setting state = 5");
        anim.SetInteger("state", (int)MovementState.attack3);
    }

    public void Die() => anim.SetInteger("state", (int)MovementState.death);

    private void HandleAttack1(InputAction.CallbackContext context)
    {
        Debug.Log("Attack1 (K) Triggered");
        PlayAttack1();
    }

    private void HandleAttack2(InputAction.CallbackContext context)
    {
        Debug.Log("Attack2 (L) Triggered");
        PlayAttack2();
    }

    private void HandleAttack3(InputAction.CallbackContext context)
    {
        Debug.Log("Attack3 (M) Triggered");
        PlayAttack3();
    }
}
