using UnityEngine;
using PTgame;

namespace PTgame
{
    public class PT_dog : MonoBehaviour
    {
        [Header("Bark Settings")]
        [SerializeField] private float barkDuration = 0.2f;
        [SerializeField] private bool randomDirection = true;
        [SerializeField] private PT_Obstacle obstacle;

        [Header("Bark Effect")]
        [SerializeField] private GameObject barkPrefab; // ★追加
        [SerializeField] private Transform spawnPoint;  // ★追加（任意）

        private float barkTimer = 0f;
        private float basePower;
        private bool hasBarked = false;

        void Start()
        {
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

                if (barkTimer <= 0f)
                {
                    obstacle.power = 0f;
                    hasBarked = false;
                    Destroy(gameObject, 1f);
                }
            }
        }

        private void Bark()
        {
            if (obstacle == null) return;

            // パワー設定
            int pm = Random.Range(1, 3);
            obstacle.power = (pm == 1) ? -basePower : basePower;

            float dir = Mathf.Sign(obstacle.power);

            // ★オブジェクト生成
            if (barkPrefab != null)
            {
                Vector3 offset = new Vector3(dir * -2.5f, -2.5f, 0f); 
                Vector3 pos = offset;

                GameObject obj = Instantiate(barkPrefab, pos, Quaternion.identity);
                Destroy(obj, 1f);
            }

            barkTimer = barkDuration;
            hasBarked = true;
        }
    }
}