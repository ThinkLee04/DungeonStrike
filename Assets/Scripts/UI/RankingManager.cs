using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    [Header("Ranking UI")]
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private RectTransform entryContainer;
    [SerializeField] private RectTransform entryTemplate;

    private List<GameObject> spawnedEntries = new List<GameObject>();

    private class RankEntry
    {
        public string name;
        public int score;
    }

    private void Start()
    {
        rankingPanel.SetActive(false);
        entryTemplate.gameObject.SetActive(false); // đảm bảo ẩn template
    }

    public void ShowRanking()
    {
        rankingPanel.SetActive(true);

        // Clear entry cũ
        foreach (GameObject entry in spawnedEntries)
        {
            Destroy(entry);
        }
        spawnedEntries.Clear();

        // Load JSON từ PlayerPrefs
        string json = PlayerPrefs.GetString("HighScoreTable", "{}");
        HighScoreList data = JsonUtility.FromJson<HighScoreList>(json);
        if (data == null || data.highScoreEntries == null) return;

        float templateHeight = 40f;

        for (int i = 0; i < data.highScoreEntries.Count; i++)
        {
            var highscore = data.highScoreEntries[i];

            RectTransform entry = Instantiate(entryTemplate, entryContainer);
            entry.gameObject.SetActive(true);
            entry.anchoredPosition = new Vector2(0, -templateHeight * i); // THỦ CÔNG: xếp dọc

            Text posText = entry.Find("posText")?.GetComponent<Text>();
            Text scoreText = entry.Find("scoreText")?.GetComponent<Text>();
            Text nameText = entry.Find("nameText")?.GetComponent<Text>();

            if (posText != null) posText.text = (i + 1).ToString();
            if (scoreText != null) scoreText.text = highscore.score.ToString();
            if (nameText != null) nameText.text = highscore.name;

            spawnedEntries.Add(entry.gameObject);
        }
    }

    public void CloseRanking()
    {
        rankingPanel.SetActive(false);
    }

    [System.Serializable]
    private class HighScoreEntry
    {
        public string name;
        public int score;
    }

    [System.Serializable]
    private class HighScoreList
    {
        public List<HighScoreEntry> highScoreEntries;
    }
}
