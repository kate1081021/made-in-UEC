using System;
using UnityEngine;
using UnityEngine.UIElements;


public class LF_background : MonoBehaviour
{
    [Header("画像リスト")]
    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    int currentIndex = 0;

    public void setBackground(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentIndex = UnityEngine.Random.Range(0, 9);
        spriteRenderer.sprite = sprites[currentIndex];
    }
}
