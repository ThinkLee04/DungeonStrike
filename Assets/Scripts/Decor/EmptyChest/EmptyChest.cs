using UnityEngine;

public class EmptyChest : MonoBehaviour
{
    public AudioSource openSound;
    public GameObject doorToOpen;
    public GameObject ExchangeItemPanel;

    private Animator animator;
    private bool isOpened = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isOpen", false);
        }
        if (ExchangeItemPanel != null)
        {
            ExchangeItemPanel.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
        if (isOpened && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (ExchangeItemPanel != null)
            {
                ExchangeItemPanel.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ExchangeItemPanel != null)
        {
            ExchangeItemPanel.SetActive(false);
        }
    }

    private void OpenChest()
    {
        isOpened = true;

        if (animator != null)
        {
            animator.SetBool("isOpen", true);
        }

        //if (openSound != null)
        //{
        //    openSound.Play();
        //}

        //if (doorToOpen != null)
        //{
        //    doorToOpen.SetActive(false);
        //}
    }
}
