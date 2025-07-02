using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("WaterHazard"))
        {
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AttackedTag"))
        {
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}
