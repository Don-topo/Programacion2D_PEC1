using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button newGameButton;
    public Button loadGameButton;
    public Button exitGameButton;

    // Start is called before the first frame update
    private void Start()
    {
        AddListener(newGameButton);       
        exitGameButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddListener(Button button)
    {
        button.onClick.AddListener(() => { StartGame(); });
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Game"); 
    }


    private void ExitGame()
    {
        Application.Quit();
    }

    public static void EndGame()
    {
        SceneManager.LoadScene("EndGame");
    }
}
