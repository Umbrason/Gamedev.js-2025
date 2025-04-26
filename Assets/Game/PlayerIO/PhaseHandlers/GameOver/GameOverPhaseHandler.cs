using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPhaseHandler : GamePhaseHandler<GameOverPhase>
{
    [SerializeField] private GameObject gameOverUI;


    public override void OnPhaseEntered()
    {
        gameOverUI.SetActive(true);
        // display winner team, maybe some stats later too
    }

    public override void OnPhaseExited()
    {
        gameOverUI.SetActive(true);
    }


    public void ReturnToMainMenu()
    {
        // disconnect from server?
        SceneManager.LoadScene("Main Menu");
    }
}
