using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject instructions;
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ExitIntro()
    {
        SceneManager.LoadScene(2);
    }

    public void Instruct()
    {
        instructions.SetActive(true);
    }

    public void Back()
    {
        instructions.SetActive(false);
    }

}
