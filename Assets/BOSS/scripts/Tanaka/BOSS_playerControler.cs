using System.Collections;
using UnityEngine;

namespace BOSS
{
    public class BOSS_playerControler : MiniGameBase
    {
        Rigidbody2D BOSS_playerRb;
        SpriteRenderer BOSS_playerSprite;
        Animator BOSS_playerAnim;
        public bool BOSS_isGameOver = false; // ゲームオーバー演出中かどうかのフラグ
        
        // ★追加：演出が終わるまで動けないようにするフラグ（最初は false）
        public bool BOSS_canMove = false; 

        [Header("ジャンプ無敵設定")]
        [SerializeField] public float BOSS_jumpInvincibleTime = 1.0f; 
        [SerializeField] public float BOSS_jumpCoolTime = 2.0f;       
        private bool BOSS_isJumpCooldown = false;

        [Header("プレイヤー設定")]
        [SerializeField] public int BOSS_playerLife = 3;
        public int BOSS_playerSpeed = 5;

        [Header("無敵設定")]
        [SerializeField] public float BOSS_invincibleTime = 2.0f; 
        [SerializeField] public float BOSS_blinkInterval = 0.1f;  
        [SerializeField] public float BOSS_jumpBlinkInterval = 0.1f;  
        private bool BOSS_isInvincible = false;                  
        private Vector2 BOSS_screenLimit;
        private Vector2 BOSS_playerHalfSize;

        public override void OnGameStart()
        {
            MGManager.Load();
            BOSS_playerRb = GetComponent<Rigidbody2D>();
            BOSS_playerSprite = GetComponent<SpriteRenderer>();
            BOSS_playerAnim = GetComponent<Animator>();

            // 画面端の計算
            BOSS_screenLimit = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
            if (BOSS_playerSprite != null)
            {
                BOSS_playerHalfSize = BOSS_playerSprite.bounds.extents;
            }
        }

        void Update()
        {
            // ★追加：演出が終わる（BOSS_canMoveがtrueになる）までは、ここで処理を止める！
            if (!BOSS_canMove) return;

            // 移動処理
            BOSS_playerRb.linearVelocity = BOSS_playerSpeed * Move.ReadValue<Vector2>();
            if (!BOSS_isGameOver)
            {
                BOSS_ClampPosition();
            }

            if (Action.WasPressedThisFrame() && !BOSS_isInvincible && !BOSS_isJumpCooldown && !BOSS_isGameOver)
            {
                StartCoroutine(BOSS_JumpInvincibleWithCooldown());
            }
        }

        // --- 無敵とクールタイムをセットで管理するコルーチン ---
        IEnumerator BOSS_JumpInvincibleWithCooldown()
        {
            // 1. 無敵スタート ＆ アニメ切り替え！
            BOSS_isInvincible = true;
            float BOSS_elapsedTime = 0;
            SEPlay("BOSS_JumpSE", false);
            BOSS_playerAnim.SetBool("isJumping", true); // アニメーションON
            
            while (BOSS_elapsedTime < BOSS_invincibleTime)
            {
                // スプライトを表示・非表示させて点滅させる
                BOSS_playerSprite.enabled = !BOSS_playerSprite.enabled;
                yield return new WaitForSeconds(BOSS_jumpBlinkInterval);
                BOSS_elapsedTime += BOSS_blinkInterval;
            }
            // 最後は必ず表示されるようにして、無敵終了
            BOSS_playerSprite.enabled = true;

            // 2. 無敵終了 ＆ アニメを戻す
            BOSS_playerSprite.enabled = true; // 念のため表示を確実にする
            BOSS_isInvincible = false;
            BOSS_playerAnim.SetBool("isJumping", false); // アニメーションOFF

            // 3. クールタイム開始
            BOSS_isJumpCooldown = true;
            yield return new WaitForSeconds(BOSS_jumpCoolTime);

            // 4. クールタイム終了
            BOSS_isJumpCooldown = false;
        }

