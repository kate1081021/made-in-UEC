using UnityEngine;

namespace BOSS
{
    public class BOSS_ObstacleManager : MonoBehaviour
    {
        [Header("障害物の設定")]
        public GameObject obstaclePrefab; // 降らせる障害物（BOSS_Obstacle）のプレハブ
        public Sprite[] obstacleSprites;  // ランダムに選ばれる画像のリスト

        [Header("出現ルールの設定")]
        public float spawnInterval = 1.0f; // 何秒ごとに落とすか
        public float minX = -8f;           // 出現するX座標の左端
        public float maxX = 8f;            // 出現するX座標の右端
        public float spawnY = 6f;          // 出現するY座標（画面の一番上）

        private float timer = 0f;
        private bool isSpawning = false;

        // ★全体マネージャーの OnGameStart() から呼んでもらう関数
        public void StartSpawning()
        {
            isSpawning = true;
            timer = 0f;
        }

        void Update()
        {
            if (!isSpawning) return;

            // Time.deltaTime は自動的に timeScale の影響を受けます
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnObstacle();
                timer = 0f; 
            }
        }

        private void SpawnObstacle()
        {
            if (obstaclePrefab == null) return;

            float randomX = Random.Range(minX, maxX);
            Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

            GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

            // 障害物のスクリプトを取得して、速度の初期化（timeScale対応）を行う
            BOSS_Obstacle obstacleScript = obj.GetComponent<BOSS_Obstacle>();
            if (obstacleScript != null)
            {
                obstacleScript.Init();
            }

            // 画像をランダムに変更
            if (obstacleSprites.Length > 0)
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    int randomIndex = Random.Range(0, obstacleSprites.Length);
                    sr.sprite = obstacleSprites[randomIndex];
                }
            }
        }

        // ★全体マネージャーからクリア時などに呼んでもらう関数
        public void StopSpawning()
        {
            isSpawning = false; 
        }
    }
}