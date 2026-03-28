using UnityEngine;
using System.Collections;
using TMPro;

namespace RS
{
    public class RS_appear : MiniGameBase
    {
        public float startTime = 1.0f;
        public float duration = 1.5f;
        public float waitTime = 0.5f;
        public int aciton_type;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshPro textMeshPro;

        public void Setup()
        {
            startTime/=Time.timeScale;
            duration/=Time.timeScale;
            waitTime/=Time.timeScale;
            SetAlpha(0f);
            StartCoroutine(FadeSequence());
        }

        public float GetCurrentAlpha()
        {
            return spriteRenderer != null ? spriteRenderer.color.a : 0f;
        }

        private IEnumerator FadeSequence()
        {
            yield return new WaitForSeconds(startTime);
            yield return StartCoroutine(Fade(0f, 1f));
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(Fade(1f, 0f));
        }

        private IEnumerator Fade(float start, float end)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                SetAlpha(Mathf.Lerp(start, end, elapsedTime / duration * elapsedTime / duration));
                yield return null;
            }
            SetAlpha(end);
        }

        private void SetAlpha(float alpha)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
            }
            if (textMeshPro != null)
            {
                Color c = textMeshPro.color;
                c.a = alpha;
                textMeshPro.color = c;
            }
        }

        public void HideImmediately()
        {
            StopAllCoroutines();
            SetAlpha(0f);
        }

        public override void OnGameStart() { }
    }
}