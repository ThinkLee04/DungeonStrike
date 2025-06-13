using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private Animator animator;
    private bool isMarkedForDeath = false;
    private bool isDead = false;

    private EnemyController enemyController;
    private EnemyAI enemyAI;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        enemyController = GetComponent<EnemyController>();
        enemyAI = GetComponent<EnemyAI>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ArrowTag"))
        {
            // Nếu va chạm với Player, gọi hàm TakeDamage
            Vector2 attackDirection = collision.relativeVelocity.normalized; // Lấy hướng tấn công từ vận tốc va chạm
            TakeDamage(attackDirection);
        }
    }
    // Gọi hàm này khi enemy bị player tấn công
    public void TakeDamage(Vector2 attackDirection)
    {
        if (isDead || isMarkedForDeath) return;

        currentHealth--;

        if (enemyController) enemyController.OnHurt();

        // Gửi hướng bị thương vào animator
        animator.SetFloat("PosX", attackDirection.x);
        animator.SetFloat("PosY", attackDirection.y);
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            isMarkedForDeath = true;
        }
    }
    public void HandleHurtAnimationEnd()
    {
        if (isDead) 
        {
            return;
        }
        if (isMarkedForDeath)
        {
            Die();
        } 
        else
        {
            if (enemyAI != null)
            {
                enemyAI.RestartAIBehavior();
            }
            if(enemyController != null)
            {
                enemyController.AfterHurt();
            }
        }
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Dead");

        if (enemyController != null) enemyController.OnDead();
        if (enemyAI != null) enemyAI.enabled = false;

        Destroy(gameObject, 1f); // Hủy đối tượng sau 2 giây
    }
}
