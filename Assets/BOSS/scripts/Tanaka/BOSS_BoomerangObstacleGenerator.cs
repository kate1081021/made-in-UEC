using UnityEngine;

namespace BOSS
{
    public class BOSS_BoomerangObstacleGenerator : MonoBehaviour
    {
        [Header("ジェネレータの設定")]
        [SerializeField] public GameObject BOSS_obstaclePrefab; // 召喚するプレハブ
        [SerializeField] public float BOSS_spawnInterval = 1.5f; // 何秒ごとに召喚するか

        private float BOSS_spawnTimer; // 次の召喚までのタイマー
        private float BOSS_minX;       // 画面の左端
        private float BOSS_maxX;       // 画面の右端
        private float BOSS_spawnY;     // 召喚する高さ（画面の少し上）

        void Start()
        {
            // カメラの表示範囲から、召喚するX座標の範囲を計算
            Vector2 BOSS_screenMin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            Vector2 BOSS_screenMax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            // 左右の端（ちょっと余裕を持たせて内側に調整するのもよい）
            BOSS_minX = BOSS_screenMin.x;
            BOSS_maxX = BOSS_screenMax.x;

            // 画面の上端より少し高い位置をスポーン位置にする
            BOSS_spawnY = BOSS_screenMax.y + 1.0f;

            // タイマーをセット
            BOSS_spawnTimer = BOSS_spawnInterval;
        }

        void Update()
        {
            // タイマーを加算
            BOSS_spawnTimer += Time.deltaTime;

            // 指定した間隔が経ったら召喚
            if (BOSS_spawnTimer >= BOSS_spawnInterval)
            {
                BOSS_SpawnObstacle();
                BOSS_spawnTimer = 0f;
            }
        }

        private void BOSS_SpawnObstacle()
        {
            if (BOSS_obstaclePrefab == null) return;

            // 1. ランダムなX座標を決定
            float BOSS_randomX = Random.Range(BOSS_minX, BOSS_maxX);

            // 2. 召喚する位置を計算
            Vector3 BOSS_spawnPosition = new Vector3(BOSS_randomX, BOSS_spawnY, 0);

            // 3. インスタンス化（召喚）
            Instantiate(BOSS_obstaclePrefab, BOSS_spawnPosition, Quaternion.identity);
        }
    }
}