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
            Value = newValue;
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
            Value = newValue;
        }
    }

    private int mValue = -1;
    public int Value
    {
        get => mValue;
        set
        {
            var newValue = Mathf.Clamp(value, Min, Max);
            Increment.interactable = newValue < Max;
            Decrement.interactable = newValue > Min;
            inputField.SetTextWithoutNotify(newValue.ToString(CultureInfo.InvariantCulture));
            if (newValue == mValue) return;
            mValue = newValue;
            OnChanged?.Invoke(mValue);
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
    [SerializeField] Button Increment;
    [SerializeField] Button Decrement;

    void OnEnable()
    {
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.onSubmit.AddListener(OnInputFieldChanged);
        inputField.SetTextWithoutNotify(Value.ToString(CultureInfo.InvariantCulture));
        Increment.onClick.AddListener(Incr);
        Decrement.onClick.AddListener(Decr);
    }
    void OnDisable()
    {
        inputField.onSubmit.RemoveListener(OnInputFieldChanged);
        Increment.onClick.RemoveListener(Incr);
        Decrement.onClick.RemoveListener(Decr);
    }

    void OnInputFieldChanged(string number)
    {
        Value = int.Parse(number);
    }

    public void Incr() => Value++;
    public void Decr() => Value--;
}
