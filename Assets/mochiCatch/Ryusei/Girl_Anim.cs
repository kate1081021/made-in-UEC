using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace catchMochi
{
    public class Girl_Anim : MonoBehaviour
    {
        // アニメーション
        public List<Sprite> girl_anim;
        public GameObject girl;
        private mochiCatch girl_eat;
        private Image girl_sprite;
        private RectTransform rect;
        private float ratio;
        private UnityEngine.Vector2 base_pos;

        void Start()
        {   
            girl_eat = girl.GetComponent<mochiCatch>();
            girl_sprite = girl.GetComponent<Image>();
            rect = girl.GetComponent<RectTransform>();
            ratio = rect.rect.width / 2080;  // 元の画像とImageのサイズ比を計算
            base_pos = rect.anchoredPosition;  // 最初の位置を取得
        }

        // Update is called once per frame
        void Update()
        {
            if (girl_eat.status == "normal")
            {
                girl_sprite.sprite = girl_anim[0];
                rect.anchoredPosition = base_pos;
            }
            else if (girl_eat.status == "catch")
            {
                girl_sprite.sprite = girl_anim[1];
            }
            else if (girl_eat.status == "draw")
            {
                girl_sprite.sprite = girl_anim[2];
            }
            else if (girl_eat.status == "eat")
            {
                girl_sprite.sprite = girl_anim[3];
            }
        }
    }
}