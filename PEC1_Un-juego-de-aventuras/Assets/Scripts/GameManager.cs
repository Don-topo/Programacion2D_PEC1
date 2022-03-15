using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button newGameButton;
    public Button exitGameButton;
    public TextAsset jsonText;
    public GameObject buttonPrefab;
    public GameObject insultUIPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    private string currentState;
    private Insults insults;
    private List<Insult> roundInsults;
    private Insult playerInsult;
    private Insult enemyInsult;
    private int playerHealth = 5;
    private int enemyHealth = 5;
    private List<GameObject> buttons;
    private string firstPlayer;
    
    

    // Start is called before the first frame update
    private void Start()
    {
        roundInsults = new List<Insult>();
        buttons = new List<GameObject>();
        AddListener(newGameButton);       
        exitGameButton.onClick.AddListener(ExitGame);
        insults = FileManager.LoadInsults(jsonText);
        Debug.LogError("Select first player");
        SelectFirstPlayer();
        SetNextState();
      
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

    public void SelectFirstPlayer()
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
        int index = 4;
        foreach(Insult insult in roundInsults)
        {            
            GameObject insultButton = Instantiate(buttonPrefab, insultUIPrefab.transform, true);
            insultButton.transform.SetParent(insultUIPrefab.transform);
            insultButton.GetComponentInChildren<TextMeshProUGUI>().text = insult.insultText;
            insultButton.GetComponent<RectTransform>().localPosition = new Vector3(0, transform.localPosition.y + index, 0);
            AddInsultListener(insultButton.GetComponent<Button>());
            buttons.Add(insultButton);
            index += 60;
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

    private void ResolveRound()
    {
        // TODO who win?
        if(firstPlayer == "player")
        {
            if(playerInsult.counterText != enemyInsult.insultText)
            {
                // Player wins
                Debug.LogError("Player wins");
                enemyHealth--;
                currentState = GameStates.playerInsult;
            }
            else
            {
                // Enemy parry
                Debug.LogError("Enemy wins");
                currentState = GameStates.enemyInsult;
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
                currentState = GameStates.enemyInsult;
            }
            else
            {
                // Player parry
                Debug.LogError("Player wins");
                currentState = GameStates.playerInsult;
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

   
}
