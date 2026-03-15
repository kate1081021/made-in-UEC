using UnityEngine;
using PTgame;

namespace PTgame
{
    public class PT_Bike : MiniGameBase
    {
        [Header("Bell Settings")]
        [SerializeField] private float bellDuration = 0.2f;
        [SerializeField] private bool randomDirection = true;
        [SerializeField] private PT_Obstacle obstacle;

        [Header("Bell Effect")]
        [SerializeField] private GameObject bikePrefab; // ★追加
        [SerializeField] private Transform spawnPoint;  // ★追加（任意）
        [SerializeField] private bool isse = true;
        [SerializeField] private AudioClip se;

        private float bellTimer = 0f;
        private float basePower;
        private bool hasBelled = false;

        public override void OnGameStart()
        {
            obstacle = GetComponent<PT_Obstacle>();
            if (obstacle != null)
            {
                basePower = obstacle.power;
                obstacle.power = 0f;
            }
            Bell();
        }

        void Update()
        {
            if (!hasBelled) return;

            if (bellTimer > 0f)
            {
                bellTimer -= Time.deltaTime;

                if (bellTimer <= 0f)
                {
                    obstacle.power = 0f;
                    hasBelled = false;
                    Destroy(gameObject, 1f);
                }
                if (isse)
                {
                    isse = false;
                    SEPlay("b", se);
                }
            }
        }

        private void Bell()
        {
            if (obstacle == null) return;

            // パワー設定
            int pm = Random.Range(1, 3);
            obstacle.power = (pm == 1) ? -basePower : basePower;

            // ★オブジェクト生成
            if (bikePrefab != null)
            {
                float dir = Mathf.Sign(obstacle.power);
                
                // 画面端の座標を取得
                float edgeX = (dir > 0)
                ? Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 1f
                : Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 1f;
                
                Vector3 pos = new Vector3(edgeX, -2.5f, 2f);
                GameObject obj = Instantiate(bikePrefab, pos, Quaternion.identity);
                
                // ←★ここが重要
                obj.GetComponent<PTgame.PT_BikeMove>().Init(dir);
            }

            bellTimer = bellDuration;
            hasBelled = true;
        }
    }
}