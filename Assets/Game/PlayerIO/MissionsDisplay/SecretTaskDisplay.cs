using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecretTaskDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text Description;
    [SerializeField] private Image Checkbox;
    [SerializeField] private GameInstance Game;
    private SecretTask m_Task;
    public SecretTask SecretTask
    {
        get => m_Task;
        set
        {
            m_Task = value;
            Refresh();
        }
    }

    void Refresh()
    {
        if (SecretTask == null) return;
        Description.text = SecretTask.Description;
        Checkbox.enabled = SecretTask.Evaluate(Game.ClientPlayerData);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => Game.ClientPlayerData != null);
        Game.ClientPlayerData.OnIslandChanged += _ => Refresh();
        Game.ClientPlayerData.OnResourceChanged += (_, _) => Refresh();
    }

    void OnDestroy()
    {
        Game.ClientPlayerData.OnIslandChanged -= _ => Refresh();
        Game.ClientPlayerData.OnResourceChanged -= (_, _) => Refresh();
    }
}