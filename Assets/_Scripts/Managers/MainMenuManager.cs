using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private LoadScenes sceneLoader;

    [Header("Options Menu")]
    [SerializeField] private GameObject optionsGameObj;       //Parent that holds all option menu items
    [SerializeField] private GameObject optionsPanel;         // The background panel
    [SerializeField] private GameObject optionsMenuContent;   // The actual popup menu
    
    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        panelCanvasGroup = optionsPanel.GetComponent<CanvasGroup>();
        optionsGameObj.SetActive(false);
    }

    public void BtnPlay()
    {
        // show Singleplayer/Multiplayer options using DoTweens
        sceneLoader.GoToLobby();
    }

    public void BtnOptions()
    {
        optionsGameObj.SetActive(true);
        panelCanvasGroup.alpha = 0;
        optionsMenuContent.transform.localScale = Vector3.zero;

        // Fade in the background panel
        LeanTween.alphaCanvas(panelCanvasGroup, 1f, 0.4f).setEase(LeanTweenType.easeOutQuad);

        // Scale up the menu content (pop effect)
        LeanTween.scale(optionsMenuContent, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack);
    }

    public void BtnCloseOptions()
    {
        // Fade out the background panel
        LeanTween.alphaCanvas(panelCanvasGroup, 0f, 0.3f).setEase(LeanTweenType.easeInQuad);

        // Scale down the menu content
        LeanTween.scale(optionsMenuContent, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            optionsGameObj.SetActive(false); // Deactivate after animation
        });
    }

    public void BtnQuit()
    {
        Application.OpenURL("https://www.wikihow.com/Close-Tabs");  // funny easter egg
        // Application.Quit();  won't work in web
    }

    public void BtnSingleplayer()
    {
        sceneLoader.GoToGame();
    }
}
