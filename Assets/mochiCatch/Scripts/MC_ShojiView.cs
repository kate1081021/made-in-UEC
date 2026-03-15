using UnityEngine;

namespace catchMochi
{
    public class MC_ShojiView : MonoBehaviour
    {
        // 座標管理
        public RectTransform rect;

        // アニメーター管理
        public Animator animator;

        // 開始時
        void Start()
        {
            // コンポーネント取得
            rect = GetComponent<RectTransform>();
            animator = GetComponent<Animator>();

            rect.position = new Vector2(262,90.15705f);  // 初期位置に配置
            
        }
    }
}
