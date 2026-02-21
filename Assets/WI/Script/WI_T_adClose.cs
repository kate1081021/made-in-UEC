using UnityEngine;

namespace WI
{
    public class WI_T_adClose : MiniGameBase
    {
        private BoxCollider2D cursorCollider;
        private bool isCursorHovering = false;
        private WI_T_advertisement parentWindow;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            GameObject cursorObj = GameObject.Find("WI_M_cursor");
            if (cursorObj != null)
            {
                cursorCollider = cursorObj.GetComponent<BoxCollider2D>();
            }

            parentWindow = this.transform.parent.GetComponent<WI_T_advertisement>();
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {
            if (isCursorHovering && Action.WasPerformedThisFrame())
            {
                if (parentWindow != null)
                {
                    parentWindow.setInputClose();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (cursorCollider != null && collision == cursorCollider)
            {
                isCursorHovering = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (cursorCollider != null && collision == cursorCollider)
            {
                isCursorHovering = false;
            }
        }
    }
}
