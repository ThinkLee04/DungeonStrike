using UnityEngine;

public class BossDoorOpener : MonoBehaviour
{
    public DoorController door;   // Gán cửa cần mở ở Inspector
    private EnemyHealth enemyHealth;
    private bool hasOpenedDoor = false;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (!hasOpenedDoor && enemyHealth != null)
        {
            // Kiểm tra nếu boss đã chết
            if (IsDead())
            {
                if (door != null)
                {
                    door.OpenDoor();
                    hasOpenedDoor = true;
                }
            }
        }
    }

    private bool IsDead()
    {
        // EnemyHealth có biến private isDead,
        // nên phải thêm 1 hàm public get IsDead
        return enemyHealth.IsDead();
    }
}
