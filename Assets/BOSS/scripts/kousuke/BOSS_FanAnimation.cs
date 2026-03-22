using UnityEngine;

namespace BOSS
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BOSS_FanAnimation : MonoBehaviour
    {
        [Header("アニメーション設定")]
        [Tooltip("扇風機の画像1")]
        public Sprite fanSprite1;
        [Tooltip("扇風機の画像2")]
        public Sprite fanSprite2;
        [Tooltip("画像を切り替える間隔（秒）")]
        public float animationInterval = 0.1f;

        private SpriteRenderer spriteRenderer;
        private float timer = 0f;
        private bool useSprite1 = true; // どちらの画像を使うかのフラグ

        // ★修正：Start() を禁止事項に沿って OnEnable() に変更！
        // （扇風機が画面に出現するたびに、ここが自動的に実行されます）
        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRendererが見つかりません。");
                enabled = false;
                return;
            }
            
            // 出現するたびに状態をリセットして、画像1からアニメーションを再開する
            timer = 0f;
            useSprite1 = true;
            UpdateSprite();
        }

        void Update()
        {
            timer += Time.deltaTime;

            // 設定した間隔を超えたら画像を切り替える
            if (timer >= animationInterval)
            {
                useSprite1 = !useSprite1; // フラグを反転
                UpdateSprite();
                timer = 0f; // タイマーリセット
            }
        }

        private void UpdateSprite()
        {
            if (useSprite1 && fanSprite1 != null)
            {
                spriteRenderer.sprite = fanSprite1;
            }
            else if (!useSprite1 && fanSprite2 != null)
            {
                spriteRenderer.sprite = fanSprite2;
            }
        }
    }
}
