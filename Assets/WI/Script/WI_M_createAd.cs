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

        // SE
        private AudioSource audioSource;
        [SerializeField] AudioClip popSE;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            if((audioSource = this.GetComponent<AudioSource>()) == null)
            {
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

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
                if (isTopWindowFromCursor())
                {
                    if (cursorCollision)
                    {
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
        }
        private IEnumerator CreateNewAds()
        {
            isCreate = true;

            float duration = 1.0f;
            float elapsed = 0f;

            float randomX, randomY;
            int sr = this.transform.parent.parent.GetComponent<SortingGroup>().sortingOrder;
            Vector2 adPosition;
            GameObject newAd;

            while (elapsed < duration)
            {
                // SEPlay("popup", false);
                soundPlay(audioSource, popSE);
                elapsed += Time.deltaTime;
                randomX = Random.Range(0f, 1f);
                randomY = Random.Range(0f, 1f);
                adPosition = Camera.main.ViewportToWorldPoint(new Vector2(randomX, randomY));
                newAd = Instantiate(advertisement, adPosition, Quaternion.identity);
                newAd.GetComponent<SortingGroup>().sortingOrder = ++sr;

                yield return null;
            }
        }

        private void soundPlay(AudioSource audioSource, AudioClip audioClip)
        {
            if (audioSource != null && audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.PlayOneShot(audioClip);
            }
        }
    }
}
