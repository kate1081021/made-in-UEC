using UnityEngine;
using System.Diagnostics;


namespace NT
{
    public class NT_judge : MiniGameBase
    {
        private bool isCleared = false;
        private float count;
        private Stopwatch sw;
        private NT_switch switchComponent; // スクリプト宣言

        [Header("NT")]
        [SerializeField] private NT_stamp stamp_script; // クリア時のスタンプを呼ぶ

        public override void OnGameStart()
        {
            MGManager.Load();
            switchComponent = GetComponent<NT_switch>(); // スクリプト取得
        }

        void Update()
        {
            if (isCleared) return;
            count = switchComponent.count;
            sw = switchComponent.sw;
            
            if (150 < count && count < 200 && sw.ElapsedMilliseconds > 2000)
            {
                isCleared = true;
                MGManager.ClearGame();
            }
        }

        public override void OnGameEnd()
        {
            count = switchComponent.count;// スクリプトのcountを取得
            sw = switchComponent.sw; 
            UnityEngine.Debug.Log(count); // countの値をログに出力
            if(150 < count && count < 200)
            {
                UnityEngine.Debug.Log("NT成功");

                // スタンプを押す
                if (stamp_script != null)
                {
                    stamp_script.PressStamp(true);
                }
                if (isCleared) return;
                MGManager.ClearGame();
            }
            else
            {
                //gameover
                UnityEngine.Debug.Log("NT失敗");

                // スタンプを押す
                if (stamp_script != null)
                {
                    stamp_script.PressStamp(false);
                }
            }
        }
    }
}
