using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

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
        [SerializeField] private float baseBpm = 120f;
        [SerializeField] private int winSpriteIndex = 0;
        [SerializeField] private int symbolsPerBeat = 4;
        
        [Header("【見た目の調整】")]
        [SerializeField] private float cellHeight = 250f;
        [SerializeField] private float visualOffsetY = 0f;

        [Header("【サイズ調整】")]
        [SerializeField, Range(0.1f, 2.0f)] private float stoppedScale = 1.0f; // 停止時の基本サイズ
        [SerializeField, Range(0.1f, 2.0f)] private float movingScale = 0.7f;  // 回転中のサイズ
        [SerializeField, Range(1.0f, 3.0f)] private float movingBlur = 1.5f;

        [Header("【演出用】")]
        [SerializeField] private ParticleSystem winParticle; 

        [Header("【音源設定】")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioClip stopSE;
        [SerializeField] private AudioClip winSE;
        [SerializeField] private AudioClip failSE;

        [Header("【デバッグ（本番時はOFFにすること）】")]
        [SerializeField] private bool isDebugMode = false;
        [SerializeField] private int debugStageIndex = 1;

        // 内部変数
        private RectTransform[][] reelImages;
        private bool[] isStopped = new bool[3];
        private int currentReelIndex = 0;
        private float[] reelPositions = new float[3];
        
        // 演出用変数
        private float[] bounceOffsets = new float[3]; // Y座標の揺れ
        private float[] bounceScales = new float[3];  // ★追加：大きさの揺れ（拡大演出用）

        private float startTime;
        private bool isClear = false;

        public override void OnGameStart()
        {
            if (isDebugMode)
            {
                Debug.Log($"【Debug】ステージ {debugStageIndex} の速度でテストプレイを開始します。");
                MGManager.TestPlay(debugStageIndex);
            }

            MGManager.Load();
            SetupReelImages();

            startTime = Time.time;

            for(int i=0; i<3; i++) 
            {
                bounceOffsets[i] = 0f;
                bounceScales[i] = 0f; // 初期化
            }

            if (bgmSource != null)
            {
                bgmSource.loop = true;
                bgmSource.Play();
            }

            if (winParticle != null)
            {
                var main = winParticle.main;
                main.useUnscaledTime = false; 
            }
        }

        public override void OnGameEnd()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
                bgmSource.pitch = 1.0f; 
            }
            StopAllCoroutines();
        }

        void SetupReelImages()
        {
            reelImages = new RectTransform[3][];
            reelImages[0] = GetImagesFromParent(reel1Parent);
            reelImages[1] = GetImagesFromParent(reel2Parent);
            reelImages[2] = GetImagesFromParent(reel3Parent);
        }

        RectTransform[] GetImagesFromParent(RectTransform parent)
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

        void Update()
        {
            float currentScale = Time.timeScale;

            if (bgmSource != null)
            {
                bgmSource.pitch = currentScale;
            }

            // ▼▼▼ ゲームプレイ（停止操作） ▼▼▼
            if (!isClear)
            {
                if (Action.WasPerformedThisFrame() || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    OnPressButton();
                }
            }

            // ▼▼▼ リスタート（Rキーのみ） ▼▼▼
            if (isDebugMode && isClear)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

            // ▼▼▼ リールの計算 ▼▼▼
            float baseBeatInterval = 60f / baseBpm;
            float elapsedTime = Time.time - startTime;

            for (int i = 0; i < 3; i++)
            {
                if (!isStopped[i])
                {
                    float currentBeatProgress = elapsedTime / baseBeatInterval;
                    reelPositions[i] = (currentBeatProgress * symbolsPerBeat + winSpriteIndex) * cellHeight;
                }
                
                // アニメーション減衰計算
                // Y座標の戻り（バネのような動き）
                bounceOffsets[i] = Mathf.Lerp(bounceOffsets[i], 0, Time.deltaTime * 10f);
                // ★スケールの戻り（拡大から元のサイズへ）
                bounceScales[i] = Mathf.Lerp(bounceScales[i], 0, Time.deltaTime * 8f);

                UpdateReelVisuals(i, reelPositions[i] + bounceOffsets[i]);
            }
        }

        void UpdateReelVisuals(int reelID, float scrollPos)
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

                // 遠近感の計算
                float dist = Mathf.Abs(rt.anchoredPosition.y - visualOffsetY);
                float perspective = Mathf.Lerp(1.0f, 0.6f, dist / cellHeight);

                // ★最終サイズ計算：(基本サイズ + バウンス拡大分) × 遠近感
                float finalScale = (baseScale + bounceScales[reelID]) * perspective;

                rt.localScale = new Vector3(finalScale, finalScale * currentBlurY, 1f);

                float virtualIndex = (scrollPos + currentY) / cellHeight;
                int spriteId = Mathf.RoundToInt(virtualIndex);
                spriteId = (-(spriteId - 2) % sourceSprites.Length + sourceSprites.Length) % sourceSprites.Length;

                rt.GetComponent<Image>().sprite = sourceSprites[spriteId];
            }
        }

        public void OnPressButton()
        {
            if (currentReelIndex >= 3) return; 

            isStopped[currentReelIndex] = true;

            Image centerImg = GetCenterImage(currentReelIndex);
            if (centerImg != null)
            {
                float diffY = centerImg.rectTransform.anchoredPosition.y - visualOffsetY;
                reelPositions[currentReelIndex] += diffY;
            }

            // 演出：停止時にY座標を少し沈ませる
            bounceOffsets[currentReelIndex] = -50f;
            
            // ★演出：停止時にサイズを「0.3」分大きく跳ねさせる
            // これにより、止まった瞬間に絵柄がグッと手前に拡大表示されます
            bounceScales[currentReelIndex] = 0.3f;

            if (seSource != null && stopSE != null) seSource.PlayOneShot(stopSE);

            currentReelIndex++;
            if (currentReelIndex >= 3) CheckWin();
        }

        void CheckWin()
        {
            string[] spriteNames = new string[3];
            Image[] centerImages = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                centerImages[i] = GetCenterImage(i);
                if (centerImages[i] != null) spriteNames[i] = centerImages[i].sprite.name;
            }

            if (spriteNames[0] == spriteNames[1] && spriteNames[1] == spriteNames[2])
            {
                isClear = true;
                if (seSource != null && winSE != null) seSource.PlayOneShot(winSE);
                
                if (winParticle != null) winParticle.Play();

                StartCoroutine(RainbowEffect(centerImages));
                Debug.Log("CLEAR!");
                MGManager.ClearGame();
            }
            else
            {
                Debug.Log("MISS! Restarting...");
                if (seSource != null && failSE != null) seSource.PlayOneShot(failSE);
                
                StartCoroutine(DelayRestart(0.5f));
            }
        }

        IEnumerator DelayRestart(float delayTime)
        {
            yield return new WaitForSecondsRealtime(delayTime);
            RestartReels();
        }

        void RestartReels()
        {
            if (isClear) return; 
            currentReelIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                isStopped[i] = false;
                bounceOffsets[i] = 0f;
                bounceScales[i] = 0f; // サイズ戻しもリセット
                foreach (var rt in reelImages[i]) rt.GetComponent<Image>().color = Color.white;
            }
        }

        Image GetCenterImage(int reelID)
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

        IEnumerator RainbowEffect(Image[] targets)
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