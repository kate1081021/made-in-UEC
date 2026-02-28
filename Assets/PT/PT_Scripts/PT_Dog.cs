using UnityEngine;
using PTgame;

namespace PTgame
{
    public class PT_Dog : MiniGameBase
    {
        [Header("Bark Settings")]
        [SerializeField] private float barkDuration = 0.2f;
        [SerializeField] private bool randomDirection = true;
        [SerializeField] private PT_Obstacle obstacle;

        [Header("Bark Effect")]
        [SerializeField] private GameObject barkPrefab; // ★追加
        [SerializeField] private Transform spawnPoint;  // ★追加（任意）
        [SerializeField] private bool isse;
        [SerializeField] private AudioClip se;


        private float barkTimer = 0f;
        private float basePower;
        private bool hasBarked = false;

        public override void OnGameStart()
        {
            isse = true;
            obstacle = GetComponent<PT_Obstacle>();
            if (obstacle != null)
            {
                basePower = obstacle.power;
                obstacle.power = 0f;
            }
            Bark();
        }

        void Update()
        {
            if (!hasBarked) return;

            if (barkTimer > 0f)
            {
                barkTimer -= Time.deltaTime;
                if (isse)
                {
                    isse = false;
                    SEPlay("d", se);
                }
                if (barkTimer <= 0f)
                {
                    obstacle.power = 0f;
                    hasBarked = false;
                    Destroy(gameObject, 1f);
                }
            }
        }

        public void ActiveBark(float dir)
        {
            if (obstacle == null) return;

            // パワー設定
            int pm = 1;
            if (dir == -1)
            {
                pm = 1;
            }
            if (dir == 1)
            {
                pm = 2;
            }
            obstacle.power = (pm == 1) ? -basePower : basePower;

            barkTimer = barkDuration;
            hasBarked = true;
        }

        private void Bark()
        {


            // ★オブジェクト生成
            if (barkPrefab != null)
            {
                float dir = (Random.value < 0.5f) ? -1f : 1f;

                float edgeX = (dir > 0)
                ? Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 1f
                : Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 1f;

                Vector3 pos = new Vector3(edgeX, -2.5f, 0f);
                GameObject obj = Instantiate(barkPrefab, pos, Quaternion.identity);

                // PT_DogMove に親犬を渡す
                obj.GetComponent<PT_DogMove>().Init(dir, this);
            }
        }
    }
}