using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public InputField nameInput;

    public void PlayGame()
    {
        string playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Vui lòng nhập tên!");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName); // Lưu tên
        GameData.PlayerName = playerName; // Cập nhật dữ liệu game
        GameData.TotalSands = 0; // Reset tổng số cát khi bắt đầu game mới
        SceneManager.LoadScene("Level 1"); // Bắt đầu game
    }
}
