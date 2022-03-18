using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public TextMeshProUGUI winnerText;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public AudioClip playerWinClip;
    public AudioClip enemyWinClip;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.playerWin)
        {
            winnerText.SetText("Player Wins!!!");
            GameObject enemy = Instantiate(enemyPrefab, transform, true);
            enemy.transform.localScale = new Vector3(12, 12, 12);
            enemy.transform.localPosition += new Vector3(0,-1,0);
            enemy.GetComponent<Animator>().SetTrigger("EnemyDeath");
            gameObject.GetComponent<AudioSource>().clip = playerWinClip;
            gameObject.GetComponent<AudioSource>().Play();
        }
        else
        {
            winnerText.SetText("Enemy Wins!!!");
            GameObject player = Instantiate(playerPrefab, transform, true);
            player.transform.localScale = new Vector3(8, 8, 8);
            player.GetComponent<Animator>().SetTrigger("PlayerDeath");
            gameObject.GetComponent<AudioSource>().clip = enemyWinClip;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    public void FinishGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
