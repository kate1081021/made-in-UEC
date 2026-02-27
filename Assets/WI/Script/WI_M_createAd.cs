using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace WI
{

    public class WI_M_createAd : MiniGameBase
    {
        private BoxCollider2D cursorCollider;

        private bool cursorCollision;

        private bool isCreate = false;

        GameObject advertisement;
        BoxCollider2D filter;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            advertisement = GameObject.Find("WI_M_copyAd");
            filter = GameObject.Find("WI_M_gameOverFilter").GetComponent<BoxCollider2D>();
            filter.transform.position = Vector3.zero;
            filter.enabled = false;
            cursorCollider = GameObject.Find("WI_M_cursor").GetComponent<BoxCollider2D>();
            cursorCollision = false;
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {
            if (Action.WasPerformedThisFrame())
            {
                Debug.Log("push");
                if (isTopWindowFromCursor())
                {
                    Debug.Log("top");
                    if (cursorCollision)
                    {
                        Debug.Log("act");
                        gameOver();
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

        private void gameOver()
        {
            Debug.Log("game over");
            filter.enabled = true;

            if (!isCreate)
            {
                StartCoroutine(CreateNewAds());
            }
            /*for (int i = 1; i < 51; i++)
            {
                randomX = Random.Range(0f, 1f);
                randomY = Random.Range(0f, 1f);
                adPosition = Camera.main.ViewportToWorldPoint(new Vector2(randomX, randomY));
                newAd = Instantiate(advertisement, adPosition, Quaternion.identity);
                newAd.GetComponent<SortingGroup>().sortingOrder = this.transform.parent.parent.GetComponent<SpriteRenderer>().sortingOrder + i;
            }*/

        }
        private IEnumerator CreateNewAds()
        {
            isCreate = true;

            float duration = 1.0f;
            float elapsed = 0f;

            float randomX, randomY;
            int sr = this.transform.parent.parent.GetComponent<SpriteRenderer>().sortingOrder;
            Vector2 adPosition;
            GameObject newAd;

            while (elapsed < duration)
            {
                Debug.Log(elapsed);
                elapsed += Time.deltaTime;
                randomX = Random.Range(0f, 1f);
                randomY = Random.Range(0f, 1f);
                adPosition = Camera.main.ViewportToWorldPoint(new Vector2(randomX, randomY));
                newAd = Instantiate(advertisement, adPosition, Quaternion.identity);
                newAd.GetComponent<SortingGroup>().sortingOrder = ++sr;

                yield return null;
            }
        }
    }
}
