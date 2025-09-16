namespace Driball
{
    using Driball.Enemies;
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;

        [Header("Jump Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float jumpHeight = 0.5f;
        [SerializeField] private float jumpDuration = 0.5f;
        [SerializeField] private float jumpBufferTime = 0.15f;
        [SerializeField] private Animator animator;

        private Rigidbody2D rb;
        private Controls inputActions;
        private Vector2 moveInput;
        private Vector2 currentVelocity;
        private Transform spriteTransform;

        private bool isJumping;
        private bool isFalling;
        private bool isGrounded = true;
        private float jumpTimer;
        private Vector3 spriteStartPos;
        private float jumpBufferCounter;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            inputActions = new Controls();
        }

        private void OnEnable()
        {
            EnableInput();
            SubscribeEvents();
        }

        private void OnDisable()
        {
            DisableInput();
            UnsubscribeEvents();
        }

        private void Start()
        {
            spriteTransform = spriteRenderer.transform;
            spriteStartPos = spriteTransform.localPosition;
        }

        private void FixedUpdate() => HandleMovement();

        private void Update()
        {
            HandleJumpBuffer();
            HandleJumpProgress();
        }

        #region Input
        private void EnableInput()
        {
            inputActions.Player.Enable();
            inputActions.Player.Movement.performed += OnMove;
            inputActions.Player.Movement.canceled += OnMove;
            inputActions.Player.Jump.performed += OnJump;
        }

        private void DisableInput()
        {
            inputActions.Player.Movement.performed -= OnMove;
            inputActions.Player.Movement.canceled -= OnMove;
            inputActions.Player.Jump.performed -= OnJump;
            inputActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context) =>
            moveInput = context.ReadValue<Vector2>().normalized;

        private void OnJump(InputAction.CallbackContext context) =>
            jumpBufferCounter = jumpBufferTime;
        #endregion

        #region Movement
        private void HandleMovement()
        {
            Vector2 targetVelocity = moveInput * moveSpeed;
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.velocity = currentVelocity;
        }
        #endregion

        #region Jump
        private void HandleJumpBuffer()
        {
            if (jumpBufferCounter > 0f)
                jumpBufferCounter -= Time.deltaTime;

            if (!isJumping && jumpBufferCounter > 0f)
            {
                StartJump();
                jumpBufferCounter = 0f;
            }
        }

        private void HandleJumpProgress()
        {
            if (!isJumping || spriteTransform == null) return;

            jumpTimer += Time.deltaTime;
            float progress = jumpTimer / jumpDuration;

            if (progress >= 1f)
            {
                EndJump();
                return;
            }

            spriteRenderer.sortingOrder = 1;
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            spriteTransform.localPosition = spriteStartPos + Vector3.up * height;
            isFalling = progress >= 0.9f;
        }

        private void StartJump()
        {
            isJumping = true;
            isGrounded = false;
            jumpTimer = 0f;
        }

        private void EndJump()
        {
            spriteRenderer.sortingOrder = 0;
            isJumping = false;
            isFalling = false;
            isGrounded = true;
            spriteTransform.localPosition = spriteStartPos;
            animator.SetTrigger("land");
            GameEvents.PlayerLanded();
        }
        #endregion

        #region Collision
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Gem"))
            {
                HandleGemCollision(collision);
            }
            else if (collision.CompareTag("Enemy"))
            {
                HandleEnemyCollision(collision);
            }
            else if (collision.CompareTag("Attack"))
            {
                HandleAttackCollision();
            }
        }

        private void HandleGemCollision(Collider2D collision) =>
            collision.GetComponent<Gem>()?.Collect();

        private void HandleEnemyCollision(Collider2D collision)
        {
            if (isFalling)
            {
                StartJump();
                Screenshake.Instance.ShakeCamera(1f);
                collision.GetComponent<Enemy>()?.Die();
            }
            else if (isGrounded)
            {
                GameEvents.PlayerHit();
                Screenshake.Instance.ShakeCamera(5f);
                collision.GetComponent<Enemy>()?.Die();
            }
        }

        private void HandleAttackCollision()
        {
            if (!isGrounded) return;

            GameEvents.PlayerHit();
            Screenshake.Instance.ShakeCamera(5f);
        }
        #endregion

        #region Events
        private void SubscribeEvents()
        {
            GameEvents.OnGameOver += Die;
            GameEvents.OnGemCompleted += Die;
            GameEvents.OnTimesUp += Die;
        }

        private void UnsubscribeEvents()
        {
            GameEvents.OnGameOver -= Die;
            GameEvents.OnGemCompleted -= Die;
            GameEvents.OnTimesUp -= Die;
        }
        #endregion

        private void Die() => gameObject.SetActive(false);
    }
}