using UnityEngine;
using System.Collections;

namespace WI
{
    public class WI_I_window_virus_buster : MiniGameBase
    {
        private bool isClosing = false;

        public override void OnGameStart()
        {
            this.gameObject.SetActive(true);
            StartCoroutine(AnimateIn());
        }

        private IEnumerator AnimateIn()
        {
            float duration = 0.5f;
            float elapsed = 0f;

            Vector3 targetPos = transform.position;
            Vector3 targetScale = transform.localScale;

            Vector3 startPos = new Vector3(targetPos.x, targetPos.y - 8.0f, targetPos.z);

            transform.position = startPos;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / duration;
                float curve = 1f - Mathf.Pow(1f - percent, 4);

                transform.position = Vector3.Lerp(startPos, targetPos, curve);        

                yield return null;
            }

            transform.position = targetPos;
            transform.localScale = targetScale;
        }

        public void CloseWindow()
        {
            if (isClosing) return;
            StartCoroutine(AnimateAndDestroy());
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
        }

        public override void OnGameEnd() { }
    }
}