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
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng vừa thoát khỏi va chạm có phải là Player không.
        if (collision.gameObject.CompareTag("Player"))
        {
            // Nếu đúng, ra lệnh cho EnemyController ngừng hành động tấn công.
            if (enemyController != null)
            {
                // Gọi hàm mới mà chúng ta đã tạo trong EnemyController.
                enemyController.StopAttack();
            }
        }
    }
}