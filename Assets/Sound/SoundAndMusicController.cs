public class SoundAndMusicController : Singleton<SoundAndMusicController>
{
    private string soundbankName = "MainSoundbank";

    private void Start()
    {
        dontDestroyOnLoad = true;
        LoadBank();

        PostEvent2D(WwiseSoundEvents.BuildVotingTimeEnd);
    }

    private void LoadBank()
    {
        AkBankManager.LoadBank(soundbankName, true, true);
    }

    public void PostEvent2D(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return;
        }

        AkUnitySoundEngine.PostEvent(eventName, gameObject);
    }
}