using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    Cached<SpriteRenderer> cached_sr;
    SpriteRenderer SR => cached_sr[this];
    [SerializeField] private Sprite[] Sprites;
    void OnEnable()
    {
        var pos = HexOrientation.Active * transform.position;
        var noise = Mathf.PerlinNoise(pos.Q * -7.621341f, pos.R * 3.419764f);
        SR.sprite = Sprites[Mathf.Clamp(Mathf.FloorToInt(noise * Sprites.Length), 0, Sprites.Length)];
    }
}
