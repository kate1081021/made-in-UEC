using System.Collections; // コルーチンを使うためにこれが必要だよ！
using Unity.VisualScripting;
using UnityEngine;

namespace BOSS
{
    public class BOSS_PlayerControler : MiniGameBase
    {
        Rigidbody2D BOSS_playerRb;
        SpriteRenderer BOSS_playerSprite;

        [Header("プレイヤー設定")]
        [SerializeField] public int BOSS_playerLife = 3;
        public int BOSS_playerSpeed = 5;

        [Header("無敵設定バイブス")]
        [SerializeField] public float BOSS_invincibleTime = 2.0f; // 無敵が続く秒数
        [SerializeField] public float BOSS_blinkInterval = 0.1f;  // 点滅する速さ
        private bool BOSS_isInvincible = false;                  // 今、無敵中かどうかのフラグ

        private Vector2 BOSS_screenLimit;
        private Vector2 BOSS_playerHalfSize;

        public override void OnGameStart()
        {
            MGManager.Load();
            BOSS_playerRb = GetComponent<Rigidbody2D>();
            BOSS_playerSprite = GetComponent<SpriteRenderer>();

            // 画面端の計算
            BOSS_screenLimit = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
            if (BOSS_playerSprite != null)
            {
                BOSS_playerHalfSize = BOSS_playerSprite.bounds.extents;
            }
        }

        void Update()
        {
            BOSS_playerRb.linearVelocity = BOSS_playerSpeed * Move.ReadValue<Vector2>();
            BOSS_ClampPosition();
        }

        // --- 当たり判定 ---
        private void OnTriggerEnter2D(Collider2D BOSS_collision)
        {
            // 無敵中じゃなくて、当たった相手が「Obstacle」タグを持ってたらダメージ
            // ※障害物のPrefabのTagを「Obstacle」に設定すること
            if (!BOSS_isInvincible && BOSS_collision.CompareTag("Obstacle"))
            {
                BOSS_ApplyDamage();
            }
        }

        void BOSS_ApplyDamage()
        {
            // ライフを減らす！
            BOSS_playerLife--;
            Debug.Log("残りライフ: " + BOSS_playerLife);

            if (BOSS_playerLife <= 0)
            {
                Debug.Log("ゲームオーバー");
                // ここにゲームオーバーの処理。
            }
            else
            {
                // 無敵＆点滅スタート
                StartCoroutine(BOSS_InvincibleRoutine());
            }
        }

        // --- 無敵と点滅を制御するコルーチン ---
        IEnumerator BOSS_InvincibleRoutine()
        {
            BOSS_isInvincible = true;
            float BOSS_elapsedTime = 0;

            // 無敵時間が終わるまでループ
            while (BOSS_elapsedTime < BOSS_invincibleTime)
            {
                // スプライトを表示・非表示させて点滅させる
                BOSS_playerSprite.enabled = !BOSS_playerSprite.enabled;

                // 指定した秒数だけ待つ
                yield return new WaitForSeconds(BOSS_blinkInterval);
                BOSS_elapsedTime += BOSS_blinkInterval;
            }

            // 最後は必ず表示されるようにして、無敵終了
            BOSS_playerSprite.enabled = true;
            BOSS_isInvincible = false;
        }

        void BOSS_ClampPosition()
        {
            Vector3 BOSS_pos = transform.position;
            BOSS_pos.x = Mathf.Clamp(BOSS_pos.x, -BOSS_screenLimit.x + BOSS_playerHalfSize.x, BOSS_screenLimit.x - BOSS_playerHalfSize.x);
            BOSS_pos.y = Mathf.Clamp(BOSS_pos.y, -BOSS_screenLimit.y + BOSS_playerHalfSize.y, BOSS_screenLimit.y - BOSS_playerHalfSize.y);
            transform.position = BOSS_pos;
        }
    }
}