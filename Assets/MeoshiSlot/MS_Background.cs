using UnityEngine;
using UnityEngine.UI;

namespace MeoshiSlotGame_IK
{
    [RequireComponent(typeof(RawImage))]
    public class BackgroundScroller : MiniGameBase
    {
        [Header("【スクロール設定】")]
        [Tooltip("横方向の速さ")]
        [SerializeField] private float scrollSpeedX = 0.05f;
        
        [Tooltip("縦方向の速さ")]
        [SerializeField] private float scrollSpeedY = 0.08f;

        [Header("【見た目の調整】")]
        [Tooltip("【重要】これをONにすると、画面サイズに合わせて自動で形を整えます")]
        [SerializeField] private bool fixAspectRatio = true;

        [Tooltip("パターンの細かさ（Xだけ設定すれば、Yは自動計算されます）")]
        [SerializeField] private float tilingX = 3f;

        // 手動調整用の変数（fixAspectRatioがOFFの時だけ使われます）
        [SerializeField, HideInInspector] private float manualTilingY = 3f;

        [Header("【色変化（ゲーミング発光）】")]
        [SerializeField] private bool useRainbowEffect = false;
        [SerializeField] private float colorChangeSpeed = 0.2f;

        private RawImage rawImage;
        private float currentHue = 0f;

        // Start() は禁止されているため、OnGameStart() に変更
        public override void OnGameStart()
        {
            rawImage = GetComponent<RawImage>();
            UpdateTiling();
        }
        
        // ゲーム終了時（必要なら実装）
        public override void OnGameEnd()
        {
            // 特になし
        }

        void UpdateTiling()
        {
            if (rawImage == null) rawImage = GetComponent<RawImage>();
            if (rawImage == null || rawImage.texture == null) return;

            float finalTilingY = manualTilingY;

            // ★ここが自動補正の計算式
            if (fixAspectRatio)
            {
                // 画面のアスペクト比（横÷縦）
                float screenRatio = (float)Screen.width / Screen.height;
                
                // 画像自体の比率（正方形なら1.0）
                float textureRatio = (float)rawImage.texture.width / rawImage.texture.height;

                // 画面が横長になればなるほど、縦の繰り返し回数を減らす調整
                // 計算式: Yタイリング = Xタイリング * (画面の縦 / 画面の横) * 画像比率
                finalTilingY = tilingX * (1f / screenRatio) * textureRatio;
            }
            else
            {
                // 自動補正OFFなら、設定した値をそのまま使う（手動調整用）
                finalTilingY = tilingX; 
            }

            Rect currentUV = rawImage.uvRect;
            currentUV.width = tilingX;
            currentUV.height = finalTilingY;
            rawImage.uvRect = currentUV;
        }

        void Update()
        {
            // rawImageが取得できていない場合の安全策
            if (rawImage == null) rawImage = GetComponent<RawImage>();

            // 画面サイズが動的に変わる場合のために毎回チェック
            if (fixAspectRatio) UpdateTiling();

            // ▼ スクロール処理 ▼
            Rect uv = rawImage.uvRect;
            
            // 仕様書対応：Time.deltaTime は Time.timeScale の影響を受けるため、
            // ゲーム速度が上がればスクロールも速くなります。
            uv.x += scrollSpeedX * Time.deltaTime;
            uv.y += scrollSpeedY * Time.deltaTime;
            
            // UVのセット
            // width/heightはUpdateTilingでセットされているので、ここでは維持される
            rawImage.uvRect = uv;

            // ▼ 色変化 ▼
            if (useRainbowEffect)
            {
                currentHue += colorChangeSpeed * Time.deltaTime;
                if (currentHue > 1.0f) currentHue -= 1.0f;
                // 色変更
                rawImage.color = Color.HSVToRGB(currentHue, 0.5f, 1.0f);
            }
        }
        
        // インスペクターで値をいじった時に即座に反映させる処理
        // OnValidate は Editor上でのみ呼ばれるので、Start禁止ルールには抵触しません
        void OnValidate()
        {
            rawImage = GetComponent<RawImage>();
            UpdateTiling();
        }
    }
}