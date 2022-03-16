using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu: MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas exitCanvas;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ConfirmExit()
    {
        mainCanvas.gameObject.SetActive(false);
        exitCanvas.gameObject.SetActive(true);
    }

    public void CancelExit()
    {
        exitCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }
}
