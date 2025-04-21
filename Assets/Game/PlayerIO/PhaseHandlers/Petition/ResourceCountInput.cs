using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCountInput : MonoBehaviour
{
    private int mMin;
    public int Min
    {
        get => mMin;
        set
        {
            mMin = Mathf.Min(value, Max);
            var newValue = Mathf.Max(Min, Value);
            if (mValue == newValue) return;
            Value = newValue;
            OnChanged?.Invoke(value);
        }
    }

    private int mMax;
    public int Max
    {
        get => mMax;
        set
        {
            mMax = Mathf.Max(value, Min);
            var newValue = Mathf.Min(Max, Value);
            if (mValue == newValue) return;
            Value = newValue;
            OnChanged?.Invoke(value);
        }
    }

    private int mValue;
    public int Value
    {
        get => mValue;
        set
        {

            var newValue = Mathf.Clamp(value, Min, Max);
            if (mValue == newValue) return;
            OnChanged?.Invoke(value);
        }
    }

    private Resource resource;
    public Resource Resource
    {
        get => resource;
        set
        {
            if (value == resource) return;
            resourceIcon.sprite = ResourceIcons[value];
            resource = value;
        }
    }

    public event Action<int> OnChanged;

    [SerializeField] ResourceSpriteLib ResourceIcons;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Image resourceIcon;
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
