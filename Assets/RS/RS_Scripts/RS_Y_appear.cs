using UnityEngine;
using System.Collections;

namespace RS
{
    public class RS_Y_appear : MiniGameBase 
    {
        public float duration = 1.5f;
        public float waitTime = 0.5f;
        public KeyCode targetKey = KeyCode.Alpha1; // これをSpawn側が読み取る

        private SpriteRenderer spriteRenderer;

        public void Setup()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetAlpha(0f);
            StartCoroutine(FadeSequence());
        }

        public float GetCurrentAlpha()
        {
            return spriteRenderer != null ? spriteRenderer.color.a : 0f;
        }

        private IEnumerator FadeSequence()
        {
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
                SetAlpha(Mathf.Lerp(start, end, elapsedTime / duration));
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
        }
        
        public override void OnGameStart() {}
    }
}