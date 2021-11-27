using CharacterThings;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    public static Player Instance { get; private set; }

    [Header("Component")]
    private BetterJumping betterJumping;
    private PlayerInputAction playerInputAction;

    private float horizontal;

    [Space]
    [Header("MovementStats")]
    public float movementSpeed = 10;
    public float jumpForce = 50;
    public float wallJumpForce = 1.5f;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public int extraJumps = 1;
    [SerializeField]
    private int availableExtraJumps;

    [Space]
    [Header("Checkers")]
    public bool canMove;
    public bool wallSlide;
    public bool isDashing;
    public bool jumpStop;

    [Space]
    private bool hasDashed;

    [Space]
    [Header("AbilityChecks")]
    public bool canGrab;
    public bool canDoubleJump;
    public bool canDash;

    [Space]
    [Header("Health and action points")]
    [SerializeField]
    public Image imageHP;

    [SerializeField]
    private Text textHP;

    [SerializeField]
    private Image imageAP;

    [SerializeField]
    private Text textAP;

    #region Properties
    public float Horizontal => horizontal;

    public bool IsDashing => isDashing;

    public bool WallSlide => wallSlide;

    #endregion


    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        playerInputAction = new PlayerInputAction();

        playerInputAction.Player.Move.performed += ctx => MovementInput(ctx);
        playerInputAction.Player.Jump.performed += ctx => OnJumpPerformedInput();
        playerInputAction.Player.Jump.canceled += ctx => OnJumpCancelledInput();
        playerInputAction.Player.Dash.performed += ctx => Dash();

        playerInputAction.Enable();
    }

    protected override void Start()
    {
        base.Start();
        betterJumping = GetComponent<BetterJumping>();

        HandleHPBar();
        HandleAPBar();


        availableExtraJumps = extraJumps;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isGrounded && !isDashing)
        {
            //wallJumped = false;
            betterJumping.enabled = true;
        }    

        if (!onWall || isGrounded)
            wallSlide = false;

        HandleFlip();
        CheckKnockback();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isDashing)
        {
            HandleMovement();
            if (isGrounded)
            {
                hasDashed = false;
                isDashing = false;
                jumpStop = false;
                if (!(Rigidbody.velocity.y > 0f))
                    availableExtraJumps = extraJumps;
            }
            else
            {
                if (onWall && !isGrounded)
                {
                    if (horizontal != 0)
                    {
                        wallSlide = true;
                        availableExtraJumps = extraJumps;
                        WallSliding();
                    }
                    else
                    {
                        wallSlide = false;
                    }
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (!canMove)
            return;

        Rigidbody.velocity = new Vector2(horizontal * movementSpeed, Rigidbody.velocity.y);
    }

    private void HandleFlip()
    {
        if ((facingRight && horizontal < 0) || (!facingRight && horizontal > 0))
        {
            Flip();
        }
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        if (canMove && !isDashing && !wallSlide)
        {
            horizontal = context.ReadValue<Vector2>().x;
            Rigidbody.velocity = new Vector2(horizontal * movementSpeed, Rigidbody.velocity.y);
        }
        else
            horizontal = 0;
    }

    /*public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!wallSlide)
                Jump();
            else
                WallJump();
        }
        if (context.canceled)
        {
            jumpStop = true;
        }
    }*/

    public void OnJumpPerformedInput()
    {
        if (!wallSlide)
            Jump();
        else
            WallJump();
    }

    public void OnJumpCancelledInput()
    {
        jumpStop = true;
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Dash();
        }
    }

    private void Dash()
    {
        if (!canDash)
            return;

        if (hasDashed)
            return;

        hasDashed = true;

        Rigidbody.velocity = Vector2.zero;
        int direction = facingRight ? 1 : -1;
        Vector2 dir = new Vector2(direction, 0);

        Rigidbody.velocity += dir.normalized * dashSpeed;

        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

        float originalGravity = Rigidbody.gravityScale;

        Rigidbody.gravityScale = 0;
        betterJumping.enabled = false;
        isDashing = true;

        yield return new WaitForSeconds(0.3f);

        Rigidbody.gravityScale = originalGravity;
        betterJumping.enabled = true;

        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (isGrounded)
        {
            hasDashed = false;
        }
    }

    private void WallJump()
    {
        if (!canGrab)
            return;

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        //canMove = false;

        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;

        Rigidbody.velocity = new Vector2(wallDir.x * 0.25f * jumpForce, 0.75f * jumpForce);
    }

    private void WallSliding()
    {
        if (!canGrab)
            return;

        if (!canMove)
            return;

        betterJumping.enabled = false;

        bool pushingWall = false;
        if ((Rigidbody.velocity.x > 0 && onRightWall) || (Rigidbody.velocity.x < 0 && !onRightWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : Rigidbody.velocity.x;
        Rigidbody.velocity = new Vector2(push, -slideSpeed);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            //availableJumps--;
            Rigidbody.velocity = Vector2.up * jumpForce;
        }
        else
        {
            if (availableExtraJumps > 0)
            {
                availableExtraJumps--;
                Rigidbody.velocity = Vector2.up * jumpForce;
            }
        }
    }
    protected IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    public override void SetVelocity(float velocity)
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.5f));
        base.SetVelocity(velocity);
    }

    public override void SetVelocity(Vector2 velocity)
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.5f));
        base.SetVelocity(velocity);
    }

    public override void SetVelocity(Vector2 velocity, int direction)
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.5f));
        base.SetVelocity(velocity, direction);
    }

    public override void SetVelocity(float velocity, Vector2 angle)
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.5f));
        base.SetVelocity(velocity, angle);
    }

    public override void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.5f));
        base.SetVelocity(velocity, angle, direction);
    }

    protected override void TakeDamage(AttackDetails attackDetails)
    {
        if (!IsInvincible)
        {
            float reduction = CharacterStats.CurrentDefense / (CharacterStats.CurrentDefense + 500);
            float multiplier = 1 - reduction;
            int incomingDMG = (int)(attackDetails.damageAmount * multiplier);

            CharacterStats.CurrentHP -= incomingDMG;

            HandleHPBar();

            GameObject damageOBJ = PoolManager.SpawnObject(GameManager.Instance.DamagePopup);
            DamagePopup damagePopup = damageOBJ.GetComponent<DamagePopup>();
            damagePopup.SetPopup(incomingDMG, DamageType.NormalDamage, transform.position);

            if (CharacterStats.CurrentHP > 0)
            {
                StartCoroutine(BecomeInvicible(1.5f));
                int direction = attackDetails.position.x < transform.position.x ? 1 : -1;

                Knockback(direction);
            }
        }
    }

    public override void Knockback(int direction)
    {
        knockback = true;
        DisableMove();
        knockbackStartTime = Time.time;
        Rigidbody.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    protected override void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            Rigidbody.velocity = new Vector2(0.0f, Rigidbody.velocity.y);
            EnableMove();           
        }
    }

    public void EnableMove()
    {
        canMove = true;
    }

    public void DisableMove()
    {
        canMove = false;
    }

    public void HandleHPBar()
    {
        imageHP.fillAmount = (float)CharacterStats.CurrentHP / (float)CharacterStats.MaxHP;
        textHP.text = CharacterStats.CurrentHP + " / " + CharacterStats.MaxHP;
    }

    public void HandleAPBar()
    {
        imageAP.fillAmount = (float)CharacterStats.CurrentAP / (float)CharacterStats.MaxAP;
        textAP.text = (int)CharacterStats.CurrentAP + " / " + CharacterStats.MaxAP;
    }
}
