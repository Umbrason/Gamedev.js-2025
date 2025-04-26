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

        if(Game.ClientPlayerData.Role == PlayerRole.Balanced)
        {
            switch (Phase.WinnerRole)
            {
                case PlayerRole.Balanced: SoundAndMusicController.Instance.PlaySFX(SFXType._17_GoodWins_GoodVersion, Game.ClientID); break;
                case PlayerRole.Selfish: SoundAndMusicController.Instance.PlaySFX(SFXType._18_SusWins_GoodVersion, Game.ClientID); break;
            }
        }
        else if (Game.ClientPlayerData.Role == PlayerRole.Selfish)
        {
            switch (Phase.WinnerRole)
            {
                case PlayerRole.Balanced: SoundAndMusicController.Instance.PlaySFX(SFXType._45_GoodWins_SusVersion, Game.ClientID); break;
                case PlayerRole.Selfish: SoundAndMusicController.Instance.PlaySFX(SFXType._46_SusWins_SusVersion, Game.ClientID); break;
            }
        }

        Canvas.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneFader.Instance.FadeToScene("Main Menu");
        Destroy(GameNetworkManager.Instance.gameObject);
    }
}