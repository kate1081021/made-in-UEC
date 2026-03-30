using UnityEngine;

namespace BOSS
{
    // ★追加：画像と「その画像専用の大きさ」をセットで登録できる仕組み
    [System.Serializable]
    public class ObstacleVisual
    {
        public Sprite obstacleSprite;    // 画像
        public float customScale = 1.0f; // この画像専用の大きさ（倍率）
    }

    public class BOSS_Obstacle : MonoBehaviour
    {
        [Header("落下の設定")]
        public float baseFallSpeed = 5f; // 基本の落ちるスピード
        public float destroyY = -7f;     // このY座標を下回ったら消す

        [Header("回転の設定")]
        public float rotationSpeed = 90f; // 1秒間に回転する度数
        public bool randomRotationDirection = true; // 回転方向をランダムにするか
        private float actualRotationSpeed;

        [Header("見た目と大きさの設定（画像ごとに調整）")]
        // ★これまでの randomSprites の代わりに、画像と大きさをセットで登録できるリスト！
        public ObstacleVisual[] obstacleVisuals; 

        // 生成時に ObstacleManager から呼ばれる
        public void Init()
        {
            // --- 画像と専用サイズの設定 ---
            if (obstacleVisuals != null && obstacleVisuals.Length > 0)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // ランダムに1つ選ぶ
                    int randomIndex = Random.Range(0, obstacleVisuals.Length);
                    ObstacleVisual chosenVisual = obstacleVisuals[randomIndex];

                    // ① 選ばれた画像をセット
                    sr.sprite = chosenVisual.obstacleSprite;

                    // ② その画像「専用の大きさ」に変更！
                    transform.localScale = new Vector3(chosenVisual.customScale, chosenVisual.customScale, 1f);
                }
            }

            // --- 回転方向の設定 ---
            if (randomRotationDirection)
            {
                // 時計回りか反時計回りをランダムに決める
                actualRotationSpeed = rotationSpeed * (Random.value > 0.5f ? 1f : -1f);
            }
            else
            {
                actualRotationSpeed = rotationSpeed;
            }
        }

        void Update()
        {
            // 落下処理（★ちゃんと down になっています！）
            transform.position += Vector3.down * baseFallSpeed * Time.deltaTime;

            // 回転処理
            transform.Rotate(new Vector3(0, 0, actualRotationSpeed * Time.deltaTime));

            // 消去判定
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