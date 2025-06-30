using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    [SerializeField] private GameObject howToplaypanel;

    public void ShowPanel()
    {
        howToplaypanel.SetActive(true);
        howToplaypanel.transform.SetAsLastSibling(); // Đảm bảo ở trên cùng
    }

    public void HidePanel()
    {
        howToplaypanel.SetActive(false);
    }
}
