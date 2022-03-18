using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextAsset jsonText;
    public GameObject buttonPrefab;
    public GameObject insultUIPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Canvas pauseCanvas;
    public Sprite[] healthSprites;
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public TextMeshProUGUI playerBaloon;
    public TextMeshProUGUI enemyBaloon;
    [HideInInspector]
    public static bool playerWin = false;

    private string currentState;
    private Insults insults;
    private List<Insult> roundInsults;
    private Insult enemyInsult;
    private string playerInsult;
    private int playerHealth = 5;
    private int enemyHealth = 5;
    private List<GameObject> buttons;
    private string firstPlayer;
    private bool isGamePaused = false;

    

    // Start is called before the first frame update
    public void Start()
    {
        roundInsults = new List<Insult>();
        buttons = new List<GameObject>();
        StartNewGame();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void StartNewGame()
    {
        insults = FileManager.LoadInsults(jsonText);
        SelectFirstPlayer();
        SetNextState();
    }

    public void PauseGame()
    {
        pauseCanvas.gameObject.SetActive(true);
        isGamePaused = true;
        Time.timeScale = 0f;
        PauseUI();
    }

    public void ResumeGame()
    {        
        pauseCanvas.gameObject.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
        PauseUI();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SelectFirstPlayer()
    {
        bool startPlayer = Random.value > 0.5;
        if (startPlayer)
        {
            currentState = GameStates.playerInsult;
            firstPlayer = "player";
        }
        else
        {
            currentState = GameStates.enemyInsult;
            firstPlayer = "enemy";
        }
    }

    public void SetNextState()
    {
        switch (currentState)
        {
            case GameStates.enemyInsult:
                {
                    PrepareRoundInsults();
                    EnemyInsult();
                    currentState = GameStates.playerResponse;
                    SetNextState();
                    break;
                }

            case GameStates.enemyResponse:
                {
                    EnemyInsult();
                    currentState = GameStates.resolveRound;
                    SetNextState();
                    break;
                }
            case GameStates.playerInsult:
                {
                    PrepareRoundInsults();
                    PrepareUI();
                    break;
                }
            case GameStates.playerResponse:
                {
                    PrepareUI();
                    currentState = GameStates.resolveRound;
                    break;
                }
            case GameStates.resolveRound:
                {
                    ResolveRound();
                    break;
                }
            case GameStates.selectPlayer:
                {
                    PrepareRoundInsults();
                    break;
                }
        }
    }

    private void PrepareUI()
    {
        int index = -20;
        foreach(Insult insult in roundInsults)
        {            
            GameObject insultButton = Instantiate(buttonPrefab, insultUIPrefab.transform, true);
            insultButton.transform.SetParent(insultUIPrefab.transform);
            if (firstPlayer.Equals("player")){
                insultButton.GetComponentInChildren<TextMeshProUGUI>().text = insult.insultText;
            }
            else
            {
                insultButton.GetComponentInChildren<TextMeshProUGUI>().text = insult.counterText;
            }

            insultButton.GetComponent<RectTransform>().localPosition = new Vector3(0, transform.localPosition.y + index, 0);
            AddInsultListener(insultButton.GetComponent<Button>());
            buttons.Add(insultButton);
            index -= 40;
        }
    }

    private void DeleteUI()
    {
        foreach(GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }

    private void PauseUI()
    {
        foreach(GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = !isGamePaused;
        }
    }

    private void AddInsultListener(Button button)
    {
        button.onClick.AddListener(() => {
            DeleteUI();            
            PlayerInsult(button.GetComponentInChildren<TextMeshProUGUI>().text);
        });
    }

    private void PrepareRoundInsults()
    {
        roundInsults.Clear();
        while(roundInsults.Count < 5)
        {
            Insult insult = GetRandomInsult();
            if (!roundInsults.Contains(insult)){
                roundInsults.Add(insult);
            }
        }
    }

    private void EnemyInsult()
    {
        enemyInsult = roundInsults[Random.Range(0, roundInsults.Count-1)];
        if(currentState == GameStates.enemyInsult)
        {
            enemyBaloon.SetText(enemyInsult.insultText);
        }
        else
        {
            enemyBaloon.SetText(enemyInsult.counterText);
        }
        enemyBaloon.gameObject.SetActive(true);
    }

    private void PlayerInsult(string response)
    {
        DeleteUI();
        playerInsult = response;
        if (currentState.Equals(GameStates.playerInsult))
        {
            currentState = GameStates.enemyResponse;
            playerBaloon.SetText(playerInsult);
        }
        else
        {
            currentState = GameStates.resolveRound;
            playerBaloon.SetText(playerInsult);
        }
        playerBaloon.gameObject.SetActive(true);
        SetNextState();
    }

    private void ResolveRound()
    {
        playerBaloon.gameObject.SetActive(false);
        enemyBaloon.gameObject.SetActive(false);
        if(firstPlayer == "player")
        {
            if(playerInsult != enemyInsult.insultText)
            {
                // Player wins
                PlayerAttack();
                
            }
            else
            {
                // Enemy wins
                EnemyAttack();               
            }                       
        }
        else
        {
            if (enemyInsult.counterText != playerInsult)
            {
                // Enemy wins
                EnemyAttack();
            }
            else
            {
                // Player wins
                PlayerAttack();
            }           
        }
        CheckIfGameIsEnded();
        SetNextState();
    }

    private void PlayerAttack()
    {
        playerPrefab.GetComponent<Animator>().SetTrigger("PlayerAttack");
        enemyPrefab.GetComponent<Animator>().SetTrigger("EnemyHit");
        playerPrefab.GetComponent<AudioSource>().Play();
        enemyPrefab.GetComponent<AudioSource>().Play();
        enemyHealth--;
        enemyHealthBar.sprite = healthSprites[enemyHealth];
        firstPlayer = "player";
        currentState = GameStates.playerInsult;
    }

    private void EnemyAttack()
    {
        enemyPrefab.GetComponent<Animator>().SetTrigger("EnemyAttack");
        playerPrefab.GetComponent<Animator>().SetTrigger("PlayerHit");
        playerPrefab.GetComponent<AudioSource>().Play();
        enemyPrefab.GetComponent<AudioSource>().Play();
        playerHealth--;
        playerHealthBar.sprite = healthSprites[playerHealth];
        firstPlayer = "enemy";
        currentState = GameStates.enemyInsult;
    }

    private void CheckIfGameIsEnded()
    {
        if(playerHealth <= 0 || enemyHealth <= 0)
        {
            playerWin = playerHealth > 0;
            SceneManager.LoadScene("EndGame");
        }
    }

    private Insult GetRandomInsult()
    {
        int index = Random.Range(0, insults.insults.Length);
        return insults.insults.GetValue(index) as Insult;
    }

    public void FinishGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

   
}
