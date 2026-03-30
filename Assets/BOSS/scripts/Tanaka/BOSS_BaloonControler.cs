using UnityEngine;

namespace BOSS
{
    public class BOSS_BaloonControler : MonoBehaviour
    {
        [Header("はなちょうちんの設定")]
        [SerializeField] public float BOSS_initialScale = 0.1f;
        [SerializeField] public float BOSS_maxScale = 5.0f;
        [SerializeField] public float BOSS_growSpeed = 1.2f;

        [Header("出現位置の設定（ジェネレーターからのズレ）")]
        [SerializeField] public Vector3 leftPositionOffset = new Vector3(-1.0f, 0f, 0f); // 左に出る時の位置
        [SerializeField] public Vector3 rightPositionOffset = new Vector3(1.0f, 0f, 0f); // 右に出る時の位置

        [HideInInspector] public bool BOSS_isFlipped = false; 

        private float BOSS_currentScale;
        
        // 画像パーツを操作するための変数
        private SpriteRenderer sr; 

        void Start()
        {
            // 画像パーツを取得
            sr = GetComponent<SpriteRenderer>();

            // 左右をランダムに決める（50%の確率）
            BOSS_isFlipped = (Random.value > 0.5f);

            // 選ばれた方向に応じて、反転と「位置」をセットする！
            if (BOSS_isFlipped)
            {
                // 【右側】が選ばれた時
                if (sr != null) sr.flipX = true; // 画像を右向きに反転
                transform.localPosition = rightPositionOffset; // 右用の位置に移動
            }
            else
            {
                // 【左側】が選ばれた時
                if (sr != null) sr.flipX = false; // 反転しない（左向きのまま）
                transform.localPosition = leftPositionOffset; // 左用の位置に移動
            }

            BOSS_currentScale = BOSS_initialScale;
            BOSS_ApplyScale();
        }

        void Update()
        {
            BOSS_currentScale += BOSS_growSpeed * Time.deltaTime;
            BOSS_ApplyScale();

            if (BOSS_currentScale >= BOSS_maxScale)
            {
                Destroy(gameObject);
            }
        }

        private void BOSS_ApplyScale()
        {
            // ★修正：変なマイナスは使わず、純粋に大きくするだけ！
            // 反転は flipX がやってくれているので、これで正しく「外側」に向かって膨らみます。
            transform.localScale = new Vector3(BOSS_currentScale, BOSS_currentScale, 1f);
        }
    }
}