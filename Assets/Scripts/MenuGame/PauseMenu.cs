using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    
   public void PauseGame()
    {
         Debug.Log("Pause Clicked");

    Time.timeScale = 0f;

    if (pauseMenuUI != null)
    {
        pauseMenuUI.SetActive(true);
    }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Home()
    {
     Debug.Log("Home Clicked");

    Time.timeScale = 1f;
    SceneManager.LoadScene("MenuGame");
    }

}
