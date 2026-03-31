using UnityEngine;

namespace MyMiniGame
{
    public class BOSS_damage : MiniGameBase
    {
        // インスペクターからセットするハートの配列。
        // これの要素数（Length）が、このミニゲームの「最大HP」となる。
        [SerializeField] private GameObject[] hearts;

        // 監視用の Update は削除。player の保持も不要。

        public override void OnGameStart()
        {
            // ここでは何もしない。プレイヤー側から初期化命令を飛ばさせる。
        }

        public override void OnGameEnd() { }

        // ★追加：HPのUIを更新する専用メソッド
        // プレイヤー側から「今のHP」を渡して呼び出してもらう
        public void UpdateHeartUI(int currentLife)
        {
            if (hearts == null) return;

            // 防御的プログラミング：HPがマイナスや最大値オーバーにならないよう制限
            currentLife = Mathf.Clamp(currentLife, 0, hearts.Length);

            // 全てのハートをチェックし、現在のHP未満のインデックスなら表示、それ以上なら非表示
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null)
                {
                    // 例: currentLife が 3 なら、i が 0,1,2 の時は true(表示)、3以降は false(非表示)になる
                    hearts[i].SetActive(i < currentLife);
                }
            }
        }
    }
}