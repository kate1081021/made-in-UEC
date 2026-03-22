using UnityEngine;
using System.Collections;

namespace BOSS
{
    public class BOSS_ObstacleManager : MonoBehaviour
    {
        [Header("降ってくる障害物の設定")]
        public GameObject fallingObstaclePrefab;
        public float spawnInterval = 1.5f;

        private Coroutine spawnCoroutine;

        // Directorから呼ばれる「生成スタート」の合図
        public void StartSpawning()
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnRoutine());
            }
        }

        // Directorから呼ばれる「生成ストップ」の合図
        public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                SpawnObstacle();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnObstacle()
        {
            if (fallingObstaclePrefab != null)
            {
                // 画面の上のほうからランダムな位置で降らせる
                float randomX = Random.Range(-7f, 7f);
                float topY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y + 2f;
                
                GameObject obj = Instantiate(fallingObstaclePrefab, new Vector3(randomX, topY, 0), Quaternion.identity);
                
                // もし障害物にBOSS_ObstacleがついていたらInitを呼ぶ
                BOSS_Obstacle obstacleScript = obj.GetComponent<BOSS_Obstacle>();
                if (obstacleScript != null)
                {
                    obstacleScript.Init();
                }
            }
        }
    }
}
