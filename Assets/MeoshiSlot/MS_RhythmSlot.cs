using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace MeoshiSlotGame_IK
{
    public class RhythmSlot : MiniGameBase
    {
        [Header("【リールの親オブジェクト】")]
        [SerializeField] private RectTransform reel1Parent;
        [SerializeField] private RectTransform reel2Parent;
        [SerializeField] private RectTransform reel3Parent;

        [Header("【絵柄の元データ】")]
        [SerializeField] private Sprite[] sourceSprites;

        [Header("【基本設定】")]
        [SerializeField] private float baseBpm = 160f; 
        [SerializeField] private int winSpriteIndex = 0;
        [SerializeField] private int symbolsPerBeat = 4;
        
        [Header("【入力ガード設定】")]
        [Tooltip("一度止めてから次に反応するまでの最短時間(秒)")]
        [SerializeField] private float inputCooldown = 0.2f;

        [Header("【演出調整】")]
        [SerializeField] private ParticleSystem winParticle; 
        [SerializeField] private float particleStartSize = 50f; 

        [Header("【見た目の調整】")]
        [SerializeField] private float cellHeight = 250f;
        [SerializeField] private float visualOffsetY = 0f;

        [Header("【サイズ・エフェクト設定】")]
        [SerializeField, Range(0.1f, 2.0f)] private float stoppedScale = 1.0f;
        [SerializeField, Range(0.1f, 2.0f)] private float movingScale = 0.7f;
        [SerializeField, Range(1.0f, 3.0f)] private float movingBlur = 1.5f;
        [SerializeField] private bool enableSymbolBeat = true;
        [SerializeField] private float beatPulseScale = 1.2f;
        [SerializeField] private float beatSmoothness = 10f;
        [SerializeField] private bool enableSymbolRainbow = false;
        [SerializeField] private float rainbowSpeed = 0.5f;

        [Header("【音源設定】")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioClip stopSE;
        [SerializeField] private AudioClip winSE;
        [SerializeField] private AudioClip failSE;

        [Header("【デバッグ（提出時はOFF）】")]
        [SerializeField] private bool isDebugMode = false;
        [SerializeField] private int debugStageIndex = 1;

        private RectTransform[][] reelImages;
        private bool[] isStopped = new bool[3];
        private int currentReelIndex = 0;
        private float[] reelPositions = new float[3];
        private float[] bounceOffsets = new float[3];
        private float[] bounceScales = new float[3]; 
        
        private float currentBeatScaleAdd = 0f;
        private float beatTimer;
        private float beatInterval;
        private float rainbowHue;
        private float startTime;
        private bool isClear = false;
        private float lastInputTime = -1f;

        // 仕様書：Start()は使用禁止のため OnGameStart で初期化
        public override void OnGameStart()
        {
            if (isDebugMode) MGManager.TestPlay(debugStageIndex);

            MGManager.Load(); // 開始時に必須
            SetupReelImages();

            startTime = Time.time;
            beatInterval = 60f / baseBpm;
            beatTimer = beatInterval;

            for(int i = 0; i < 3; i++) 
            {
                bounceOffsets[i] = 0f;
                bounceScales[i] = 0f;
                isStopped[i] = false;
            }

            if (bgmSource != null)
            {
                bgmSource.loop = false;
                bgmSource.Play();
            }

            if (winParticle != null)
            {
                var main = winParticle.main;
                main.useUnscaledTime = false; // Time.timeScaleに準拠
                main.startSize = particleStartSize;
                main.scalingMode = ParticleSystemScalingMode.Local;
            }
        }

        public override void OnGameEnd()
        {
            if (bgmSource != null) bgmSource.Stop();
            StopAllCoroutines();
        }

        private void SetupReelImages()
        {
            reelImages = new RectTransform[3][];
            reelImages[0] = GetImagesFromParent(reel1Parent);
            reelImages[1] = GetImagesFromParent(reel2Parent);
            reelImages[2] = GetImagesFromParent(reel3Parent);
        }

        private RectTransform[] GetImagesFromParent(RectTransform parent)
        {
            List<RectTransform> list = new List<RectTransform>();
            foreach (Transform child in parent)
            {
                if (child.GetComponent<Image>() != null)
                {
                    RectTransform rt = child.GetComponent<RectTransform>();
                    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.sizeDelta = new Vector2(cellHeight, cellHeight);
                    list.Add(rt);
                }
            }
            return list.ToArray();
        }

        private void Update()
        {
            // Time.timeScaleに合わせてBGMピッチを同期
            if (bgmSource != null) bgmSource.pitch = Time.timeScale;

            if (enableSymbolBeat)
            {
                beatTimer += Time.deltaTime;
                if (beatTimer >= beatInterval)
                {
                    beatTimer -= beatInterval;
                    currentBeatScaleAdd = (beatPulseScale - 1.0f);
                }
                currentBeatScaleAdd = Mathf.Lerp(currentBeatScaleAdd, 0f, Time.deltaTime * beatSmoothness);
            }

            if (enableSymbolRainbow && !isClear)
            {
                rainbowHue += Time.deltaTime * rainbowSpeed;
                if (rainbowHue > 1.0f) rainbowHue -= 1.0f;
            }

            // 入力処理（クールタイムによる2列停止ガード）
            if (!isClear)
            {
                // 仕様書：Action.WasPerformedThisFrame() を使用
                if (Action.WasPerformedThisFrame() || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    if (Time.unscaledTime - lastInputTime > inputCooldown)
                    {
                        OnPressButton();
                        lastInputTime = Time.unscaledTime;
                    }
                }
            }

            float elapsedTime = Time.time - startTime;

            for (int i = 0; i < 3; i++)
            {
                if (!isStopped[i])
                {
                    // Time.timeScaleの影響を受ける移動計算
                    float currentBeatProgress = elapsedTime / beatInterval;
                    reelPositions[i] = (currentBeatProgress * symbolsPerBeat + winSpriteIndex) * cellHeight;
                }
                
                bounceOffsets[i] = Mathf.Lerp(bounceOffsets[i], 0, Time.deltaTime * 10f);
                bounceScales[i] = Mathf.Lerp(bounceScales[i], 0, Time.deltaTime * 8f);

                UpdateReelVisuals(i, reelPositions[i] + bounceOffsets[i]);
            }
        }

        private void UpdateReelVisuals(int reelID, float scrollPos)
        {
            RectTransform[] images = reelImages[reelID];
            int count = images.Length;
            float totalHeight = count * cellHeight;

            float currentBlurY = isStopped[reelID] ? 1.0f : movingBlur;
            float baseScale = isStopped[reelID] ? stoppedScale : movingScale;

            for (int j = 0; j < count; j++)
            {
                RectTransform rt = images[j];
                float basePosY = (2 - j) * cellHeight;
                float currentY = basePosY - (scrollPos % totalHeight);

                if (currentY < -(totalHeight / 2)) currentY += totalHeight;
                else if (currentY > (totalHeight / 2)) currentY -= totalHeight;

                rt.anchoredPosition = new Vector2(0, currentY + visualOffsetY);

                float dist = Mathf.Abs(rt.anchoredPosition.y - visualOffsetY);
                float perspective = Mathf.Lerp(1.0f, 0.6f, dist / cellHeight);

                float finalScale = (baseScale + bounceScales[reelID] + currentBeatScaleAdd) * perspective;
                rt.localScale = new Vector3(finalScale, finalScale * currentBlurY, 1f);

                float virtualIndex = (scrollPos + currentY) / cellHeight;
                int spriteId = Mathf.RoundToInt(virtualIndex);
                spriteId = (-(spriteId - 2) % sourceSprites.Length + sourceSprites.Length) % sourceSprites.Length;

                Image img = rt.GetComponent<Image>();
                img.sprite = sourceSprites[spriteId];

                if (!isClear)
                {
                    img.color = enableSymbolRainbow ? Color.HSVToRGB(rainbowHue, 0.7f, 1.0f) : Color.white;
                }
            }
        }

        private void OnPressButton()
        {
            if (currentReelIndex >= 3) return; 

            if (seSource != null && stopSE != null) seSource.PlayOneShot(stopSE);

            isStopped[currentReelIndex] = true;

            Image centerImg = GetCenterImage(currentReelIndex);
            if (centerImg != null)
            {
                float diffY = centerImg.rectTransform.anchoredPosition.y - visualOffsetY;
                reelPositions[currentReelIndex] += diffY;
            }

            bounceOffsets[currentReelIndex] = -50f;
            bounceScales[currentReelIndex] = 0.3f; 

            currentReelIndex++;
            if (currentReelIndex >= 3) CheckWin();
        }

        private void CheckWin()
        {
            string[] spriteNames = new string[3];
            for (int i = 0; i < 3; i++)
            {
                Image img = GetCenterImage(i);
                if (img != null) spriteNames[i] = img.sprite.name;
            }

            if (spriteNames[0] == spriteNames[1] && spriteNames[1] == spriteNames[2])
            {
                isClear = true;
                if (seSource != null && winSE != null) seSource.PlayOneShot(winSE);
                if (winParticle != null) winParticle.Play();

                Image[] centerImages = { GetCenterImage(0), GetCenterImage(1), GetCenterImage(2) };
                StartCoroutine(RainbowEffect(centerImages));
                
                MGManager.ClearGame(); // 仕様書通りのクリア報告
            }
            else
            {
                if (seSource != null && failSE != null) seSource.PlayOneShot(failSE);
                StartCoroutine(DelayRestart(0.5f));
            }
        }

        private IEnumerator DelayRestart(float delayTime)
        {
            yield return new WaitForSecondsRealtime(delayTime);
            RestartReels();
        }

        private void RestartReels()
        {
            if (isClear) return; 
            currentReelIndex = 0;
            lastInputTime = -1f; 

            for (int i = 0; i < 3; i++)
            {
                isStopped[i] = false;
                bounceOffsets[i] = 0f;
                bounceScales[i] = 0f;
                foreach (var rt in reelImages[i]) rt.GetComponent<Image>().color = Color.white;
            }
        }

        private Image GetCenterImage(int reelID)
        {
            float minDistance = float.MaxValue;
            Image closestImage = null;
            foreach (var rt in reelImages[reelID])
            {
                float distance = Mathf.Abs(rt.anchoredPosition.y - visualOffsetY);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestImage = rt.GetComponent<Image>();
                }
            }
            return closestImage;
        }

        private IEnumerator RainbowEffect(Image[] targets)
        {
            float h = 0;
            while (true)
            {
                h += Time.deltaTime * 3f;
                Color c = Color.HSVToRGB(h % 1f, 1f, 1f);
                foreach (var img in targets) if (img != null) img.color = c;
                yield return null;
            }
        }
    }
}