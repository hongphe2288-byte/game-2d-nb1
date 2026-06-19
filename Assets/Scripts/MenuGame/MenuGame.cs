using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuGame : MonoBehaviour
{
   public void PlayGame()
   {
        SceneManager.LoadScene("map");
   }
   public void QuitGame()
   {
        Application.Quit();
   }
   public void ContinueGame()
   {
        SceneManager.LoadScene("Continue");
   }

}
