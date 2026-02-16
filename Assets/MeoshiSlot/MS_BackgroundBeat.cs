using UnityEngine;

namespace MeoshiSlotGame_IK
{
    public class BackgroundBeat : MiniGameBase
    {
        [Header("【設定】")]
        [SerializeField] private float bpm = 120f;       // 曲のBPM
        [SerializeField] private float power = 1.05f;    // 拡大率（1.05 = 5%拡大）
        [SerializeField] private float smoothness = 10f; // 元に戻る速さ

        // 内部変数
        private Vector3 initialScale;
        private float beatInterval;
        private float timer;

        public override void OnGameStart()
        {
            // ▼▼▼ 修正ポイント：ピボット（中心点）を強制的に画面中央にする ▼▼▼
            // これで「左と右で動きが違う」現象が直り、真ん中から均等に広がります
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.pivot = new Vector2(0.5f, 0.5f);
            }

            // 最初の大きさを記憶
            initialScale = transform.localScale;
            
            beatInterval = 60f / bpm;

            // 1拍目からドンッと鳴らすために、タイマーを満タンにしておく
            timer = beatInterval;
        }

        public override void OnGameEnd()
        {
            // サイズを元に戻す
            transform.localScale = initialScale;
        }

        void Update()
        {
            timer += Time.deltaTime;

            // ビートのタイミングが来たら
            if (timer >= beatInterval)
            {
                timer -= beatInterval;
                // 一瞬だけ大きくする（ドンッ！）
                transform.localScale = initialScale * power;
            }

            // 常に滑らかに元のサイズに戻ろうとする
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * smoothness);
        }
    }
}