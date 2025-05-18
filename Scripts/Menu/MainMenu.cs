using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject options;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleOptions()
    {
        if( options.activeInHierarchy )
        {
            options.SetActive(false);
        }
        else
        {
            options.SetActive(true);
        }
    }

}
