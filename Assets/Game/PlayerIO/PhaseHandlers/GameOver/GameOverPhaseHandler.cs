using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPhaseHandler : GamePhaseHandler<GameOverPhase>
{
    [SerializeField] private GameObject BalancedFactionWin;
    [SerializeField] private GameObject SelfishFactionWin;
    [SerializeField] private GameObject Canvas;


    public override void OnPhaseEntered()
    {
        BalancedFactionWin.SetActive(Phase.WinnerRole == PlayerRole.Balanced);
        SelfishFactionWin.SetActive(Phase.WinnerRole == PlayerRole.Selfish);
        Canvas.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
