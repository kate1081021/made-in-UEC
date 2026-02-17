using UnityEngine;

namespace WI 
{

    public class WI_M_inputClose : MiniGameBase 
    {
        private WI_M_buttonManager rootManager;

        private BoxCollider2D cursorCollider;

        private bool cursorCollision;
        private bool isCovered;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            rootManager = transform.parent.GetComponent<WI_M_buttonManager>();
            cursorCollider = GameObject.Find("WI_M_cursor").GetComponent<BoxCollider2D>();
            cursorCollision = false;
            isCovered = false;
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update() 
        {
            if (cursorCollision)
            {
                if (Action.WasPerformedThisFrame())
                {
                    rootManager.setInputClose();
                }
                else
                {
                    // nop
                }
            }
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject != transform.parent)
            {
                if (collision.bounds.Contains(this.gameObject.transform.position))
                {
                    //Debug.Log(collision.gameObject.GetComponent<SpriteRenderer>().sprite);
                    //Debug.Log(isCovered);
                    if (collision.gameObject.tag == "window")
                    {
                        isCovered = true;
                    }
                }
                if (collision == cursorCollider)
                {
                    cursorCollision = true;

                }
                else
                {
                    //nop
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "window")
            {
                isCovered = false;
            }
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

