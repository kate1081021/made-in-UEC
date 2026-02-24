using UnityEngine;
using UnityEngine.UI;

namespace plugmatch
{
    public class PM_FanAnimator : MonoBehaviour
    {
        [Header("扇風機の画像(Image)コンポーネント")]
        public Image fanImage;

        [Header("パラパラ漫画にする2枚の画像")]
        public Sprite fanFrame1;
        public Sprite fanFrame2;

        [Header("切り替わる速さ（秒）")]
        public float animationSpeed = 0.05f; // 数字を小さくするほど高速回転します

        private float timer = 0f;
        private bool isFrame1 = true;

        void Update()
        {
            // 時間をカウント（Time.deltaTime はゲーム全体の倍速設定の影響を自動で受けます）
            timer += Time.deltaTime;

            // 指定した時間が経過したら画像を切り替える
            if (timer >= animationSpeed)
            {
                timer = 0f;
                isFrame1 = !isFrame1; // trueとfalseを反転させる

                // isFrame1がtrueならFrame1を、falseならFrame2を表示する
                if (isFrame1)
                {
                    fanImage.sprite = fanFrame1;
                }
                else
                {
                    fanImage.sprite = fanFrame2;
                }
            }
        }
    }
}