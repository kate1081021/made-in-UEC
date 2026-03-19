using UnityEngine;

namespace BOSS
{
    public class BOSS_BaloonGenerator : MonoBehaviour
    {
        [Header("ジェネレータの設定")]
        [SerializeField] public GameObject BOSS_baloonPrefab; // はなちょうちんのプレハブ
        [SerializeField] public float BOSS_spawnInterval = 5.0f; // 召喚の間隔

        private float BOSS_spawnTimer;
        private float BOSS_screenLeft;
        private float BOSS_screenRight;
        private float BOSS_minY;
        private float BOSS_maxY;

        void Start()
        {
            // カメラから画面の端っこの座標を取得する
            Vector2 BOSS_min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            Vector2 BOSS_max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            BOSS_screenLeft = BOSS_min.x;
            BOSS_screenRight = BOSS_max.x;
            BOSS_minY = BOSS_min.y;

            BOSS_spawnTimer = BOSS_spawnInterval;
        }

        void Update()
        {
            BOSS_spawnTimer += Time.deltaTime;

            if (BOSS_spawnTimer >= BOSS_spawnInterval)
            {
                BOSS_SpawnBaloon();
                BOSS_spawnTimer = 0f;
            }
        }

        private void BOSS_SpawnBaloon()
        {
            if (BOSS_baloonPrefab == null) return;

            // 1. 左右どっちに出すか決める（0なら左、1なら右）
            bool BOSS_isRight = Random.Range(0, 2) == 1;
            float BOSS_spawnX = BOSS_isRight ? BOSS_screenRight : BOSS_screenLeft;

            // 2. 高さは画面内のランダム
            float BOSS_spawnY = BOSS_minY;

            // 3. 生成！
            GameObject BOSS_newBaloon = Instantiate(BOSS_baloonPrefab, new Vector3(BOSS_spawnX, BOSS_spawnY, 0), Quaternion.identity);

            // 4. 右側の時は左右反転させる
            // スクリプトを取得して、反転フラグをオンにする
            BOSS_BaloonControler BOSS_ctrl = BOSS_newBaloon.GetComponent<BOSS_BaloonControler>();
            if (BOSS_ctrl != null && BOSS_isRight)
            {
                BOSS_ctrl.BOSS_isFlipped = true;
            }
        }
    }
}