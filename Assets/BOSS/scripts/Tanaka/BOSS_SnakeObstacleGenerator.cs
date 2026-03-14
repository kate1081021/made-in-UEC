using UnityEngine;

namespace BOSS
{
    public class BOSS_ObstacleGenerator : MiniGameBase
    {
        [Header("ジェネレーター設定")]
        [SerializeField] 
        public GameObject BOSS_ObstaclePrefab; // さっき作った障害物のプレハブをここに入れる
        
        [SerializeField] 
        public float BOSS_SpawnInterval = 1.5f; // 降らせる間隔

        private float BOSS_SpawnTimer = 0f;     // タイマー用の変数
        private Vector2 BOSS_ScreenLimit;       // 画面端を覚える用

        public override void OnGameStart()
        {
            MGManager.Load();
            
            // 画面の右上（1,1）の座標を取得して、出現範囲を決める
            BOSS_ScreenLimit = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        }

        void Update()
        {
            // 毎フレーム、時間をタイマーに足していく
            BOSS_SpawnTimer += Time.deltaTime;

            // もしタイマーが、設定した秒数（BOSS_SpawnInterval）を超えたら
            if (BOSS_SpawnTimer >= BOSS_SpawnInterval)
            {
                // 障害物生成
                BOSS_SpawnObstacle();
                
                // タイマーを0に戻して、また次のカウントダウンを開始する
                BOSS_SpawnTimer = 0f;
            }
        }

        void BOSS_SpawnObstacle()
        {
            if (BOSS_ObstaclePrefab == null) return;

            // 1. 出現する横位置（X）をランダムで決める
            // 画面の左端から右端までの間でランダム
            float BOSS_RandomX = Random.Range(-BOSS_ScreenLimit.x + 0.5f, BOSS_ScreenLimit.x - 0.5f);

            // 2. 出現する位置（Y）は画面の少し上に設定
            Vector3 BOSS_SpawnPos = new Vector3(BOSS_RandomX, BOSS_ScreenLimit.y + 1.0f, 0);

            // 3. プレハブのクローンを生成（Instantiate）
            Instantiate(BOSS_ObstaclePrefab, BOSS_SpawnPos, Quaternion.identity);
        }
    }
}