using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 名前空間（MeoshiSlotフォルダならMeoshiSlotにするのが無難ですが、ここでは一意な名前空間にします）
namespace MeoshiSlotGame_IK
{
    // MiniGameBaseを継承
    public class RhythmSlot : MiniGameBase
    {
        [Header("【リールの親オブジェクト】")]
        [SerializeField] private RectTransform reel1Parent;
        [SerializeField] private RectTransform reel2Parent;
        [SerializeField] private RectTransform reel3Parent;

        [Header("【絵柄の元データ】")]
        [SerializeField] private Sprite[] sourceSprites;

        [Header("【設定】")]
        [SerializeField] private float bpm = 120f;
        [SerializeField] private int winSpriteIndex = 0;
        [SerializeField] private int symbolsPerBeat = 4;
        [SerializeField] private float timeLimit = 10f;

        [Header("【見た目の調整】")]
        [SerializeField] private float cellHeight = 250f;
        [SerializeField] private float visualOffsetY = 0f;

        [Header("【音源設定】")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioClip stopSE;
        [SerializeField] private AudioClip winSE;
        [SerializeField] private AudioClip failSE;

        [Header("【UI表示用(任意)】")]
        [SerializeField] private Text timerText;

        private RectTransform[][] reelImages;
        private bool[] isStopped = new bool[3];
        private int currentReelIndex = 0;
        private float[] reelPositions = new float[3];
        private float beatInterval;
        private float startTime;
        private float remainingTime;
        private bool isGameOver = false;
        private bool isClear = false;

        // 仕様書ルール：Start禁止、OnGameStartを使用
        public override void OnGameStart()
        {
            // 仕様書ルール：ロード処理
            MGManager.Load();

            SetupReelImages();
            beatInterval = 60f / bpm;
            startTime = Time.time;
            remainingTime = timeLimit;

            if (bgmSource != null)
            {
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }

        // 仕様書ルール：終了処理
        public override void OnGameEnd()
        {
            if (bgmSource != null) bgmSource.Stop();
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
            if (isGameOver || isClear) return;

            // タイマー管理
            remainingTime -= Time.deltaTime;
            if (timerText != null) timerText.text = "TIME: " + Mathf.Max(0, remainingTime).ToString("F1");

            if (remainingTime <= 0)
            {
                TimeUp();
                return;
            }

            // ★ここが修正ポイント！
            // 「Action（仕様書）」または「Input（いつもの）」のどちらかが反応すればOKにする
            if (Action.WasPerformedThisFrame() || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                OnPressButton();
            }

            // リールの回転
            float elapsedTime = Time.time - startTime;
            for (int i = 0; i < 3; i++)
            {
                if (!isStopped[i])
                {
                    float currentBeatProgress = elapsedTime / beatInterval;
                    reelPositions[i] = (currentBeatProgress * symbolsPerBeat + winSpriteIndex) * cellHeight;
                }
                UpdateReelVisuals(i, reelPositions[i]);
            }
        }
        void UpdateReelVisuals(int reelID, float scrollPos)
        {
            RectTransform[] images = reelImages[reelID];
            int count = images.Length;
            float totalHeight = count * cellHeight;

            for (int j = 0; j < count; j++)
            {
                RectTransform rt = images[j];
                float basePosY = (2 - j) * cellHeight;
                float currentY = basePosY - (scrollPos % totalHeight);

                if (currentY < -(totalHeight / 2)) currentY += totalHeight;
                else if (currentY > (totalHeight / 2)) currentY -= totalHeight;

                rt.anchoredPosition = new Vector2(0, currentY + visualOffsetY);

                float dist = Mathf.Abs(rt.anchoredPosition.y - visualOffsetY);
                float scale = Mathf.Lerp(1.0f, 0.6f, dist / cellHeight);
                rt.localScale = new Vector3(scale, scale, 1f);

                float virtualIndex = (scrollPos + currentY) / cellHeight;
                int spriteId = Mathf.RoundToInt(virtualIndex);
                spriteId = (-(spriteId - 2) % sourceSprites.Length + sourceSprites.Length) % sourceSprites.Length;

                rt.GetComponent<Image>().sprite = sourceSprites[spriteId];
            }
        }

        public void OnPressButton()
        {
            if (currentReelIndex >= 3 || remainingTime <= 0) return;

            isStopped[currentReelIndex] = true;

            Image centerImg = GetCenterImage(currentReelIndex);
            if (centerImg != null)
            {
                float diffY = centerImg.rectTransform.anchoredPosition.y - visualOffsetY;
                reelPositions[currentReelIndex] += diffY;
                UpdateReelVisuals(currentReelIndex, reelPositions[currentReelIndex]);
            }

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
                StartCoroutine(RainbowEffect(centerImages));
                Debug.Log("CLEAR!");

                // 仕様書ルール：クリア時に呼ぶ
                MGManager.ClearGame();
            }
            else
            {
                Debug.Log("MISS! Restarting...");
                if (seSource != null && failSE != null) seSource.PlayOneShot(failSE);
                Invoke("RestartReels", 0.5f);
            }
        }

        void RestartReels()
        {
            if (remainingTime <= 0 || isClear) return;

            currentReelIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                isStopped[i] = false;
                foreach (var rt in reelImages[i]) rt.GetComponent<Image>().color = Color.white;
            }
        }

        void TimeUp()
        {
            isGameOver = true;
            Debug.Log("TIME UP!");
            
            for (int i = 0; i < 3; i++)
            {
                Image center = GetCenterImage(i);
                if (center != null) center.color = new Color(0.3f, 0.3f, 0.3f);
            }
            if (seSource != null && failSE != null) seSource.PlayOneShot(failSE);
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

        System.Collections.IEnumerator RainbowEffect(Image[] targets)
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