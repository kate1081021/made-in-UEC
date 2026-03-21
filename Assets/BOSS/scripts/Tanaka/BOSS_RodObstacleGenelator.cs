using UnityEngine;

namespace BOSS
{
    public class BOSS_RodObstacleGenelator : MiniGameBase
    {
        [Header("ジェネレーター設定")]
        [SerializeField] 
        public GameObject BOSS_ObstaclePrefab; // さっき作った障害物のプレハブをここに入れる
        
        [SerializeField] 
        public float BOSS_SpawnInterval = 1.5f; // 降らせる間隔

        private float BOSS_SpawnTimer = 0f;     // タイマー用の変数
        public override void OnGameStart()
        {
            
        }

        void Update()
        {
            // 毎フレーム、時間をタイマーに足していく
            BOSS_SpawnTimer += Time.deltaTime;

            // もしタイマーが、設定した秒数（BOSS_SpawnInterval）を超えたら
            if (BOSS_SpawnTimer >= BOSS_SpawnInterval)
            {
                // 障害物生成
                BOSS_SpawnRodObstacle();
                
                // タイマーを0に戻して、また次のカウントダウンを開始する
                BOSS_SpawnTimer = 0f;
            }
        }
        void BOSS_SpawnRodObstacle()
        {
            if (BOSS_ObstaclePrefab == null) return;

            Vector3 BOSS_SpawnPos = new Vector3(0, 6 + 1.0f, 0);

            // 3. プレハブのクローンを生成（Instantiate）
            Instantiate(BOSS_ObstaclePrefab, BOSS_SpawnPos, Quaternion.identity);
        }
    }
}