using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Click Sound")]
    [SerializeField] private SFXType soundEffect = SFXType._03_GeneralButtonClicks;

    [Header("Optional: Hover Sound")]
    [SerializeField] private bool playHoverSound = false;
    [SerializeField] private SFXType hoverSoundEffect = SFXType._41_MainMenuNavHover;

    private bool hasHovered = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        SoundAndMusicController.Instance.PlaySFX(soundEffect, PlayerID.None);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || hasHovered) return;

        SoundAndMusicController.Instance.PlaySFX(hoverSoundEffect, PlayerID.None);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasHovered = false;
    }
}