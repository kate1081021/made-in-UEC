using UnityEngine;
using System.Collections;

namespace WI
{

    public class WI_M_popParentManager : MiniGameBase
    {
        private GameObject closeButton;
        private GameObject popupWindow;

        private bool createPopup = false;

        private bool isClosing = false;

        [SerializeField] private bool useAnimation = true;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            closeButton = this.transform.GetChild(0).gameObject;
            closeButton.GetComponent<WI_M_inputClose>().enabled = false;
            closeButton.GetComponent<WI_M_createPop>().enabled = true;

            this.gameObject.SetActive(true);
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {
            if (createPopup)
            {
                if (popupWindow == null)
                {
                    // もう一度Closeボタンを押させる場合
                    // buttonActivate();
                    
                    // ポップアップ削除と同時に消える場合
                    this.setInputClose();
                }
            }
        }

        public void setInputClose()
        {
            if (isClosing) return;
            StartCoroutine(AnimateAndDestroy());

            if (useAnimation)
            {
                StartCoroutine(AnimateAndDestroy());
            }
            else
            {
                isClosing = true;
                DestroyImmediate();
            }
        }

        private void DestroyImmediate()
        {
            Destroy(this.gameObject);
        }

        private IEnumerator AnimateAndDestroy()
        {
            isClosing = true;

            float duration = 0.15f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / duration;

                float curve = 1f - percent;
                transform.localScale = new Vector3(startScale.x * curve, startScale.y * curve, startScale.z);

                yield return null;

            }

            Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }

        public void registerPopup(GameObject popup)
        {
            this.popupWindow = popup;
        }
        public void buttonTypeChange()
        {
            closeButton.GetComponent<WI_M_inputClose>().enabled = true;
            closeButton.GetComponent<WI_M_createPop>().enabled = false;
            createPopup = true;
        }

        public void buttonDisactivate()
        {
            closeButton.GetComponent<BoxCollider2D>().enabled = false;
        }
        public void buttonActivate()
        {
            closeButton.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}

