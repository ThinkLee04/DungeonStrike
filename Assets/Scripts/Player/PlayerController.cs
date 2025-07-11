using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] private int maxHealth = 4;

    [SerializeField] private GameObject canvasPrefabRoot;

    [Header("Coins Settings")]
    [SerializeField] private int coins;
    [SerializeField] private int hpToGain = 1;
    [SerializeField] private int coinsToExchange = 10;

    private GameObject gameOverPanel;
    private GameObject PausePanel;
    private GameObject HpPanel;

    private int currentHealth;
    private GameObject[] hpObjects;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public bool hasKey = false;
    private bool canShoot = true;
    private bool isInvincible = false;
    private float invincibleTimer = 0f;
    private bool isPaused = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth - 2;

        if (canvasPrefabRoot != null)
        {
            gameOverPanel = canvasPrefabRoot.transform.Find("GameOverPanel")?.gameObject;
            PausePanel = canvasPrefabRoot.transform.Find("PausePanel")?.gameObject;
        }
        else
        {
            Debug.LogError("Canvas prefab root is not assigned!");
        }
    }

    private void Start()
    {
        currentHealth = GameData.CurrentHealth > 0 ? GameData.CurrentHealth : maxHealth - 2;
        coins = GameData.TotalCoins;

        hpObjects = new GameObject[maxHealth + 1];
        for (int i = 0; i <= maxHealth; i++)
        {
            HpPanel = canvasPrefabRoot.transform.Find("HpPanel")?.gameObject;
            hpObjects[i] = HpPanel.transform.Find($"{i}HP")?.gameObject;

            if (hpObjects[i] == null)
                Debug.LogWarning($"Không tìm thấy object {i}HP trong HpPanel");
        }

        UpdateHealthUI();
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

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused) PauseGame();
            else ContinueGame();
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
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        spriteRenderer.flipX = mousePos.x < playerScreenPoint.x;
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

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            GameData.CurrentHealth = currentHealth;

            UpdateHealthUI();

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
        animator.SetTrigger("die");
        this.enabled = false;
        rb.linearVelocity = Vector2.zero;
        gameOverPanel?.SetActive(true);
        StartCoroutine(PauseGameAfterDelay(1.2f));
    }

    private IEnumerator PauseGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetCoins() => GameData.TotalCoins;

    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameData.TotalCoins = 0;
        GameData.CurrentHealth = maxHealth - 2;
        SceneManager.LoadScene("Level 1");
    }

    public void ReturnHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomePage");
    }

    public void PauseGame()
    {
        if (isPaused) return;

        Time.timeScale = 0f;
        isPaused = true;
        playerControls.Disable();
        PausePanel?.SetActive(true);
    }

    public void ContinueGame()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        isPaused = false;
        playerControls.Enable();
        PausePanel?.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i <= maxHealth; i++)
        {
            if (hpObjects[i] != null)
            {
                hpObjects[i].SetActive(i == currentHealth);
            }
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        GameData.CurrentHealth = currentHealth;
        UpdateHealthUI();
    }

    public void ExchangeCoinsForHP()
    {
        if (coins >= coinsToExchange && currentHealth < maxHealth)
        {
            coins -= coinsToExchange;
            currentHealth += hpToGain;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            GameData.TotalCoins = coins;
            GameData.CurrentHealth = currentHealth;

            UpdateHealthUI();
            FindObjectOfType<CoinsManager>()?.UpdateCoinsUI();
        }
    }
    public void AddCoins(int amount)
    {
        coins += amount;
        GameData.TotalCoins = coins;
        FindObjectOfType<CoinsManager>()?.UpdateCoinsUI();
    }
}
