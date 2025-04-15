using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    // Scene Names
    const string LOGIN = "Login";
    const string MAIN_MENU = "Main Menu";
    const string GAME_SCENE = "Game";

    public void Logout()
    {
        // do the logout
        // PlayerPrefs.DeleteKey("JWT");
        SceneManager.LoadScene(LOGIN);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void GoToGame()
    {
        // @Fabio how do we save the SP/MP state? Static StatsManager class? transfer it as bool here? idk
        // do we have seperate scenes for SP/MP?

        SceneManager.LoadScene(GAME_SCENE);
    }
}
