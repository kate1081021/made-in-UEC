using UnityEngine;

namespace NT
{
    public class NT_judge : MiniGameBase
    {
        private int count;
        public override void OnGameStart()
        {
            count = GetComponent<NT_switch>().count;
        }

        // Update is called once per frame
        public override void OnGameEnd()
        {
            if(150<count && count<200 )
            {
                MGManager.ClearGame();
            }
            else
                {
                    //gameover
                }
        }
    }
}
