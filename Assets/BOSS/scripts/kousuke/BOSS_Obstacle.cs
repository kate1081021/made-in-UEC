using UnityEngine;

namespace BOSS
{
    public class BOSS_Obstacle : MonoBehaviour
    {
        [Header("落下の設定")]
        public float baseFallSpeed = 5f; // 基本の落ちるスピード
        public float destroyY = -7f;     // このY座標を下回ったら消す

        [Header("見た目の設定（ランダム画像）")]
        public Sprite[] randomSprites;   // ★追加：画像を複数入れられる「リスト」の枠

        // 生成時に ObstacleManager から呼ばれる
        public void Init()
        {
            // ★追加：画像リストに中身がセットされていれば、ランダムに1枚選ぶ！
            if (randomSprites != null && randomSprites.Length > 0)
            {
                // 自分にくっついている SpriteRenderer（画像表示パーツ）を取得
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // 0番目 ～ リストの最後の番号 の中からサイコロを振る
                    int randomIndex = Random.Range(0, randomSprites.Length);
                    
                    // 選ばれた画像をセットする！
                    sr.sprite = randomSprites[randomIndex];
                }
            }
        }

        void Update()
        {
            // 落下処理
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