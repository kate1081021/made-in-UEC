using UnityEngine;
using UnityEngine.UI;

namespace catchMochi
{
    public class MC_MotherView : MonoBehaviour
    {
        // スプライト
        private Image image;
        public Sprite motherNormal;
        public Sprite motherAngry;

        // 開始時
        void Start()
        {
            // コンポーネントを抽出
            image = GetComponent<Image>();

        }

        // お母さんを有効/無効化する
        public void GetEnabled(bool value)
        {
            image.enabled = value;
        }

        // お母さん正常
        public void GetNormal()
        {
            image.sprite = motherNormal;
        }

        // お母さん激怒
        public void GetAngry()
        {
            image.sprite = motherAngry;
        }
        
    }
}
