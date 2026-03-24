using UnityEngine;

namespace BOSS
{
    public class BOSS_Obstacle : MonoBehaviour
    {
        [Header("落下の設定")]
        public float baseFallSpeed = 5f; // 基本の落ちるスピード
        public float destroyY = -7f;     // このY座標を下回ったら消す

        // ★生成時に ObstacleManager から呼ばれる
        public void Init()
        {
            // （今回は特に速度の事前計算が不要になったので空っぽでOKです。
            // もし今後「出現時のエフェクト」などを追加したくなったらここに書けます！）
        }

        void Update()
        {
            // ★修正：Time.deltaTime に変更！
            // 倍速にする必要がなく、一時停止（ポーズ）にも自動で対応してくれる最強の書き方です。
            transform.position += Vector3.down * baseFallSpeed * Time.deltaTime;

            if (transform.position.y < destroyY)
            {
                Destroy(gameObject);
            }
        }

        // プレイヤーとの当たり判定
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("プレイヤーに障害物がヒットしました！");
                
                // TODO: ライフ管理担当者のダメージ処理関数を後で追記する
            }
        }
    }
}