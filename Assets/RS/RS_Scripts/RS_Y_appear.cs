using UnityEngine;
using System.Collections;

namespace RS
{
    public class RS_Y_appear : MiniGameBase 
    {
        [Header("フェード設定")]
        public float duration = 1.5f;
        public float waitTime = 0.5f;

        private SpriteRenderer spriteRenderer;

        public override void OnGameStart()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            StartCoroutine(FadeSequence());
        }

        private IEnumerator FadeSequence()
        {
            yield return StartCoroutine(Fade(0f, 1f));

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(Fade(1f, 0f));
        }

        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                SetAlpha(currentAlpha);
                yield return null;
            }
            SetAlpha(endAlpha);
        }

        private void SetAlpha(float alpha)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
            }
        }
    }
}