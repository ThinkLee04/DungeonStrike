using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    private PlayerController player;

    [System.Obsolete]
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        UpdateCoinsUI();
    }

    void Update()
    {
        UpdateCoinsUI();
    }

    public void UpdateCoinsUI()
    {
        if (player != null && coinsText != null)
        {
            coinsText.text = player.GetCoins().ToString();
        }
    }
}