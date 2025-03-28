using UnityEngine;
using UnityEngine.SceneManagement;
public class Memu : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("GameList");
    }
}