        // --- 当たり判定 ---
        private void OnTriggerEnter2D(Collider2D BOSS_collision)
        {
            if (!BOSS_isInvincible && BOSS_collision.CompareTag("Obstacle"))
            {
                SEPlay("BOSS_DamageSE", false);
                BOSS_ApplyDamage();
            }
        }

        void BOSS_ApplyDamage()
        {
            BOSS_playerLife--;
            Debug.Log("残りライフ: " + BOSS_playerLife);

            if (BOSS_playerLife <= 0)
            {
                Debug.Log("ゲームオーバー演出開始！");
                StartCoroutine(BOSS_GameOverSequence());
            }
            else
            {
                StartCoroutine(BOSS_InvincibleRoutine());
            }
        }

        // --- ゲームオーバー演出のコルーチン ---
        IEnumerator BOSS_GameOverSequence()
        {
            // 1. フラグON ＆ 他のシステムを停止
            BOSS_isGameOver = true;
            var allSystems = FindObjectsByType<MiniGameBase>(FindObjectsSortMode.None);
            foreach (var system in allSystems)
            {
                if (system != this) system.enabled = false;
            }

            // 2. 物理を止める
            Rigidbody2D[] allRbs = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
            foreach (var rb in allRbs)
            {
                rb.linearVelocity = Vector2.zero;
                if (rb.gameObject != this.gameObject) rb.simulated = false;
            }

            // 3. 画面上の「Obstacle」タグが付いたプレハブを全部消す
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (GameObject obj in obstacles)
            {
                Destroy(obj); // 画面から消去
            }

            // 4. ゲームオーバーアニメーション ＆ 物理オフ
            BOSS_playerAnim.SetBool("Gameover", true);
            BOSS_playerRb.simulated = false;

            // 5. 画面下へ落下
            float fallSpeed = 5.0f;
            while (transform.position.y > -BOSS_screenLimit.y - 3.0f)
            {
                transform.position += Vector3.up * -fallSpeed * Time.deltaTime;
                yield return null;
            }

            // 6. 完了
            BOSS_FinalGameOverTrigger();
        }

        void BOSS_FinalGameOverTrigger()
        {
            Debug.Log("運営側のゲームオーバー処理");
            MGManager.FinishGame();
        }

        IEnumerator BOSS_JumpInvincibleRoutine()
        {
            BOSS_isInvincible = true;
            float BOSS_elapsedTime = 0;
            float BOSS_jumpInvincibleTime = 1.0f; // 1秒固定
            while (BOSS_elapsedTime < BOSS_jumpInvincibleTime)
            {
                BOSS_playerSprite.enabled = !BOSS_playerSprite.enabled;
                yield return new WaitForSeconds(BOSS_blinkInterval);
                BOSS_elapsedTime += BOSS_blinkInterval;
            }
            BOSS_playerSprite.enabled = true;
            BOSS_isInvincible = false;
        }

        // --- 無敵と点滅を制御するコルーチン ---
        IEnumerator BOSS_InvincibleRoutine()
        {
            BOSS_isInvincible = true;
            float BOSS_elapsedTime = 0;
            while (BOSS_elapsedTime < BOSS_invincibleTime)
            {
                BOSS_playerSprite.enabled = !BOSS_playerSprite.enabled;
                yield return new WaitForSeconds(BOSS_blinkInterval);
                BOSS_elapsedTime += BOSS_blinkInterval;
            }
            BOSS_playerSprite.enabled = true;
            BOSS_isInvincible = false;
        }

        void BOSS_ClampPosition()
        {
            Vector3 BOSS_pos = transform.position;
            BOSS_pos.x = Mathf.Clamp(BOSS_pos.x, -6.0f, 6.0f);
            BOSS_pos.y = Mathf.Clamp(BOSS_pos.y, -BOSS_screenLimit.y + BOSS_playerHalfSize.y, BOSS_screenLimit.y - BOSS_playerHalfSize.y);
            transform.position = BOSS_pos;
        }
    }
}