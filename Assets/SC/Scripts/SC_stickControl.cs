using UnityEngine;

namespace SC
{

    public class SC_stickControl : MiniGameBase 
    {
        private Rigidbody2D rb;

        public override void OnGameStart()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.simulated = true;
        }

        public override void OnGameEnd() { }

        public void StopStick()
        {
            if (rb != null)
            {
                rb.simulated = false;
            }
        }
    }
}
