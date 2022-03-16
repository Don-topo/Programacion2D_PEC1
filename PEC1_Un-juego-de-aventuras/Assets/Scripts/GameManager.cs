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

    private string currentState;
    private Insults insults;
    private List<Insult> roundInsults;
    private Insult playerInsult;
    private Insult enemyInsult;
    private int playerHealth = 5;
    private int enemyHealth = 5;
    private List<GameObject> buttons;
    private string firstPlayer;
    private bool isGamePaused = false;
    

    // Start is called before the first frame update
    private void Start()
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
        Debug.LogError("Select first player");
        SelectFirstPlayer();
        SetNextState();
    }

    public void PauseGame()
    {
        pauseCanvas.gameObject.SetActive(true);
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        pauseCanvas.gameObject.SetActive(false);
        isGamePaused = false;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SelectFirstPlayer()
    {
        int startPlayer = Random.Range(0, 1);
        if(startPlayer == 1)
        {
            currentState = GameStates.playerInsult;
            firstPlayer = "player";
        }
        else
        {
            currentState = GameStates.enemyInsult;
            firstPlayer = "enemy";
        }
        Debug.LogError("Next player " + currentState);
    }

    public void SetNextState()
    {
        switch (currentState)
        {
            case GameStates.enemyInsult:
                {
                    Debug.LogError("EnemyInsult");
                    PrepareRoundInsults();
                    EnemyInsult();
                    currentState = GameStates.playerResponse;
                    SetNextState();
                    break;
                }

            case GameStates.enemyResponse:
                {
                    Debug.LogError("EnemyResponse");
                    EnemyInsult();
                    currentState = GameStates.resolveRound;
                    SetNextState();
                    break;
                }
            case GameStates.playerInsult:
                {
                    Debug.LogError("PlayerInsult");
                    PrepareRoundInsults();
                    PrepareUI();
                    break;
                }
            case GameStates.playerResponse:
                {
                    Debug.LogError("PlayerResonse");
                    PrepareUI();
                    currentState = GameStates.resolveRound;
                    break;
                }
            case GameStates.resolveRound:
                {
                    Debug.LogError("ResolveRound");
                    ResolveRound();
                    break;
                }
            case GameStates.selectPlayer:
                {
                    Debug.LogError("SelectPlayer");
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
            insultButton.GetComponentInChildren<TextMeshProUGUI>().text = insult.insultText;
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
    }

    private void AddInsultListener(Button button)
    {
        button.onClick.AddListener(() => {
            DeleteUI();
            playerInsult = roundInsults[Random.Range(0, roundInsults.Count - 1)];
            PlayerInsult(button);
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
    }

    private void PlayerInsult(Button button)
    {
        DeleteUI();
        // TODO Catch insult
        if (currentState.Equals(GameStates.playerInsult))
        {
            currentState = GameStates.enemyResponse;
        }
        else
        {
            currentState = GameStates.resolveRound;
        }
        SetNextState();
    }

    // TODO Refactor this a lot of code is duplicated and is not efficient
    private void ResolveRound()
    {
        if(firstPlayer == "player")
        {
            if(playerInsult.counterText != enemyInsult.insultText)
            {
                // Player wins
                Debug.LogError("Player wins");
                enemyHealth--;
                enemyHealthBar.sprite = healthSprites[enemyHealth];
                playerPrefab.GetComponent<Animator>().SetTrigger("PlayerAttack");
                enemyPrefab.GetComponent<Animator>().SetTrigger("EnemyHit");
                // TODO play sounds here
                currentState = GameStates.playerInsult;
            }
            else
            {
                // Enemy parry
                Debug.LogError("Enemy wins");
                currentState = GameStates.enemyInsult;
                enemyPrefab.GetComponent<Animator>().SetTrigger("EnemyAttack");
                // TODO play sounds here
                firstPlayer = "enemy";
            }
        }
        else
        {
            if(enemyInsult.counterText != playerInsult.insultText)
            {
                // Enemy wins
                Debug.LogError("Enemy wins");
                playerHealth--;
                playerHealthBar.sprite = healthSprites[playerHealth];
                currentState = GameStates.enemyInsult;
                enemyPrefab.GetComponent<Animator>().SetTrigger("EnemyAttack");
                playerPrefab.GetComponent<Animator>().SetTrigger("PlayerHit");
                // TODO play sounds here
            }
            else
            {
                // Player parry
                Debug.LogError("Player wins");
                currentState = GameStates.playerInsult;
                enemyPrefab.GetComponent<Animator>().SetTrigger("EnemyAttack");
                playerPrefab.GetComponent<Animator>().SetTrigger("PlayerAttack");
                // TODO play sounds here
                firstPlayer = "player";
            }
        }
        CheckIfGameIsEnded();
        SetNextState();
    }

    private void CheckIfGameIsEnded()
    {
        if(playerHealth <= 0 || enemyHealth <= 0)
        {
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
