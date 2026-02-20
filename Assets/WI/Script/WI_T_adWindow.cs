using UnityEngine;

namespace WI
{

    public class WI_T_adWindow : MiniGameBase
    {
        
        public GameObject parentCloseButton;
        public GameObject adWindow;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            if (parentCloseButton != null)
            {
                parentCloseButton.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {
            if (adWindow == null)
            {
                if (parentCloseButton != null)
                {
                    parentCloseButton.GetComponent<BoxCollider2D>().enabled = true;
                }
                this.enabled = false;
            }
        }
    }
}

