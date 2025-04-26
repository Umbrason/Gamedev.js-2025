using UnityEngine;

public class RandomAnimationOffset : MonoBehaviour
{
    Cached<Animator> cached_anim;
    Animator Anim => cached_anim[this];
    void OnEnable()
    {
        var pos = HexOrientation.Active * transform.position;
        var noise = Mathf.PerlinNoise(pos.Q * -7.621341f, pos.R * 3.419764f);
        var info = Anim.GetCurrentAnimatorClipInfo(0)[0];
        var skipTime = noise * info.clip.averageDuration;
        //Debug.Log(skipTime);
        Anim.Update(skipTime);
    }
}
