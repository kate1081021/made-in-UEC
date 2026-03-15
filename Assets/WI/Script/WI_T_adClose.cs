using UnityEngine;
using UnityEngine.Rendering;

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
                if (isTopWindowFromCursor())
                {
                    if (parentWindow != null)
                    {
                        parentWindow.setInputClose();
                    }
                }
            }
        }

        private bool isTopWindowFromCursor()
        {
            SpriteRenderer cursorPos = cursorCollider.GetComponent<SpriteRenderer>();
            Vector2 cursorPosition = new Vector2(cursorPos.bounds.min.x + 0.06f,
                                                 cursorPos.bounds.max.y);

            SortingGroup renderer;

            // ���Wpos�ɂ���S�Ă�Collider2D�擾
            RaycastHit2D[] hits = Physics2D.RaycastAll(cursorPosition, Vector2.zero);

            int highestOrder = int.MinValue;
            GameObject topWindow = null;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag != "window") continue;

                renderer = hit.collider.GetComponent<SortingGroup>();

                if (renderer == null) continue;

                if (renderer.sortingOrder > highestOrder)
                {
                    highestOrder = renderer.sortingOrder;
                    topWindow = hit.collider.gameObject;
                }
            }
            //if (topWindow != null) Debug.Log(topWindow.name);
            return topWindow == this.transform.parent.gameObject;
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
