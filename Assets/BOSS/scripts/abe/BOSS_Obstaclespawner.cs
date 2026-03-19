using UnityEngine;

namespace MyMiniGame
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject obstaclePrefab;
        // ランダムな間隔の最小値と最大値を設定できるように変更
        [SerializeField] private float minSpawnInterval = 0.5f;
        [SerializeField] private float maxSpawnInterval = 2.0f;
        [SerializeField] private float spawnXRange = 7.0f;

        private float timer;
        private float currentInterval;

        void Start()
        {
            // 最初の出現間隔をランダムに決定
            currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= currentInterval)
            {
                if (obstaclePrefab != null)
                {
                    float rx = Random.Range(-spawnXRange, spawnXRange);
                    Vector3 pos = new Vector3(rx, transform.position.y, 0);
                    Instantiate(obstaclePrefab, pos, Quaternion.identity);
                }

                timer = 0;
                // 次に出現するまでの間隔を再度ランダムに決定
                currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }
}