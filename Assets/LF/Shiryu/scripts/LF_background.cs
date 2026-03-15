using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace LoupeFire{
    public class LF_background : MiniGameBase
    {
        [Header("画像リスト")]
        public Sprite[] sprites;
        private SpriteRenderer spriteRenderer;
        int currentIndex = 0;
        public override void OnGameStart()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentIndex = UnityEngine.Random.Range(0, 8);
            spriteRenderer.sprite = sprites[currentIndex];
        }
    }
}