using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Gọi hàm này khi enemy bị player tấn công
    public void TakeDamage(Vector2 attackDirection)
    {
        if (isDead) return;

        currentHealth--;

        // Gửi hướng bị thương vào animator
        animator.SetFloat("HitX", attackDirection.x);
        animator.SetFloat("HitY", attackDirection.y);
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        animator.SetTrigger("Die");

        // Ngăn enemy tiếp tục di chuyển
        //GetComponent<EnemyAI>()?.enabled = false;
        //GetComponent<EnemyController>()?.enabled = false;
        //GetComponent<EnemyAttack>()?.enabled = false;

        // Có thể thêm delay rồi Destroy(gameObject) nếu muốn enemy biến mất
    }
}
