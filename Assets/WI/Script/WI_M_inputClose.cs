using UnityEngine;

namespace WI 
{

    public class WI_M_inputClose : MiniGameBase
    {
        private WI_M_buttonManager rootManager;

        private BoxCollider2D cursorCollider;
        private RaycastHit2D rayHit;

        private bool cursorCollision;

        private int myParentId;
        private int coveringObjectId;
        private bool isCovered;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            rootManager = this.transform.parent.GetComponent<WI_M_buttonManager>();

            cursorCollider = GameObject.Find("WI_M_cursor").GetComponent<BoxCollider2D>();
            cursorCollision = false;

            //myParentId = getWindowIndex(this.transform.parent.gameObject);
            coveringObjectId = 1000;
            isCovered = false;
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {

            if (cursorCollision)
            {
                if (isTopWindowFromCursor())
                {
                    Debug.Log("push" + this.GetComponent<SpriteRenderer>().sortingOrder);
                    if (Action.WasPerformedThisFrame())
                    {
                        Debug.Log("top" + this.GetComponent<SpriteRenderer>().sortingOrder);
                        rootManager.setInputClose();

                    }
                }
            }
            
                
                 

        }

        private bool isTopWindowFromCursor()
        {
            Vector2 cursorPosition = new Vector2(cursorCollider.transform.position.x, cursorCollider.transform.position.y);

            // ç¿ïWposÇ…Ç†ÇÈëSÇƒÇÃCollider2DéÊìæ
            RaycastHit2D[] hits = Physics2D.RaycastAll(cursorPosition, Vector2.zero);

            int highestOrder = int.MinValue;
            GameObject topWindow = null;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag != "window") continue;

                SpriteRenderer renderer = hit.collider.GetComponent<SpriteRenderer>();

                if (renderer == null) continue;

                if(renderer.sortingOrder > highestOrder)
                {
                    highestOrder = renderer.sortingOrder;
                    topWindow = hit.collider.gameObject;
                }
            }
            return topWindow == this.transform.parent.gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == cursorCollider)
            {
                cursorCollision = true;

            }
            else
            {
                //nop
            }
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == cursorCollider)
            {
                cursorCollision = false;

            }
            else
            {
                //nop
            }
        }
    }
}

