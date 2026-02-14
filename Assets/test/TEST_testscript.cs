using UnityEngine;

namespace testgame {
    public class TEST_testscript : MiniGameBase
    {
        //
        
        // ミニゲーム開始時
        public override void OnGameStart()
        {
            MGManager.Load(5.0f);
            MGManager.ClearGame();
        }
        void Update()
        {
            
        }

        // ミニゲーム終了時
        public override void OnGameEnd()
        {
            
        }
    }
}