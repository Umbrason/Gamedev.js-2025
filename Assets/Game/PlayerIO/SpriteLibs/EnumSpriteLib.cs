using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnumSpriteLib<T> : ScriptableObject, ISerializationCallbackReceiver where T : Enum
{
    [SerializeField] private T[] keys = new T[0];
    [SerializeField] private Sprite[] values = new Sprite[0];
    public IReadOnlyDictionary<T, Sprite> Sprites;
    public Sprite this[T key] => Sprites[key];

    public void OnAfterDeserialize()
    {
        var dict = new Dictionary<T, Sprite>();
        for (int i = 0; i < keys.Length; i++)
            dict.Add(keys[i], values[i]);
        Sprites = dict;
    }

    public void OnBeforeSerialize()
    {
        keys = Sprites?.Keys?.ToArray();
        values = Sprites?.Values?.ToArray();
    }
}