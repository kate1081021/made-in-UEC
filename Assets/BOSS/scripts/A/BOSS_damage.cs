using UnityEngine;

namespace MyMiniGame
{
    public class BOSS_damage : MiniGameBase
    {
        [SerializeField] private GameObject[] hearts;
        private int life;

        // 監視対象のプレイヤーを保持する
        private BOSS.BOSS_playerControler player;

        public override void OnGameStart()
        {
            // MGManager.Load(); // 必要なら残す

            // シーン内からプレイヤーのスクリプトを探して捕まえる
            player = Object.FindAnyObjectByType<BOSS.BOSS_playerControler>();

            life = hearts.Length;

            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null) hearts[i].SetActive(true);
            }
        }

        void Update()
        {
            // プレイヤーが見つかっていない、またはゲーム中じゃないなら何もしない
            if (player == null) return;


            // 今のUIの「life」より小さくなっていたら、ダメージを受けたと判断する
            if (player.BOSS_playerLife < life)
            {
                BOSS_ApplyDamage();
            }
        }

        public override void OnGameEnd() { }

        public void BOSS_ApplyDamage()
        {
            if (life > 0)
            {
                life--;
                hearts[life].SetActive(false);
            }
        }
    }
}