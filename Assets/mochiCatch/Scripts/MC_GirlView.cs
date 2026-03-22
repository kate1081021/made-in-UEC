using UnityEngine;
using UnityEngine.UI;

namespace catchMochi
{
    public class MC_GirlView : MonoBehaviour
    {
        // アニメーション
        public Sprite[] girlAnimation;
        public Sprite girlWin;
        public Sprite girlSurprised;
        private Image image;
        private RectTransform rect;
        private Vector2 base_pos;

        public void Start()
        {
            image = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
            base_pos = rect.anchoredPosition;  // 最初の位置を取得
        }
        

        // アニメーション
        public void Animation(string status)
        {
            if (status == "normal")
            {
                image.sprite = girlAnimation[0];
                rect.anchoredPosition = base_pos;
            }
            else if (status == "catch")
            {
                image.sprite = girlAnimation[1];
            }
            else if (status == "draw")
            {
                image.sprite = girlAnimation[2];
            }
            else if (status == "eat")
            {
                image.sprite = girlAnimation[3];
            }
        }

        // 勝利時
        public void Win()
        {
            image.sprite = girlWin;
        }
        // 敗北時
        public void Surprised()
        {
            image.sprite = girlSurprised;
        }

    }
}
