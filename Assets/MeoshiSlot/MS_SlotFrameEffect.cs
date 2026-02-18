using UnityEngine;
using UnityEngine.UI;

namespace MeoshiSlotGame_IK
{
    public class SlotFrameEffect : MiniGameBase
    {
        [Header("【リズム設定】")]
        [SerializeField] private float bpm = 160f;       // BPM160

        [Header("【演出1：ビート振動】")]
        [SerializeField] private bool enableBeatPulse = true; // ビートに合わせて動くか
        [SerializeField] private float pulseScale = 1.02f;    // ビート時の拡大率
        [SerializeField] private float smoothness = 15f;      // 元に戻る速さ

        [Header("【演出2：ゲーミング発光】")]
        [SerializeField] private bool enableRainbow = true;   // 七色に光らせるか
        [SerializeField] private float rainbowSpeed = 0.5f;   // 色が変わる速さ
        [SerializeField, Range(0f, 1f)] private float saturation = 0.8f; // 色の鮮やかさ
        [SerializeField, Range(0f, 1f)] private float brightness = 1.0f; // 明るさ

        // 内部変数
        private Image targetImage;
        private Vector3 initialScale;
        private float beatInterval;
        private float timer;
        private float hue; 

        public override void OnGameStart()
        {
            targetImage = GetComponent<Image>();
            
            // ピボットを中心にしておくと綺麗に拡大します
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.pivot = new Vector2(0.5f, 0.5f);
            }

            initialScale = transform.localScale;
            beatInterval = 60f / bpm;
            timer = beatInterval; // 1拍目から作動させる
        }

        public override void OnGameEnd()
        {
            // 終了時は元のサイズ・色（白）に戻す
            transform.localScale = initialScale;
            if (targetImage != null) targetImage.color = Color.white;
        }

        void Update()
        {
            // ▼▼▼ 1. ビート振動の処理 ▼▼▼
            if (enableBeatPulse)
            {
                timer += Time.deltaTime;
                if (timer >= beatInterval)
                {
                    timer -= beatInterval;
                    // ビートの瞬間に拡大
                    transform.localScale = initialScale * pulseScale;
                }
                // 滑らかに戻る
                transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * smoothness);
            }

            // ▼▼▼ 2. ゲーミングレインボーの処理 ▼▼▼
            if (enableRainbow && targetImage != null)
            {
                // 時間経過で色相(Hue)を回転させる
                hue += Time.deltaTime * rainbowSpeed;
                if (hue > 1.0f) hue -= 1.0f;

                // HSVからRGBに変換して適用
                targetImage.color = Color.HSVToRGB(hue, saturation, brightness);
            }
        }
    }
}