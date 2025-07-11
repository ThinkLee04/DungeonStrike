using UnityEngine;

public class KeyController : MonoBehaviour
{
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetHasKey(true); // cập nhật key + UI
                player.AddCoins(1);     // tăng coins
            }

            SoundUtils.PlaySoundAndDestroy(collectSound, transform.position);
            Destroy(gameObject);
        }
    }
}
