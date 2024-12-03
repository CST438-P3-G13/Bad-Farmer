using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    
    public void Pause() {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue() {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void GoToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
