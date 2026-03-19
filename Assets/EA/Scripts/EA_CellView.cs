using UnityEngine;

namespace EA
{
    public class EA_CellView : MonoBehaviour
    {
        /// プロパティ ///
        private SpriteRenderer spriteRenderer;
        public Sprite Head;  // セルのスプライト(表)
        public Sprite Tail;  // セルのスプライト(裏)

        // ゲーム開始時に呼ばれる
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();  // スプライトコンポーネントを抽出
        }

        // 表裏を設定する
        public void Flip(bool side)
        {
            spriteRenderer.sprite = side ? Head : Tail;
        }

    }
}
