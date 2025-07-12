using UnityEngine;

public class BossDoorOpener : MonoBehaviour
{
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
            PlayerController player = FindObjectOfType<PlayerController>();
            if (IsDead())
            {
                player.bossKilled = true;
                hasOpenedDoor = true;
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
