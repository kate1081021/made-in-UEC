using UnityEngine;

namespace WI
{
    public class WI_T_advertisement : MiniGameBase 
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart() { }

        public override void OnGameEnd() { }
        void Update() { }

        public void CloseWindow()
        {
            Destroy(this.gameObject);
        }
    }
}
