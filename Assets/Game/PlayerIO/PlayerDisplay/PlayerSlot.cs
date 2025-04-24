using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayerSlot : MonoBehaviour
{
    [Tooltip("Make sure to assign in correct order! (see PlayerFactions enum)")]
    [SerializeField] private AnimationClip[] factionAnim;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material GhostMaterial;
    [SerializeField] private Material DefaultMaterial;

    private PlayerFaction faction = PlayerFaction.None;
    public PlayerFaction ActiveFaction
    {
        get => faction; set
        {
            faction = value;
            UpdateAnim();
        }
    }

    private bool m_IsGhost;
    public bool IsGhost
    {
        get => m_IsGhost;
        set
        {
            m_IsGhost = value;
            spriteRenderer.material = value ? GhostMaterial : DefaultMaterial;
        }
    }

    PlayableGraph graph;
    AnimationPlayableOutput output;
    void Start()
    {
        graph = PlayableGraph.Create();
        output = AnimationPlayableOutput.Create(graph, "output", animator);
        for (int i = 1; i < Enum.GetValues(typeof(PlayerFaction)).Length; i++)
        {
            var faction = (PlayerFaction)i;
            playables[faction] = AnimationClipPlayable.Create(graph, factionAnim[i - 1]);
        }
        spriteRenderer.enabled = false;
    }

    readonly Dictionary<PlayerFaction, AnimationClipPlayable> playables = new();
    private void UpdateAnim()
    {
        if (faction == PlayerFaction.None)
        {
            spriteRenderer.enabled = false;
            graph.Stop();
            return;
        }
        spriteRenderer.enabled = true;
        output.SetSourcePlayable(playables[faction], 0);
        graph.Play();
    }
}
