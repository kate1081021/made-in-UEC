using UnityEngine;
using UnityEngine.Rendering;

namespace WI
{

    public class WI_M_createPop : MiniGameBase
    {
        private WI_M_popParentManager rootManager;

        private BoxCollider2D cursorCollider;

        private bool cursorCollision;

        GameObject popup;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            rootManager = this.transform.parent.GetComponent<WI_M_popParentManager>();

            cursorCollider = GameObject.Find("WI_M_cursor").GetComponent<BoxCollider2D>();
            cursorCollision = false;
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {
            if (Action.WasPerformedThisFrame())
            {
                if (isTopWindowFromCursor())
                {
                    if (cursorCollision)
                    {
                        popup = GameObject.Find("WI_M_popup");
                        popup = Instantiate(popup, this.transform.parent.position, Quaternion.identity);
                        popup.GetComponent<SortingGroup>().sortingOrder = this.transform.parent.GetComponent<SortingGroup>().sortingOrder + 1;
                        popup.transform.parent = this.transform.parent;
                        rootManager.registerPopup(popup);
                        rootManager.colorChange(new Color(0.5f, 0.5f, 0.5f, 1.0f));
                        rootManager.buttonDisactivate();
                        rootManager.buttonTypeChange();
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

            // ç¿ïWposÇ…Ç†ÇÈëSÇƒÇÃCollider2DéÊìæ
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
            if (topWindow != null) Debug.Log(topWindow.name);
            return topWindow == this.transform.parent.gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == cursorCollider)
            {
                cursorCollision = true;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == cursorCollider)
            {
                cursorCollision = false;
            }
        }
    }
}

