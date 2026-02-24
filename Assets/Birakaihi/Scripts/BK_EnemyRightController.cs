using UnityEngine;

namespace BK
{
    public class BK_EnemyRightController : MiniGameBase
    {
        // Enemyそのものの取得
        public GameObject enemy;
        private RectTransform enemy_pos;  // enemyの位置
        private Animator animator;  // enemyのAnimator Controller

        // Playerを取得
        public GameObject player;
        private RectTransform player_pos;  // Playerの位置

        // GameManager
        public BK_GameManager gameManager;

        // アニメーションが発動した
        private bool isAnimationTriggered = false;
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            // 各プロパティに値をセットする
            enemy_pos = enemy.GetComponent<RectTransform>();
            animator = enemy.GetComponent<Animator>();
            player_pos = player.GetComponent<RectTransform>();

        }

        // animation
        private void triggerAnimation()
        {
            // 移動させる
            Vector2 pos = enemy_pos.anchoredPosition;
            pos.x = 230f;
            enemy_pos.anchoredPosition = pos;

            // アニメーションを再生
            animator.SetBool("birakubari", true);

        }

        // Update is called once per frame
        void Update()
        {
            // アニメーションがまだ一度も動いていないときだけ
            if (!isAnimationTriggered) 
            {
                // プレイヤーが許容範囲内に入った時
                float player_y = player_pos.anchoredPosition.y;
                float enemy_y = enemy_pos.anchoredPosition.y;
                if (Mathf.Abs(player_y - enemy_y) <= BK_GameManager.escapableRange / 4)
                {
                    isAnimationTriggered = true;
                    triggerAnimation();  // アニメーションを発動
                }
            }
        }
    }

}
