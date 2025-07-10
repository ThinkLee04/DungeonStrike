using UnityEngine;

public class EmptyChest : MonoBehaviour
{
    public AudioSource openSound;
    public GameObject doorToOpen;

    private Animator animator;
    private bool isOpened = false;

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
