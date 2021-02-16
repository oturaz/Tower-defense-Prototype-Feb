using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public enum menuStates
    {
        MainMenu,
        EditorLevelSelect,
        EditorLevelEdit,
        PlayLevelSelect,
        PlayLevel,
        PlayLevelPause
    }
    public menuStates menuState;

    public menuStates MenuState
    {
        set
        {
            menuState = value;
            MenuStateChanger();
        }
    }

    public List<GameObject> elementsToDisable = new List<GameObject>();
    public List<GameObject> elementsToEnable = new List<GameObject>();
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitButton()
    {
        Debug.Log("Game was quit");
        Application.Quit();
    }

    public void BackButton(GameObject[] _elementsToDisable)
    {
        
            
    }

    public void MenuStateChanger()
    {
        switch (menuState)
        {
            case menuStates.EditorLevelSelect:
                SceneManager.LoadScene("LevelEditor");
                break;
            case menuStates.MainMenu:
                SceneManager.LoadScene("Menu");
                break;
        }
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Level");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LevelEditorButton()
    {
        SceneManager.LoadScene("LevelEditor");
    }
}
