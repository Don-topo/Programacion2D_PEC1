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

    private string currentState;
    private Insults insults;
    private List<Insult> roundInsults;
    private Insult playerInsult;
    private Insult enemyInsult;
    private int playerLife = 5;
    private int enemyLife = 5;
    private List<GameObject> buttons;
    
    

    // Start is called before the first frame update
    private void Start()
    {
        roundInsults = new List<Insult>();
        AddListener(newGameButton);       
        exitGameButton.onClick.AddListener(ExitGame);
        insults = FileManager.LoadInsults(jsonText);
        SelectFirstPlayer();
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
        }
        else
        {
            currentState = GameStates.enemyInsult;
        }
        SetNextState();
    }

    public void SetNextState()
    {
        switch (currentState)
        {
            case GameStates.enemyInsult:
                {
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
                    PrepareUI();
                    currentState = GameStates.enemyResponse;
                    SetNextState();
                    break;
                }
            case GameStates.playerResponse:
                {
                    PrepareUI();
                    currentState = GameStates.resolveRound;
                    SetNextState();
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
            SetNextState(); 
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
        enemyInsult = roundInsults[Random.Range(0, roundInsults.Count)];      
    }

    private void PlayerInsult(Button button)
    {
        // TODO delete Buttons UI
        // TODO Catch insult
        // TODO NextState
    }

    private void ResolveRound()
    {
        // TODO who win?
        if(playerInsult.counterText != enemyInsult.insultText)
        {
            // player win
        } else
        {
            // enemy win
        }        
        // TODO set next firstPlayer
        // TODO is game ended? => load scene
        // TODO nextState
    }

    private Insult GetRandomInsult()
    {
        int index = Random.Range(0, insults.insults.Length);
        return insults.insults.GetValue(index) as Insult;
    }

   
}
