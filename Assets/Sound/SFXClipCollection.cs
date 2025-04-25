using UnityEngine;
using static Unity.Collections.Unicode;

[CreateAssetMenu(fileName = "SFXClips", menuName = "Audio/SFXClipCollection")]
public class SFXClipCollection : ScriptableObject
{
    //public AudioClip sharedGoalProgress;

    public AudioClip generalButtonClick;
    public AudioClip popupOpens;

    public AudioClip bumbiJoins;
    public AudioClip bumbiLeaves;
    public AudioClip lykiJoins;
    public AudioClip lykiLeaves;
    public AudioClip pigynJoins;
    public AudioClip pigynLeaves;
    public AudioClip gumbiJoins;
    public AudioClip gumbiLeaves;
    public AudioClip seltasJoins;
    public AudioClip seltasLeaves;
    public AudioClip pomPomJoins;
    public AudioClip pomPomLeaves;

    public AudioClip goodSideWins;
    public AudioClip susSideWins;

    public AudioClip roundStart;
    public AudioClip roundEnd;

    public AudioClip buildVotingTimeStart;
    public AudioClip buildVotingTimeEnd;
    public AudioClip susVotingTimeStart;
    public AudioClip susVotingTimeEnd;

    public AudioClip timerTicking;

    //public AudioClip creaturesWalking;

    public AudioClip lookAtBumbiIsland;
    public AudioClip lookAtLykiIsland;
    public AudioClip lookAtPigynIsland;
    public AudioClip lookAtGumbiIsland;
    public AudioClip lookAtSeltasIsland;
    public AudioClip lookAtPomPomIsland;

    public AudioClip dropBuilding;
    //public AudioClip openingDropdown;
    //PLEDGE PHASE NUMBER PICKER
    public AudioClip numberTicker;
    //FOR BUILD VOTE PHASE
    public AudioClip confirmButton;
    //ACCUSE
    public AudioClip voteSusButton;

    //VOTING BUILDING PHASE
    public AudioClip nextIsland;
    //public AudioClip pauseButton;

    //WHEN CLICKING A BUTTON
    public AudioClip mainMenuNavButtons;
    //WHEN HOVERING ABOVE A BUTTON

    public AudioClip MainMenuNavHover;
    public AudioClip MainMenuGameStart;
    public AudioClip PopUpCloses;

    public AudioClip TimerTicking_SusVersion;
    public AudioClip GoodWins_SusVersion;
    public AudioClip SusWins_SusVersion;
    public AudioClip RoundStart_SusVersion;
    public AudioClip RoundEnd_SusVersion;
    public AudioClip SusVote_start_SusVersion;
    public AudioClip SusVoteEnd_SusVersion;
    public AudioClip BuildVoteStart_SusVersion;
    public AudioClip BuildVoteEnd_SusVersion;
}