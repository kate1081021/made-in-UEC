using UnityEngine;

namespace WI
{
    public class WI_S_afterclearbackground : MiniGameBase
    {
        [SerializeField] private float fadeDuration = 0.5f; //0.5秒で変化
        [SerializeField] private Color nightColor = new Color(0.2f, 0.3f, 0.6f, 1f); //夜の色

        private SpriteRenderer sr;
        private Color startColor;
        private float timer = 0f;
        private bool isFading = false;


        public override void OnGameStart()
        {
            sr = GetComponent<SpriteRenderer>();
            startColor = sr.color; //昼の色を記録
        }

        public override void OnGameEnd()
        {
            timer = 0f;
            isFading = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isFading) return;
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            sr.color = Color.Lerp(startColor, nightColor, t);
        }
    }
}
