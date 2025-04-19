using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceItem : MonoBehaviour
{

    [SerializeField] private Resource resourceType;
    [SerializeField] private int amount;
    [SerializeField] private TextMeshProUGUI text;

    public void Init(Resource type, int resourceAmount = 0)
    {
        resourceType = type;
        amount = resourceAmount;
    }

    public void UpdateAmount(int resourceAmount, bool updateUI = true)
    {
        amount = resourceAmount;
        if(updateUI) UpdateUI();
    }

    public void UpdateUI()
    {
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = amount.ToString();
    }
}
