using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject dynamicUI;
    public Text gameOverScore;
    public GameObject timeOverUI;
    public Text timeOverScore;
    public GameObject player;
    public AudioSource moneybag;
    public AudioSource jailcell;

    private bool flag = false;
    
    public static ScoreManager Instance 
    { 
        get;
        private set; 
    }

    private int score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1;
    }

    /*private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Application is focused");
        }
        else
        {
            Debug.Log("Application is unfocused");
        }
    }*/

    public void AddScore(int amount)
    {
        moneybag.Play();
        score += amount;

        UpdateUIText();

        //Debug.Log("Score: " + score);
    }

    private void UpdateUIText()
    {
        Text scoreText = GameObject.Find("Score").GetComponent<Text>();
        scoreText.text = "Payout:\n$" + score;
    }

    public void GameOver()
    {
        if(flag == false)
        {
            jailcell.Play();
            Cursor.visible = true;
            player.GetComponent<InputManager>().enabled = false;
            dynamicUI.SetActive(false);
            gameOverUI.SetActive(true);
            gameOverScore.text = "Your payout:\n$" + score;
            flag = true;
            Time.timeScale = 0;
        }    
    }

    public void TimeOver()
    {
        if(flag == false)
        {
            Cursor.visible = true;
            player.GetComponent<InputManager>().enabled = false;
            dynamicUI.SetActive(false);
            timeOverUI.SetActive(true);
            timeOverScore.text = "Your payout:\n$" + score;
            flag = true;
            Time.timeScale = 0;
        }
    }

    public float returnScore()
    {
        return score;
    }
}
