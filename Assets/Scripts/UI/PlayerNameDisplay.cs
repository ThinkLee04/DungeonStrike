using UnityEngine;
using UnityEngine.UI;

public class PlayerNameDisplay : MonoBehaviour
{
    public Text welcomeText;

    void Start()
    {
        string name = GameData.PlayerName;

        // Nếu GameData chưa có, thử lấy từ PlayerPrefs
        if (string.IsNullOrEmpty(name))
        {
            name = PlayerPrefs.GetString("PlayerName", "Player");
            GameData.PlayerName = name; // cập nhật ngược lại
        }

        welcomeText.text = $"Welcome, {name}";
    }
}
