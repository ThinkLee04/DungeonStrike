using UnityEngine;

public class Healing : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount);
            }
        }
    }
}
