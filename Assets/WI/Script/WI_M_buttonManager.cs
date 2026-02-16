using UnityEngine;

namespace WI
{

    public class WI_M_buttonManager : MiniGameBase
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            this.gameObject.SetActive(true);
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {

        }

        public void setInputClose()
        {
            this.gameObject.SetActive(false);
        }
    }
}

