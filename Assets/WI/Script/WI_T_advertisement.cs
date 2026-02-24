using UnityEngine;
using System.Collections;

namespace WI
{
    public class WI_T_advertisement : MiniGameBase 
    {
        private bool isClosing = false;
        [SerializeField] private bool useAnimation = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart() { }

        public override void OnGameEnd() { }
        void Update() { }

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
                CloseWindow();
            }
        }

        public void CloseWindow()
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
    }
}
