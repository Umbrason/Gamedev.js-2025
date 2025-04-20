using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceButton : MonoBehaviour
{
    [SerializeField] PlayerIDSpriteLib playerIcons;
    [SerializeField] ResourceSpriteLib resourceIcons;
    public PlayerID TargetPlayer { get; set; }
    public BuildingPetition ActivePetition { get; set; }
    public event Action<Resource, int> OnResourceCounterChanged;

    [SerializeField] ResourceCountInput ResourceCountInputTemplate;
    [SerializeField] Image TinyResourceIconTemplate;


}
