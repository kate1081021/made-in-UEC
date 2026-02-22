using UnityEngine;

namespace NT
{
    public class NT_judge : MiniGameBase
    {
        private float count;
        private NT_switch switchComponent; // スクリプト宣言

        [Header("NT")]
        [SerializeField] private NT_stamp stamp_script; // クリア時のスタンプを呼ぶ

        public override void OnGameStart()
        {
            MGManager.Load();
            switchComponent = GetComponent<NT_switch>(); // スクリプト取得
        }

        // Update is called once per frame
        public override void OnGameEnd()
        {
            count = switchComponent.count; // スクリプトのcountを取得
            Debug.Log(count); // countの値をログに出力
            if(150 < count && count < 200)
            {
                Debug.Log("NT成功");

                // スタンプを押す
                if (stamp_script != null)
                {
                    stamp_script.PressStamp(true);
                }

                MGManager.ClearGame();
            }
            else
            {
                //gameover
                Debug.Log("NT失敗");

                // スタンプを押す
                if (stamp_script != null)
                {
                    stamp_script.PressStamp(false);
                }
            }
        }
    }
}
