using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourcesQuantities
{
    [SerializeField] private Item[] items;

    private Dictionary<Resource, int> _itemsDict = null;
    public Dictionary<Resource, int> Items
    {
        get
        {
            if (_itemsDict == null)
            {
                _itemsDict = new();

                foreach (var item in items)
                {
                    _itemsDict[item.res] = _itemsDict.GetValueOrDefault(item.res) + item.quantity;
                }
            }
            return _itemsDict;
        }
    }

    [System.Serializable] public class Item
    {
        public Resource res;
        public int quantity;
    }
}
