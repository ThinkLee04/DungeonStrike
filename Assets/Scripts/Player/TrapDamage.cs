using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}
