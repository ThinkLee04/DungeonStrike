using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyController enemyController;
    private Vector2 lastMoveDirection = Vector2.down;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = direction.normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enemyController != null)
            {
                // SỬA ĐỔI: Truyền cả hướng tấn công và GameObject của Player
                enemyController.InitiateAttack(lastMoveDirection, collision.gameObject);
            }
            //demo 
            float delayInSeconds = 5f;
            Destroy(collision.gameObject, delayInSeconds);
            //demo
        }
    }
}