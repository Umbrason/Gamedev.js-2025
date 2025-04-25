using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    // Scene Names
    const string LOBBY = "Lobby";
    const string MAIN_MENU = "Main Menu";
    const string GAME_SCENE = "PlayerGame";

    public void GoToLobby()
    {
        // do the logout
        // PlayerPrefs.DeleteKey("JWT");
        SceneFader.Instance.FadeToScene(LOBBY); //Fades to black then goes to scene.
    }

    public void GoToMainMenu()
    {
        SceneFader.Instance.FadeToScene(MAIN_MENU);
    }

    public void GoToGame()
    {
        // @Fabio how do we save the SP/MP state? Static StatsManager class? transfer it as bool here? idk
        // do we have seperate scenes for SP/MP?

        SceneFader.Instance.FadeToScene(GAME_SCENE);
    }
}
