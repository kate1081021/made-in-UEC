using UnityEngine;

namespace BOSS
{
    // ※司令塔から生成される「ただの部品」になるので、MiniGameBaseではなくMonoBehaviourにします
    public class BOSS_RodObstacleGenelator : MonoBehaviour 
    {
        [Header("ジェネレーター設定")]
        [SerializeField] 
        public GameObject BOSS_ObstaclePrefab; // 障害物のプレハブ
        
        [SerializeField] 
        public float BOSS_SpawnInterval = 1.5f; // 降らせる間隔

        private float BOSS_SpawnTimer = 0f;     
        
        // ★追加：生成中かどうかのスイッチ
        private bool isSpawning = false; 

        // ★追加：司令塔から呼ばれるスイッチONの命令
        public void StartSpawning()
        {
            isSpawning = true;
            BOSS_SpawnTimer = 0f; // タイマーをリセット
        }

        // ★追加：司令塔から呼ばれるスイッチOFFの命令
        public void StopSpawning()
        {
            isSpawning = false;
        }

        void Update()
        {
            // ★追加：スイッチがオフの時は何もしない！
            if (!isSpawning) return;

            // 毎フレーム、時間をタイマーに足していく
            BOSS_SpawnTimer += Time.deltaTime;

            // もしタイマーが、設定した秒数を超えたら
            if (BOSS_SpawnTimer >= BOSS_SpawnInterval)
            {
                BOSS_SpawnRodObstacle();
                BOSS_SpawnTimer = 0f;
            }
        }

        void BOSS_SpawnRodObstacle()
        {
            if (BOSS_ObstaclePrefab == null) return;

            Vector3 BOSS_SpawnPos = new Vector3(0, 6 + 1.0f, 0);
            Quaternion BOSS_Rotation = Quaternion.Euler(0, 0, 90);

            Instantiate(BOSS_ObstaclePrefab, BOSS_SpawnPos, BOSS_Rotation);
        }
    }
}