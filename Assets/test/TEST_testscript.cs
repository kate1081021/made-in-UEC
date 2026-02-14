using UnityEngine;

namespace testgame {
    public class TEST_testscript : MiniGameBase
    {
        // ミニゲーム開始時
        public override void OnGameStart()
        {
            MGManager.Load();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MGManager.ClearGame();
            }
        }
        // ミニゲーム終了時
        public override void OnGameEnd()
        {
            
        }
    }
}