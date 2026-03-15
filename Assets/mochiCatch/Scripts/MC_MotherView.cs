using UnityEngine;
using UnityEngine.UI;

namespace catchMochi
{
    public class MC_MotherView : MonoBehaviour
    {
        // スプライト
        public Image image;
        public Sprite motherAngry;

        // 開始時
        void Start()
        {
            // コンポーネントを抽出
            image = GetComponent<Image>();

        }

        // お母さん激怒
        public void GetAngry()
        {
            image.sprite = motherAngry;
        }
        
    }
}
