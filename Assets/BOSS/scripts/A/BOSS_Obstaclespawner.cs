using UnityEngine;

namespace BOSS 
{
    public class BOSS_ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject obstaclePrefab;
        [SerializeField] private float minSpawnInterval = 0.5f;
        [SerializeField] private float maxSpawnInterval = 2.0f;
        [SerializeField] private float spawnXRange = 7.0f;

        private float timer;
        private float currentInterval;
        private bool isSpawning = false; 

        public void StartSpawning()
        {
            isSpawning = true;
            currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            timer = 0f; 
        }

        public void StopSpawning()
        {
            isSpawning = false;
        }

        void Update()
        {
            if (!BOSS_StartSequence.isGamePlaying) return;

            if (!isSpawning) return;

            timer += Time.deltaTime;
            if (timer >= currentInterval)
            {
                if (obstaclePrefab != null)
                {
                    float rx = Random.Range(-spawnXRange, spawnXRange);
                    
                    // ==========================================
                    // ★修正：スポーナーの置かれている場所に関係なく、
                    // 「カメラの画面の一番下（からさらに-1.0f下げた画面外）」のY座標を計算してそこから出す！
                    // ==========================================
                    float bottomOfScreenY = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y - 1.0f;
                    
                    Vector3 pos = new Vector3(rx, bottomOfScreenY, 0);
                    Instantiate(obstaclePrefab, pos, Quaternion.identity);
                }

                timer = 0;
                currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }
}