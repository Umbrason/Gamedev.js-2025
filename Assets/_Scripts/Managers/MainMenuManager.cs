using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private LoadScenes sceneLoader;


    public void BtnPlay()
    {
        // show Singleplayer/Multiplayer options using DoTweens

    }

    public void BtnOptions()
    {
        // show options menu using DoTweens

    }

    public void BtnQuit()
    {
        Application.Quit();
    }

    public void BtnSingleplayer()
    {
        sceneLoader.GoToGame();
    }
}
