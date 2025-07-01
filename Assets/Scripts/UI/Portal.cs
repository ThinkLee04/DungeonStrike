using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // hoặc TMPro nếu dùng TextMeshPro

public class Portal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text sandsText; // Nếu dùng TextMeshPro thì là TMP_Text

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                GameData.TotalSands += player.GetSands(); // cộng dồn sands
            }

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "SampleScene")
            {
                Time.timeScale = 0f;

                if (winPanel != null)
                {
                    winPanel.SetActive(true);

                    // Hiển thị tổng số sands lên text
                    if (sandsText != null)
                    {
                        sandsText.text = $"Total Sands: {GameData.TotalSands}";
                    }
                    else
                    {
                        Debug.LogWarning("Chưa gán SandsText trong Portal script.");
                    }
                    SaveRank(GameData.PlayerName, GameData.TotalSands);
                }
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
    private void SaveRank(string playerName, int totalSands)
    {
        // Tăng số lượng người chơi đã lưu
        int rankCount = PlayerPrefs.GetInt("RankCount", 0);

        // Lưu tên và điểm theo key riêng
        PlayerPrefs.SetString($"RankName{rankCount}", playerName);
        PlayerPrefs.SetInt($"RankSands{rankCount}", totalSands);

        // Tăng số lượng xếp hạng
        PlayerPrefs.SetInt("RankCount", rankCount + 1);

        PlayerPrefs.Save(); // đảm bảo lưu ngay
        Debug.Log($"Saved {playerName} with {totalSands} sands to rank.");
    }

}
