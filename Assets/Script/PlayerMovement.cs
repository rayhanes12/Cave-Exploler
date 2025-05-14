using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private Vector2 moveInput;
    private bool isJumping = false;

    private enum MovementState { idle = 0, run = 1, jump = 2, att1 = 3, att2 = 4, att3 = 5, death = 6 }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    private Coroutine animResetCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();

        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        playerController.Movement.Jump.performed += ctx => Jump();
        playerController.Movement.Attack1.performed += ctx => PlayAttackAnimation(1);
        playerController.Movement.Attack2.performed += ctx => PlayAttackAnimation(2);
        playerController.Movement.Attack3.performed += ctx => PlayAttackAnimation(3);
        playerController.Movement.Death.performed += ctx => PlayDeathAnimation();
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void FixedUpdate()
    {
        if (anim.GetInteger("state") == (int)MovementState.death) return;

        Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;

        if (!IsInAttackOrDeath())
        {
            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (moveInput.x > 0f)
        {
            state = MovementState.run;
            sprite.flipX = false;
        }
        else if (moveInput.x < 0f)
        {
            state = MovementState.run;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
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
        if (isGrounded() && !IsInAttackOrDeath())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void PlayAttackAnimation(int attackIndex)
    {
        if (IsInAttackOrDeath()) return;

        if (attackIndex < 1 || attackIndex > 3) return;
        MovementState attackState = (MovementState)(2 + attackIndex); // att1=3, att2=4, att3=5
        anim.SetInteger("state", (int)attackState);

        if (animResetCoroutine != null) StopCoroutine(animResetCoroutine);
        animResetCoroutine = StartCoroutine(ResetToIdleAfter(0.5f));
    }

    private void PlayDeathAnimation()
    {
        if (IsInAttackOrDeath()) return;

        anim.SetInteger("state", (int)MovementState.death);
    }

    private IEnumerator ResetToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (IsInAttackOrDeath())
        {
            anim.SetInteger("state", (int)MovementState.idle);
        }
    }

    private bool IsInAttackOrDeath()
    {
        int state = anim.GetInteger("state");
        return state >= (int)MovementState.att1 && state <= (int)MovementState.death;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (coll == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(coll.bounds.center + Vector3.down * 0.1f, coll.bounds.size);
    }
#endif
}
