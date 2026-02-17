using UnityEngine;

namespace NT
{
    public class NT_judge : MiniGameBase
    {
        private int count;
        private NT_switch switchComponent; // スクリプト宣言
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
                MGManager.ClearGame();
            }
            else
            {
                //gameover
                Debug.Log("NT失敗");
            }
        }
    }
}
