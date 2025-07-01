using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public GameObject rankingPanel;
    public Text rankingText;

    public void ShowRanking()
    {
        rankingPanel.SetActive(true);

        int count = PlayerPrefs.GetInt("RankCount", 0);
        var entries = new List<(string name, int score)>();

        // Lấy dữ liệu từ PlayerPrefs
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"RankName{i}", "Unknown");
            int score = PlayerPrefs.GetInt($"RankSands{i}", 0);
            entries.Add((name, score));
        }

        // Sắp xếp theo score giảm dần
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Hiển thị
        string result = "";
        for (int i = 0; i < entries.Count; i++)
        {
            result += $"{i + 1}. {entries[i].name} - {entries[i].score} sands\n";
        }

        rankingText.text = result;
    }


    public void CloseRanking()
    {
        rankingPanel.SetActive(false);
    }
}
