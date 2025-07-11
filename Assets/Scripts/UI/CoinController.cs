using UnityEngine;

public class CoinController : MonoBehaviour
{
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddCoins(1);
            }

            SoundUtils.PlaySoundAndDestroy(collectSound, transform.position);

            Destroy(gameObject);
        }
    }
}
