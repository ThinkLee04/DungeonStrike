using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text coinsText;
    [SerializeField] private AudioClip winSound;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

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
                    if (player != null && player.hasKey)
                    {
                        Debug.Log("Player has key → opening door!");

                        OpenDoor();

                        Invoke(nameof(GoToNextScene), 1f);
                    }
                }
            }

            if (currentScene == "Level 2")
            {
                if (other.CompareTag("Player"))
                {
                    if (player != null && player.bossKilled)
                    {
                        OpenDoor();

                        Invoke(nameof(GoToNextScene), 1f);
                    }
                }
            }

            if (currentScene == "Level 3")
            {
                if (other.CompareTag("Player"))
                {
                    if (player != null)
                    {
                        OpenDoor();

                        Invoke(nameof(EndGame), 1f);
                    }
                }
            }
        }
    }

    public void OpenDoor()
    {
        animator.SetTrigger("open");
    }

    private void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
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

    private void EndGame()
    {
        Time.timeScale = 0f;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            SoundUtils.PlaySoundAndDestroy(winSound, transform.position);
            if (coinsText != null)
            {
                coinsText.text = $"Total score: {GameData.TotalCoins}";
            }

            SaveHighScore(GameData.PlayerName, GameData.TotalCoins);
        }
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
