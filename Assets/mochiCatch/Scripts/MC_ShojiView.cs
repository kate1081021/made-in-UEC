using UnityEngine;

namespace catchMochi
{
    public class MC_ShojiView : MonoBehaviour
    {
        // 座標管理
        private RectTransform rect;

        // アニメーター管理
        private Animator animator;

        // 開始時
        public void Start()
        {
            // コンポーネント取得
            rect = GetComponent<RectTransform>();
            animator = GetComponent<Animator>();

            rect.position = new Vector2(262,90.15705f);  // 初期位置に配置
            
        }

        // 障子を揺らす
        public void Shaking(bool value)
        {
            animator.SetBool("isShaking", value);
        }

        // 障子を開く
        public void Open(bool value)
        {
            animator.SetBool("open", value);
        }

        // 障子を少しだけ開く
        public void OpenLittle()
        {
            animator.SetTrigger("open_little");
        }

    }
}
