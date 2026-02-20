using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace YakinikuGameProject
{
    // クラス名は新しく「YK_BubbleData」とします
    [System.Serializable]
    public class YK_BubbleData
    {
        public Transform positionTransform;
        public bool flipX = false;         
        public Vector2 textOffset = Vector2.zero;
    }

    [RequireComponent(typeof(AudioSource))]
    public class YK_Yakiniku : MiniGameBase
    {
        [Header("【デバッグ設定】")]
        [SerializeField] private bool useTestStage = false;
        [SerializeField] private int testStageNumber = 10;
        [SerializeField] private Transform gameRoot; 

        [Header("【★「あなた」画像の点滅設定】")]
        [SerializeField] private Image youImage; 
        [SerializeField] private float blinkSpeed = 4f; 

        [Header("【演出設定】")]
        [SerializeField] private GameObject smokePrefab; 
        [SerializeField] private Transform smokeSpawnPoint; 
        [SerializeField] private Color burntSmokeColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private string[] msgTooEarly = { "早すぎ！" };
        [SerializeField] private string[] msgTooLate = { "焦げてる！" };
        [SerializeField] private string[] msgClear = { "完璧！" };

        [Header("【吹き出し個別設定】")]
        [SerializeField] private YK_BubbleData[] bubbleSettings; 
        [SerializeField] private GameObject speechBubbleRoot; 
        [SerializeField] private Image shockOverlay; 
        [SerializeField] private Color shockOverlayColor = new Color(0.1f, 0.1f, 0.3f, 0.8f);

        [Header("【音量設定】")]
        [SerializeField] [Range(0f, 1f)] private float resultVolume = 0.3f;
        [SerializeField] [Range(0f, 1f)] private float grillingVolume = 0.5f;

        [Header("【効果音】")]
        [SerializeField] private AudioClip flipSE;      
        [SerializeField] private AudioClip winSE;       
        [SerializeField] private AudioClip shockSE;     
        [SerializeField] private AudioClip grillingSE; 
        [SerializeField] private AudioClip speechSE;
        [SerializeField] private AudioClip landSE;

        [Header("【肉・UI】")]
        [SerializeField] private Image meatImage;
        [SerializeField] private Sprite rawSprite;    
        [SerializeField] private Sprite cookedSprite; 
        [SerializeField] private Sprite burntSprite;  
        [SerializeField] private Image timerGauge;        
        [SerializeField] private Image safeZoneImage;     
        [SerializeField] private RectTransform safeZoneRect; 

        [Header("【バランス】")]
        [SerializeField] private float timeLimitPerSide = 3.0f; 
        [SerializeField] private float safeZoneFirst = 0.45f;   
        [SerializeField] private float safeZoneSecond = 0.25f;
        [Tooltip("ゲージが出現する最小位置（秒）")]
        [SerializeField] private float randomStartMin = 0.5f; 
        [Tooltip("ゲージが出現する最大位置（秒）")]
        [SerializeField] private float randomStartMax = 2.0f; 

        [SerializeField] private float fadeStartTime = 0.5f;
        [SerializeField] private float fadeDuration = 0.8f;
        [SerializeField] private float gameZoomSpeed = 0.03f;
        [SerializeField] private float maxGameZoom = 1.3f;

        private AudioSource audioSource;
        private AudioSource grillingAudioSource; 
        private float currentOkStart, currentOkEnd;
        private float sideAProgress, sideBProgress;
        private bool isSideAUp = true, isPlaying = false, isFlipping = false;
        private int flipCount = 0;
        private float gaugeTimer = 0f, smokeTimer = 0f;
        private List<GameObject> activeBubbles = new List<GameObject>();
        private Coroutine blinkingCoroutine;

        public override void OnGameStart() {
            if (useTestStage) MGManager.TestPlay(testStageNumber);
            MGManager.Load(); 
            
            // ★追加：ゲーム開始時にゲージの設定を強制的に直す！★
            ForceFixUI();

            audioSource = GetComponent<AudioSource>();
            if (grillingAudioSource == null) grillingAudioSource = gameObject.AddComponent<AudioSource>();
            grillingAudioSource.clip = grillingSE; grillingAudioSource.loop = true;
            grillingAudioSource.volume = grillingVolume;
            ResetGame();
        }

        public override void OnGameEnd() { StopGrillingSound(); StopAllCoroutines(); }

        // ★追加：インスペクターのズレを自動修正するメソッド★
// ★追加：インスペクターのズレを自動修正するメソッド★
        private void ForceFixUI() {
            // 安全地帯の回転軸（Pivot）を中心にし、位置を(0,0)に強制リセット
            if (safeZoneRect != null) {
                safeZoneRect.pivot = new Vector2(0.5f, 0.5f);
                safeZoneRect.anchoredPosition = Vector2.zero;
            }
            // ゲージの画像設定を強制的に「円形ゲージ（Radial 360）」にする
            if (timerGauge != null) {
                timerGauge.type = Image.Type.Filled;
                timerGauge.fillMethod = Image.FillMethod.Radial360;
                // ★追加：開始位置を「真上(Top=2)」に強制する
                timerGauge.fillOrigin = (int)Image.Origin360.Top; 
            }
            if (safeZoneImage != null) {
                safeZoneImage.type = Image.Type.Filled;
                safeZoneImage.fillMethod = Image.FillMethod.Radial360;
                // ★追加：開始位置を「真上(Top=2)」に強制する
                safeZoneImage.fillOrigin = (int)Image.Origin360.Top; 
            }
        }

        private void Update() {
            if (!isPlaying) return;
            float dt = Time.deltaTime; 
            if (gameRoot != null) {
                Vector3 cur = gameRoot.localScale;
                gameRoot.localScale = Vector3.one * Mathf.Min(cur.x + (gameZoomSpeed * dt), maxGameZoom);
            }
            float curBurn = isSideAUp ? sideBProgress : sideAProgress;

            if (!isFlipping) {
                gaugeTimer += dt;
                if (isSideAUp) sideBProgress += dt; else sideAProgress += dt;
                smokeTimer += dt;
                if (smokeTimer >= 0.15f) { SpawnSmoke(curBurn > currentOkEnd); smokeTimer = 0f; }
            }

            if (curBurn > currentOkEnd && !isFlipping) {
                float over = (curBurn - currentOkEnd) / (timeLimitPerSide - currentOkEnd);
                if (meatImage) meatImage.color = Color.Lerp(Color.white, new Color(0.2f, 0.2f, 0.2f), over);
                if (shockOverlay) shockOverlay.color = new Color(0, 0, 0, over * 0.4f);
            }

            if (!isFlipping && curBurn > timeLimitPerSide) {
                isPlaying = false; StopGrillingSound(); FlipMeat(curBurn); StartCoroutine(FailSequence(msgTooLate)); return;
            }
            if (Action.WasPerformedThisFrame() && !isFlipping) HandleInput(curBurn);
            UpdateGaugeOnly(); 
        }

        private IEnumerator BlinkImageRoutine(Image targetImage) {
            if (targetImage == null) yield break;
            targetImage.gameObject.SetActive(true); 
            Color originalColor = targetImage.color;
            while (true) {
                float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; 
                alpha = 0.2f + (alpha * 0.8f); 
                targetImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        private IEnumerator ClearSequence(string[] messages) {
            if (audioSource && winSE) audioSource.PlayOneShot(winSE, resultVolume);
            SetGaugeAlpha(1f); 
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SpawnBubblesSequence(messages));
            
            yield return new WaitForSeconds(0.8f); 

            float fadeOutTime = 0.5f;
            for (float t = 0; t < fadeOutTime; t += Time.deltaTime) {
                SetGaugeAlpha(1f - (t / fadeOutTime)); 
                yield return null;
            }
            SetGaugeAlpha(0f); 

            yield return new WaitForSeconds(0.5f); 
            MGManager.ClearGame(); 
        }

        private IEnumerator FailSequence(string[] messages) {
            if (audioSource && shockSE) audioSource.PlayOneShot(shockSE, resultVolume);
            SetGaugeAlpha(1f); SetGaugeColor(new Color(1f, 0.3f, 0.3f, 1f));
            if (shockOverlay != null) {
                float el = 0f;
                while (el < 0.5f) { el += Time.deltaTime; shockOverlay.color = Color.Lerp(shockOverlay.color, shockOverlayColor, el / 0.5f); yield return null; }
            }
            yield return new WaitForSeconds(0.3f); StartCoroutine(SpawnBubblesSequence(messages));
        }

        private IEnumerator SpawnBubblesSequence(string[] messages) {
            if (speechBubbleRoot == null || bubbleSettings == null) yield break;
            for (int i = 0; i < bubbleSettings.Length; i++) {
                YK_BubbleData setting = bubbleSettings[i];
                if (setting.positionTransform == null) continue;
                GameObject nb = Instantiate(speechBubbleRoot, speechBubbleRoot.transform.parent);
                nb.transform.position = setting.positionTransform.position;
                float fX = setting.flipX ? -1f : 1f;
                nb.transform.localScale = new Vector3(fX, 1f, 1f);
                Text t = nb.GetComponentInChildren<Text>();
                if (t != null) { 
                    t.text = messages[i % messages.Length]; 
                    t.transform.localScale = new Vector3(fX, 1f, 1f); 
                    t.rectTransform.anchoredPosition = setting.textOffset; 
                }
                nb.SetActive(true); activeBubbles.Add(nb);
                StartCoroutine(PopUpAnimation(nb.transform, fX));
                if (audioSource && speechSE) audioSource.PlayOneShot(speechSE, resultVolume);
                yield return new WaitForSeconds(0.15f);
            }
        }

        private void SpawnSmoke(bool isBurnt) {
            if (!smokePrefab || !smokeSpawnPoint) return;
            GameObject s = Instantiate(smokePrefab, smokeSpawnPoint.position, Quaternion.identity, smokeSpawnPoint);
            s.SetActive(true); StartCoroutine(SmokeRoutine(s, isBurnt));
        }

        private IEnumerator SmokeRoutine(GameObject s, bool b) {
            Image img = s.GetComponent<Image>(); float el = 0f, dur = 2f;
            Vector3 start = s.transform.localPosition; float drift = Random.Range(-35f, 35f);
            Color baseC = b ? burntSmokeColor : Color.white;
            while (el < dur) {
                el += Time.deltaTime; float p = el/dur; if (!s) break;
                s.transform.localPosition = start + new Vector3(Mathf.Sin(p*4f)*drift, p*400f, 0);
                s.transform.localScale = Vector3.one * (1f + p*1.5f);
                if (img) img.color = new Color(baseC.r, baseC.g, baseC.b, (1f-p)*baseC.a);
                yield return null;
            }
            if (s) Destroy(s);
        }

        private void HandleInput(float p) {
            bool ok = (p >= currentOkStart && p <= currentOkEnd);
            if (audioSource && flipSE) audioSource.PlayOneShot(flipSE);
            if (flipCount == 0) {
                if (!ok) { isPlaying = false; StopGrillingSound(); FlipMeat(p); StartCoroutine(FailSequence(p < currentOkStart ? msgTooEarly : msgTooLate)); }
                else { FlipMeat(p); }
            } else {
                isPlaying = false; StopGrillingSound(); FlipMeat(p); 
                if (ok) StartCoroutine(ClearSequence(msgClear)); else StartCoroutine(FailSequence(p < currentOkStart ? msgTooEarly : msgTooLate));
            }
        }

        private void StopGrillingSound() { 
            if (grillingAudioSource != null) grillingAudioSource.Stop(); 
        }

        private void FlipMeat(float p) { flipCount++; StartCoroutine(FlipAnimationRoutine(p)); }

        private IEnumerator FlipAnimationRoutine(float p) {
            if (!meatImage) yield break; isFlipping = true;
            Sprite next = (p < currentOkStart) ? rawSprite : (p <= currentOkEnd ? cookedSprite : burntSprite);
            float sX = meatImage.transform.localScale.x; bool sw = false;
            for (float t = 0; t < 0.25f; t += Time.deltaTime) {
                float pr = t/0.25f; float sy = Mathf.Abs(Mathf.Cos(pr*Mathf.PI));
                if (pr >= 0.5f && !sw) {
                    sw = true; meatImage.sprite = next; meatImage.color = Color.white;
                    if (audioSource && landSE) audioSource.PlayOneShot(landSE);
                    if (flipCount == 1 && isPlaying) { isSideAUp = !isSideAUp; gaugeTimer = 0f; RandomizeSafeZone(); }
                }
                meatImage.transform.localScale = new Vector3(sw ? -sX : sX, sy, 1f); yield return null;
            }
            meatImage.transform.localScale = new Vector3(-sX, 1f, 1f); isFlipping = false;
        }

        private IEnumerator PopUpAnimation(Transform t, float fX) {
            t.localScale = Vector3.zero;
            for (float time = 0; time < 0.4f; time += Time.deltaTime) {
                float s = (time/0.4f < 0.8f ? Mathf.Sin((time/0.4f)*Mathf.PI*0.5f)*1.1f : 1f);
                t.localScale = new Vector3(fX * s, s, 1f); yield return null;
            }
        }

        private void UpdateGaugeOnly() {
            if (timerGauge) timerGauge.fillAmount = gaugeTimer / timeLimitPerSide;
            if (flipCount == 1 && isPlaying) SetGaugeAlpha(Mathf.Clamp01((gaugeTimer > fadeStartTime) ? 1f - ((gaugeTimer - fadeStartTime) / fadeDuration) : 1f));
        }

        private void SetGaugeAlpha(float a) { if (timerGauge) { Color c = timerGauge.color; c.a = a; timerGauge.color = c; } if (safeZoneImage) { Color c = safeZoneImage.color; c.a = a; safeZoneImage.color = c; } }
        private void SetGaugeColor(Color c) { if (timerGauge) { float a = timerGauge.color.a; c.a = a; timerGauge.color = c; } }
        
        private void RandomizeSafeZone() { 
            float currentDuration = (flipCount == 0) ? safeZoneFirst : safeZoneSecond;
            float max = Mathf.Min(randomStartMax, timeLimitPerSide - currentDuration);
            float min = Mathf.Min(randomStartMin, max);

            currentOkStart = Random.Range(min, max); 
            currentOkEnd = currentOkStart + currentDuration; 
            
            if (safeZoneImage) safeZoneImage.fillAmount = currentDuration / timeLimitPerSide; 
            if (safeZoneRect) safeZoneRect.localRotation = Quaternion.Euler(0, 0, -(currentOkStart / timeLimitPerSide) * 360f); 
        }
        
        private void ResetGame() {
            flipCount = 0; gaugeTimer = 0f; isPlaying = true; isFlipping = false;
            if (meatImage) { meatImage.color = Color.white; meatImage.sprite = rawSprite; }
            if (shockOverlay) shockOverlay.color = Color.clear;
            foreach (var b in activeBubbles) if (b) Destroy(b); activeBubbles.Clear();
            if (speechBubbleRoot) speechBubbleRoot.SetActive(false);
            if (grillingAudioSource) grillingAudioSource.Play();
            RandomizeSafeZone();

            if (blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);
            if (youImage != null) {
                blinkingCoroutine = StartCoroutine(BlinkImageRoutine(youImage));
            }
        }
    }
}