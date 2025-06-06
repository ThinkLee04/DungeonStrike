using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private void Start()
    {
        // Bỏ qua va chạm với Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D arrowCollider = GetComponent<Collider2D>();
            if (playerCollider != null && arrowCollider != null)
            {
                Physics2D.IgnoreCollision(arrowCollider, playerCollider);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }
}
