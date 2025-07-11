using UnityEngine;

public class CoinChest : MonoBehaviour
{
    public bool isKeyChest = false;
    public GameObject coinPrefab;
    public int coinAmount = 9;
    public AudioClip openSound;
    private bool isOpened = false;
    private Animator animator;
    //private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isOpen", false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        if (animator != null)
        {
            animator.SetBool("isOpen", true);
        }

        //if (openSound && audioSource)
        //    audioSource.PlayOneShot(openSound);

        Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0);
        for (int i = 0; i < coinAmount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 1.5f), 0);
            GameObject coin = Instantiate(coinPrefab, transform.position + offset, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(3f, 5f)), ForceMode2D.Impulse);
            }
        }
    }
}
