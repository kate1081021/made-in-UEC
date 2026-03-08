using UnityEngine;
using PTgame;

namespace PTgame
{
    public class PT_Ball : MiniGameBase
    {
        [Header("Ball Settings")]
        [SerializeField] private float ballDuration = 0.2f;
        [SerializeField] private bool randomDirection = true;
        [SerializeField] private PT_Obstacle obstacle;

        [Header("Ball Effect")]
        [SerializeField] private GameObject ballPrefab; // ★Prefab名をballに変更
        [SerializeField] private Transform spawnPoint;  // ★任意
        [SerializeField] private bool isse = true;
        [SerializeField] private AudioClip se;

        private float ballTimer = 0f;
        private float basePower;
        private bool hasBalled = false;

        public override void OnGameStart()
        {
            obstacle = GetComponent<PT_Obstacle>();
            if (obstacle != null)
            {
                basePower = obstacle.power;
                obstacle.power = 0f;
            }
            Ball();
        }

        void Update()
        {
            if (!hasBalled) return;

            if (ballTimer > 0f)
            {
                ballTimer -= Time.deltaTime;

                if (ballTimer <= 0f)
                {
                    obstacle.power = 0f;
                    hasBalled = false;
                    Destroy(gameObject, 1f);
                }
                if (isse)
                {
                    isse = false;
                    SEPlay("c", se);
                }
            }
        }

        private void Ball()
        {
            if (obstacle == null) return;

            // パワー設定
            int pm = Random.Range(1, 3);
            obstacle.power = (pm == 1) ? -basePower : basePower;

            // ★オブジェクト生成（斜め後方から飛んでくる）
            if (ballPrefab != null)
            {
                float dir = Mathf.Sign(obstacle.power);

                // 横は画面端、縦は画面上
                float edgeX = (dir > 0)
                    ? Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).x - 1f
                    : Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)).x + 1f;

                float topY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y + 1f;

                Vector3 pos = new Vector3(edgeX, topY, 0f);

                GameObject obj = Instantiate(ballPrefab, pos, Quaternion.identity);

                // ←★斜め後方飛行スクリプトにInit
                obj.GetComponent<PTgame.PT_DiagonalFly>().Init(dir);
            }

            ballTimer = ballDuration;
            hasBalled = true;
        }
    }
}