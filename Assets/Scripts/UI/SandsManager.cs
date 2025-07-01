using UnityEngine;
using TMPro;

public class SandsManager : MonoBehaviour
{
    public TextMeshProUGUI sandsText;
    private PlayerController player;

    [System.Obsolete]
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        UpdateSandsUI();
    }

    void Update()
    {
        UpdateSandsUI();
    }

    public void UpdateSandsUI()
    {
        if (player != null && sandsText != null)
        {
            sandsText.text = player.GetSands().ToString();
        }
    }
}