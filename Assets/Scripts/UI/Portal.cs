using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text coinsText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                GameData.TotalCoins = player.GetCoins(); // giữ nguyên score sau màn cuối
                GameData.CurrentHealth = player.GetCurrentHealth(); // nếu muốn dùng lại máu
            }

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Level 1")
            {
                if (other.CompareTag("Player"))
                {
                    if (player != null && player.HasKey)
                    {
                        Debug.Log("Door opened! Loading next level...");
                        SceneManager.LoadScene(nextSceneName);
                    }
                    else
                    {
                        Debug.Log("You need a key to open this door.");
                    }
                }
            }

            if (currentScene == "Level 3")
            {
                Time.timeScale = 0f;

                if (winPanel != null)
                {
                    winPanel.SetActive(true);

                    if (coinsText != null)
                    {
                        coinsText.text = $"Total score: {GameData.TotalCoins}";
                    }

                    SaveHighScore(GameData.PlayerName, GameData.TotalCoins);
                }
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    [System.Serializable]
    public class HighScoreEntry
    {
        public string name;
        public int score;
    }

    [System.Serializable]
    public class HighScoreList
    {
        public List<HighScoreEntry> highScoreEntries = new List<HighScoreEntry>();
    }

    private void SaveHighScore(string name, int score)
    {
        // Lấy JSON hiện có
        string json = PlayerPrefs.GetString("HighScoreTable", "{}");
        HighScoreList highScoreList = JsonUtility.FromJson<HighScoreList>(json);

        if (highScoreList == null)
            highScoreList = new HighScoreList();

        // Thêm entry mới
        HighScoreEntry newEntry = new HighScoreEntry { name = name, score = score };
        highScoreList.highScoreEntries.Add(newEntry);

        // Sắp xếp giảm dần theo score
        highScoreList.highScoreEntries.Sort((a, b) => b.score.CompareTo(a.score));

        // Giới hạn 10 người
        if (highScoreList.highScoreEntries.Count > 10)
            highScoreList.highScoreEntries.RemoveRange(10, highScoreList.highScoreEntries.Count - 10);

        // Lưu lại
        string newJson = JsonUtility.ToJson(highScoreList);
        PlayerPrefs.SetString("HighScoreTable", newJson);
        PlayerPrefs.Save();

        Debug.Log($"Saved new high score: {name} - {score}");
    }
}
