using UnityEngine;

namespace LoupeFire
{
    public class LF_test : MiniGameBase
    {
        public override void OnGameStart()
        {
            MGManager.Load();
        }
        void Update()
        {
            if (Action.WasPerformedThisFrame())
            {
                MGManager.ClearGame();
            }
        }
    }
}