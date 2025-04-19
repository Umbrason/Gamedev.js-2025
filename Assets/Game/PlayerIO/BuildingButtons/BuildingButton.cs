using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public bool Active
    {
        get;
        set;
    }
    private Building m_Building;
    public Building Building
    {
        get => m_Building; set
        {
            m_Building = value;
            image.sprite = buildingIcons[value];
        }
    }

    [SerializeField] private BuildingSpriteLib buildingIcons;
    [SerializeField] private Image image;

    public event Action<Building> OnStartDrag;
    public event Action<Vector3> OnUpdateDrag;
    public event Action OnStopDrag;
    public event Action<HexPosition> OnDrop;


    private bool dragging;
    public void OnDrag(PointerEventData eventData)
    {
        if (!Active) return;
        if (!dragging && !EventSystem.current.IsPointerOverGameObject())
        {
            dragging = true;
            OnStartDrag?.Invoke(m_Building);
        }
        var worldPos = Camera.main.ScreenPointToXZIntersection(eventData.position);
        if (dragging) OnUpdateDrag?.Invoke(worldPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            var worldPos = Camera.main.ScreenPointToXZIntersection(eventData.position);
            var pos = HexOrientation.Active * worldPos;
            OnDrop?.Invoke(pos);
        }
        OnStopDrag?.Invoke();
        dragging = false;
    }
}