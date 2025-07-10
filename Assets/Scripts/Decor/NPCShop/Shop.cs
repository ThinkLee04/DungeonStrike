using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject ExchangeItemPanel;

    void Start()
    {
        if (ExchangeItemPanel != null)
        {
            ExchangeItemPanel.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
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
}
