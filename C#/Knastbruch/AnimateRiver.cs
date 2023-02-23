using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRiver : MonoBehaviour
{
    public List<Sprite> sprites;
    public float FramesPerSecond = 30;
    public float frameLimiter = 0.15f;

    void Update()
    {
        int frame = (int)(Time.time * FramesPerSecond * frameLimiter);
        
        frame = frame % sprites.Count;
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites[frame];
    }
}
