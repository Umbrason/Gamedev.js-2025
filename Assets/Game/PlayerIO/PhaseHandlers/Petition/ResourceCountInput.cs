using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCountInput : MonoBehaviour
{

    public int mMin;
    public int mMax;
    private int mValue;

    public int Min
    {
        get => mMin;
        set
        {
            mMin = Mathf.Min(value, Max);
            Value = Mathf.Max(Min, Value);
        }
    }
    public int Max
    {
        get => mMax;
        set
        {
            mMax = Mathf.Max(value, Min);
            Value = Mathf.Min(Max, Value);
        }
    }
    private int Value
    {
        get => mValue;
        set => mValue = Mathf.Clamp(value, Min, Max);
    }



    public event Action<int> OnChanged;
    [SerializeField] TMP_InputField inputField;
    void OnEnable()
    {
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(OnInputFieldChanged);
    }
    void OnDisable() => inputField.onValueChanged.RemoveListener(OnInputFieldChanged);

    void OnInputFieldChanged(string number)
    {
        Value = int.Parse(number);
        inputField.SetTextWithoutNotify(Value.ToString(CultureInfo.InvariantCulture));
    }

    public void Incr() => Value++;
    public void Decr() => Value--;
}
