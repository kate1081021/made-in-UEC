using UnityEngine;

namespace BK
{
    public class BK_PlayerController : MiniGameBase
    {
        // アニメーター
        [SerializeField] private Animator animator;

        // パラメーター
        public int escape = 0;  // よけていないときは0、右によけたら1、左によけたら-1をそれぞれ持つ
        private bool success = true;  // ミニゲーム成功or失敗
        private Vector2 previous_val;  // 最後の入力を入れる

        // ゲーム開始時に呼ばれる
        public override void OnGameStart()
        {
            // 特に何もしない
        }

        // 方向キーが押されたとき
        protected override void OnMovePerformed(Vector2 value)
        {
            // すでによけている場合はすぐに戻す
            if (escape != 0) { return; }

            // ゲームがすでに終わっている場合は戻る
            if (MGManager.IsClear) { return; }

            Vector2 val = convert_stick_to_dir(value);
            if (val == previous_val) { return; }
            previous_val = val;

            // 左によけた時
            if (val.x < -0.5f) {
                animator.SetTrigger("isLeftKeyPressed");
                escape = -1;
            
            // 右によけた時
            } else if (val.x > 0.5f) {
                animator.SetTrigger("isRightKeyPressed");
                escape = 1;
            }

        }

        protected override void OnMoveCanceled(Vector2 value)
        {
            previous_val = convert_stick_to_dir(value);
        }

        // エスケープアニメーションが終了したとき
        public void escapeEnded()
        {
            escape = 0;
        }

        // Walkingアニメーション開始
        public void startWalking()
        {
            animator.SetBool("walking", true);
        }
        
        // Walkingアニメーションが終了したとき
        public void walkingEnded()
        {
            if (success) { 
                // 10%の確率で城之内
                int rand = Random.Range(1, 11);
                if (rand == 5)
                {
                    animator.SetBool("Jounouchi", true);
                }
                else
                {
                    animator.SetBool("Success", true); 
                }
            }
        }

        // Failureアニメーションが呼ばれる
        public void FailureAnimation()
        {
            // 10%の確率で城之内
            int rand = Random.Range(1, 11);
            if (rand == 5)
            {
                animator.SetBool("Naoya", true);
            }
            else
            {
                animator.SetBool("Failure", true);   
            }
            animator.SetFloat("LayerSpeed", 0.0f);
            success = false;
        }

        // 毎フレームごとに呼ばれる
        void Update()
        {
            // ボタンの判定
        }
    }
}

