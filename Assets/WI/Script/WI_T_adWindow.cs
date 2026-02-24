using UnityEngine;

namespace WI
{

    public class WI_T_adWindow : MiniGameBase
    {
        
        public GameObject parentCloseButton;
        public GameObject adWindow;

        public Color darkerColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private SpriteRenderer myRenderer;
        private SpriteRenderer myButtonRenderer;
        private Color originalColor = Color.white;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            myRenderer = GetComponent<SpriteRenderer>();
            myButtonRenderer = parentCloseButton.GetComponent<SpriteRenderer>();
            myRenderer.color = myButtonRenderer.color = darkerColor;

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
                myRenderer.color = myButtonRenderer.color = originalColor;

                if (parentCloseButton != null)
                {
                    parentCloseButton.GetComponent<BoxCollider2D>().enabled = true;
                }
                this.enabled = false;
            }
        }
    }
}

