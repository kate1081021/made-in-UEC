using UnityEngine;

namespace BOSS // ★仕様書ルール：ネームスペースで囲む
{
    // ★修正：MiniGameBase ではなく MonoBehaviour に変更しました
    public class BOSS_ObstacleManager : MonoBehaviour
    {
        [Header("障害物の設定")]
        public GameObject obstaclePrefab; 
        public Sprite[] obstacleSprites;  

        [Header("出現ルールの設定")]
        public float spawnInterval = 1.0f; 
        public float minX = -8f;           
        public float maxX = 8f;            
        public float spawnY = 6f;          

        private float timer = 0f;
        private bool isSpawning = false; // ★最初は停止状態にしておく

        // ★全体マネージャーの OnGameStart() から呼んでもらう関数
        public void StartSpawning()
        {
            isSpawning = true;
            timer = 0f;
        }

        void Update()
        {
            if (!isSpawning) return;

            // Time.deltaTime は自動的に timeScale の影響を受けるため、
            // ゲームスピードが上がると生成間隔（スポーン）も自動で速くなります。
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

        // ★全体マネージャーからクリア時・ゲームオーバー時に呼んでもらう関数
        public void StopSpawning()
        {
            isSpawning = false; 
        }
    }
}