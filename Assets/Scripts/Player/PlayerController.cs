using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float arrowRange = 5f;
    [SerializeField] private float shootCooldown = 3f;
    [SerializeField] private float shootDelay = 0.08f;

    [Header("Damage Settings")]
    [SerializeField] private float invincibleDuration = 2f;
    [SerializeField] private int maxHealth = 5;     // Máu tối đa

    [SerializeField] private GameObject gameOverPanel;


    private int currentHealth;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool canShoot = true;
    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Combat.Shoot.performed += _ => Shoot();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Combat.Shoot.performed -= _ => Shoot();
    }

    private void Update()
    {
        PlayerInput();

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        AdjustPlayerFacingDirection();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);
        animator.SetFloat("speed", movement.magnitude);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Shoot()
    {
        if (!canShoot) return;

        canShoot = false;
        animator.SetTrigger("shoot");

        Invoke(nameof(SpawnArrow), shootDelay);
        Invoke(nameof(EnableShooting), shootCooldown);
    }

    private void SpawnArrow()
    {
        if (arrowPrefab == null) return;

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.linearVelocity = direction * arrowSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(arrow, arrowRange / arrowSpeed);
    }

    private void EnableShooting()
    {
        canShoot = true;
    }

    // ---------------- Damage & Invincibility ----------------
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            Debug.Log("Player took damage. Current Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                Hurt();
            }
        }
    }

    private void Hurt()
    {
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        animator.SetTrigger("isHurting");
    }

    private void Die()
    {
        Debug.Log("Player died.");
        animator.SetTrigger("die");

        this.enabled = false;
        rb.linearVelocity = Vector2.zero;
        gameOverPanel.SetActive(true);

        // Bắt Coroutine đợi 2 giây realtime rồi pause game
        StartCoroutine(PauseGameAfterDelay(1.2f));
    }

    private IEnumerator PauseGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);  // Đợi 2 giây thật (bất chấp timeScale)
        Time.timeScale = 0f;
    }


    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

}
