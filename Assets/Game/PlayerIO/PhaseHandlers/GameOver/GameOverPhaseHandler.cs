using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPhaseHandler : GamePhaseHandler<GameOverPhase>
{
    [SerializeField] private GameObject BalancedFactionWin;
    [SerializeField] private GameObject SelfishFactionWin;


    public override void OnPhaseEntered()
    {
        BalancedFactionWin.SetActive(Phase.WinnerRole == PlayerRole.Balanced);
        SelfishFactionWin.SetActive(Phase.WinnerRole == PlayerRole.Selfish);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
