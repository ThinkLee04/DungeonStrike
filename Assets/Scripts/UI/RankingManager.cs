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
        string result = "";

        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"RankName{i}", "Unknown");
            int score = PlayerPrefs.GetInt($"RankSands{i}", 0);
            result += $"{i + 1}. {name} - {score} sands\n";
        }

        rankingText.text = result;
    }

    public void CloseRanking()
    {
        rankingPanel.SetActive(false);
    }
}
