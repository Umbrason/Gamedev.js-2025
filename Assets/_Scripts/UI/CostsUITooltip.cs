using System.Collections;
using TMPro;
using UnityEngine;

public class CostsUITooltip : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;


    // 
    public void Setup(string t)
    {
        text.text = t;
    }
}
