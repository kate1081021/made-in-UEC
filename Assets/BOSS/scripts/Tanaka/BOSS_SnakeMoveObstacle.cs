using UnityEngine;

namespace BOSS
{
    public class BOSS_SnakeMoveObstacle : MonoBehaviour 
    {
        [Header("蛇行障害物の設定")]
        [SerializeField] public float BOSS_fallSpeed = 3.0f;     // 落ちる速さ
        [SerializeField] public float BOSS_waveAmplitude = 2.0f; // 左右に振れる幅
        [SerializeField] public float BOSS_waveFrequency = 3.0f; // ウネウネの速さ

        private float BOSS_spawnX;       // 最初に生成された横位置
        private float BOSS_birthTime;    // 生まれた時間
        private float BOSS_screenBottom; // 画面の一番下の座標

        void Start()
        {
            // 生まれた時のX座標。ここを中心に蛇行する
            BOSS_spawnX = transform.position.x;
            // 生まれた時間を記録する。Mathf.Sinの計算に使う
            BOSS_birthTime = Time.time;
            // 画面の下端を計算しておく（これより下に行ったら削除）
            BOSS_screenBottom = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y;
        }

        void Update()
        {
            // 1. 縦の動き：一定のスピードで下に落ちる
            float BOSS_newY = transform.position.y - (BOSS_fallSpeed * Time.deltaTime);

            // 2. 横の動き：Mathf.Sinを使って蛇行させる
            // Mathf.Sin(時間 * 速さ) * 振れ幅
            float BOSS_timePassed = Time.time - BOSS_birthTime;
            float BOSS_offset = Mathf.Sin(BOSS_timePassed * BOSS_waveFrequency) * BOSS_waveAmplitude;
            float BOSS_newX = BOSS_spawnX + BOSS_offset;

            // 3. 新しい位置をセット
            transform.position = new Vector3(BOSS_newX, BOSS_newY, 0);

            // 4. 画面外（下）に行ったら自分を消去
            if (transform.position.y < BOSS_screenBottom - 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}