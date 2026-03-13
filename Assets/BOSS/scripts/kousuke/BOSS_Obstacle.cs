using UnityEngine;

namespace BOSS // ★仕様書ルール：ネームスペースで囲む
{
    // ★修正：MiniGameBase ではなく MonoBehaviour に変更しました
    public class BOSS_Obstacle : MonoBehaviour
    {
        [Header("落下の設定")]
        public float baseFallSpeed = 5f; // 基本の落ちるスピード
        public float destroyY = -7f; 

        private float currentFallSpeed;

        // ★仕様書ルール：Start() は使わず、生成時にマネージャーから呼ばれる初期化関数を用意する
        public void Init()
        {
            // 仕様書の「移動速度 = 5 * Time.timeScale; という風に初期化」に対応
            currentFallSpeed = baseFallSpeed * Time.timeScale;
        }

        void Update()
        {
            // 下に向かって移動する（Time.unscaledDeltaTime を使うことで、timeScaleの二重掛けを防ぎつつ滑らかに動かします）
            transform.position += Vector3.down * currentFallSpeed * Time.unscaledDeltaTime;

            if (transform.position.y < destroyY)
            {
                Destroy(gameObject);
            }
        }

        // プレイヤーとの当たり判定（ライフ・UI担当者と連携する部分）
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("プレイヤーに障害物がヒットしました！");
                
                // TODO: ライフ管理担当者の関数が完成したらここに追記する
                // 例: collision.GetComponent<BOSS_LifeManager>().TakeDamage();
            }
        }
    }
}