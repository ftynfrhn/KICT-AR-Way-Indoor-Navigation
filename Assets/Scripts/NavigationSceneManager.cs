using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationSceneManager : MonoBehaviour
{
    /// <summary>
    /// Navigate to the previous scene in the build settings.
    /// </summary>
    public void GoBack()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex > 0)
        {
            SceneManager.LoadScene(currentSceneIndex - 1);
        }
    }

    /// <summary>
    /// Exit the application.
    /// </summary>
    public void ExitApplication()
    {
        Application.Quit();
    }
}
