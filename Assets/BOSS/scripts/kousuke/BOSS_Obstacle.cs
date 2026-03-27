using UnityEngine;

namespace BOSS
{
    public class BOSS_Obstacle : MonoBehaviour
    {
        [Header("落下の設定")]
        public float baseFallSpeed = 5f; // 基本の落ちるスピード
        public float destroyY = -7f;     // このY座標を下回ったら消す

        private float currentFallSpeed;

        // ★生成時に ObstacleManager から呼ばれる
        public void Init()
        {
            // 仕様書ルール：timeScale を掛けて初期化
            currentFallSpeed = baseFallSpeed * Time.timeScale;
        }

        void Update()
        {
            // Time.unscaledDeltaTime を使い、timeScaleの二重掛けを防ぎつつ移動
            transform.position += Vector3.down * currentFallSpeed * Time.unscaledDeltaTime;

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