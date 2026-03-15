using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ObakeGameProject
{
    public class OB_Obake : MiniGameBase
    {
        [Header("【UIオブジェクト設定】")]
        [SerializeField] private RectTransform backgroundRect; 
        [SerializeField] private RectTransform ghostRect;      
        [SerializeField] private Image ghostImage;             
        [SerializeField] private RectTransform customerRect;   
        [SerializeField] private Image customerImage;          
        [SerializeField] private RectTransform targetAreaRect; 

        [Header("【画面外吹き出しUI設定】")]
        [SerializeField] private GameObject rightBubbleObj;     
        [SerializeField] private GameObject leftBubbleObj;      
        [SerializeField] private float offScreenThreshold = 1100f;  

        [Header("【画面内（お化け用）吹き出しUI設定】")]
        [SerializeField] private GameObject ghostRightBubbleObj;     
        [SerializeField] private GameObject ghostLeftBubbleObj;     
        [Tooltip("お化けから吹き出しをどれくらいずらすか(Xは右方向への基準値)")]
        [SerializeField] private Vector2 ghostBubbleOffset = new Vector2(150f, 150f);

        [Header("【演出・難易度アップUI】")]
        [SerializeField] private CanvasGroup darkEdgesGroup;     
        [SerializeField] private RectTransform leftDarkEdge;     
        [SerializeField] private RectTransform rightDarkEdge;    
        [SerializeField] private float darkEdgeSlideAmount = 500f; 

        [SerializeField] private float targetFadeOutTime = 1.5f; 
        [SerializeField] private float rewindTime = 0.8f; 
        [SerializeField] private float targetBlinkSpeed = 15f; 

        [Header("【お客さんの画像設定】")]
        [SerializeField] private Sprite normalCustomerSprite;    
        [SerializeField] private Sprite surprisedCustomerSprite; 
        [SerializeField] private GameObject questionMarkObj; 

        [Header("【ゲームの進行・距離設定】")]
        [Tooltip("背景の開始位置をずらす最大幅（背景画像が途切れない範囲で設定してください）")]
        [SerializeField] private float maxBackgroundOffset = 500f; 

        [SerializeField] private float minGhostDistance = 3000f; 
        [SerializeField] private float maxGhostDistance = 5000f; 
        [SerializeField] private float memorizeTime = 1.0f;      
        [SerializeField] private float ghostFadeTime = 0.5f;     
        [SerializeField] private float scrollSpeed = 800f;       

        [Header("【判定エリアの設定】")]
        [SerializeField] private float perfectDistance = 250f;   

        [Header("【歩行アニメーション設定】")]
        [SerializeField] private float walkBobbingSpeed = 15f;   
        [SerializeField] private float walkBobbingAmount = 10f;  

        [Header("【幽霊の浮遊アニメーション】")]
        [SerializeField] private float ghostFloatSpeed = 3f;     
        [SerializeField] private float ghostFloatAmount = 20f;   

        [Header("【オーディオ設定（一時的）】")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip appearSE;      
        [SerializeField] private AudioClip footstepSE;    
        [SerializeField] private AudioClip scareSE;       
        [SerializeField] private AudioClip surpriseSE;    
        
        private float ghostInitialY; 
        private float customerInitialX; 
        private float customerInitialY; 
        
        private CanvasGroup targetCanvasGroup;           

        private Vector2 rightBubbleInitialPos;
        private Vector3 rightBubbleInitialScale;
        private Vector2 leftBubbleInitialPos;
        private Vector3 leftBubbleInitialScale;
        
        private Vector3 ghostRightBubbleInitialScale;
        private Vector3 ghostLeftBubbleInitialScale;

        private float leftDarkTargetX;
        private float rightDarkTargetX;

        private float currentBgOffset;

        private float currentScrollSpeed;
        private float currentWalkBobbingSpeed;
        private float currentGhostFloatSpeed;
        private float currentTargetBlinkSpeed;
        private float currentMemorizeTime;
        private float currentGhostFadeTime;
        private float currentRewindTime;
        private float currentTargetFadeOutTime;

        private enum GameState { Memorize, Rewinding, Scrolling, Result }
        private GameState currentState;

        private bool canInput = false;

        public override void OnGameStart()
        {
            MGManager.Load(); 
            customerInitialX = customerRect.anchoredPosition.x; 
            customerInitialY = customerRect.anchoredPosition.y; 
            ghostInitialY = ghostRect.anchoredPosition.y;

            if (targetAreaRect != null)
            {
                targetCanvasGroup = targetAreaRect.GetComponent<CanvasGroup>();
                if (targetCanvasGroup == null)
                {
                    targetCanvasGroup = targetAreaRect.gameObject.AddComponent<CanvasGroup>();
                }
            }

            if (rightBubbleObj != null)
            {
                var rt = rightBubbleObj.GetComponent<RectTransform>();
                rightBubbleInitialPos = rt.anchoredPosition;
                rightBubbleInitialScale = rt.localScale;
            }
            if (leftBubbleObj != null)
            {
                var rt = leftBubbleObj.GetComponent<RectTransform>();
                leftBubbleInitialPos = rt.anchoredPosition;
                leftBubbleInitialScale = rt.localScale;
            }
            
            if (ghostRightBubbleObj != null)
            {
                ghostRightBubbleInitialScale = ghostRightBubbleObj.GetComponent<RectTransform>().localScale;
            }
            if (ghostLeftBubbleObj != null)
            {
                ghostLeftBubbleInitialScale = ghostLeftBubbleObj.GetComponent<RectTransform>().localScale;
            }

            if (leftDarkEdge != null) leftDarkTargetX = leftDarkEdge.anchoredPosition.x;
            if (rightDarkEdge != null) rightDarkTargetX = rightDarkEdge.anchoredPosition.x;

            ResetGame();
        }

        public override void OnGameEnd()
        {
        }

        private void ResetGame()
        {
            customerImage.sprite = normalCustomerSprite; 
            customerImage.enabled = true; 
            canInput = false; 

            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;

            currentScrollSpeed = scrollSpeed * ts;
            currentWalkBobbingSpeed = walkBobbingSpeed * ts;
            currentGhostFloatSpeed = ghostFloatSpeed * ts;
            currentTargetBlinkSpeed = targetBlinkSpeed * ts;

            currentMemorizeTime = memorizeTime / ts;
            currentGhostFadeTime = ghostFadeTime / ts;
            currentRewindTime = rewindTime / ts;
            currentTargetFadeOutTime = targetFadeOutTime / ts;

            currentBgOffset = Random.Range(0f, maxBackgroundOffset);

            if (questionMarkObj != null) questionMarkObj.SetActive(false);

            ghostRect.localScale = Vector3.one;
            
            if (rightBubbleObj != null)
            {
                rightBubbleObj.SetActive(false);
                var rt = rightBubbleObj.GetComponent<RectTransform>();
                rt.anchoredPosition = rightBubbleInitialPos;
                rt.localScale = rightBubbleInitialScale;
            }
            
            if (leftBubbleObj != null)
            {
                leftBubbleObj.SetActive(false);
                var rt = leftBubbleObj.GetComponent<RectTransform>();
                rt.anchoredPosition = leftBubbleInitialPos;
                rt.localScale = leftBubbleInitialScale;
            }

            if (ghostRightBubbleObj != null)
            {
                ghostRightBubbleObj.SetActive(false);
                ghostRightBubbleObj.GetComponent<RectTransform>().localScale = ghostRightBubbleInitialScale;
            }
            if (ghostLeftBubbleObj != null)
            {
                ghostLeftBubbleObj.SetActive(false);
                ghostLeftBubbleObj.GetComponent<RectTransform>().localScale = ghostLeftBubbleInitialScale;
            }

            if (targetAreaRect != null)
            {
                targetAreaRect.gameObject.SetActive(false);
                if (targetCanvasGroup != null)
                {
                    targetCanvasGroup.alpha = 1f;
                }
            }

            if (darkEdgesGroup != null) darkEdgesGroup.alpha = 0f;
            if (leftDarkEdge != null) 
                leftDarkEdge.anchoredPosition = new Vector2(leftDarkTargetX - darkEdgeSlideAmount, leftDarkEdge.anchoredPosition.y);
            if (rightDarkEdge != null) 
                rightDarkEdge.anchoredPosition = new Vector2(rightDarkTargetX + darkEdgeSlideAmount, rightDarkEdge.anchoredPosition.y);

            float randomX = Random.Range(minGhostDistance, maxGhostDistance); 
            
            ghostRect.anchoredPosition = new Vector2(randomX - currentBgOffset, ghostInitialY);
            backgroundRect.anchoredPosition = new Vector2(-randomX + currentBgOffset, backgroundRect.anchoredPosition.y);
            customerRect.anchoredPosition = new Vector2(customerInitialX - randomX, customerInitialY);

            Color ghostColor = ghostImage.color;
            ghostColor.a = 0f;
            ghostImage.color = ghostColor;
            ghostImage.enabled = true;

            if (audioSource != null) audioSource.Stop();
            SEPlay("OB_Appear");
            //if (audioSource != null && appearSE != null) audioSource.PlayOneShot(appearSE);

            StartCoroutine(GhostInitialSpawnRoutine());

            currentState = GameState.Memorize;
            StartCoroutine(MemorizeRoutine());
        }

        private IEnumerator GhostInitialSpawnRoutine()
        {
            float elapsed = 0f;
            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;
            float duration = 0.2f / ts; 
            
            Color c = ghostImage.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                c.a = Mathf.Lerp(0f, 1f, t);
                ghostImage.color = c;
                
                float scaleVal = Mathf.Lerp(0.5f, 1f, 1f - Mathf.Pow(1f - t, 3f));
                ghostRect.localScale = new Vector3(scaleVal, scaleVal, 1f);
                
                yield return null;
            }
            
            c.a = 1f;
            ghostImage.color = c;
            ghostRect.localScale = Vector3.one;
        }

        private IEnumerator MemorizeRoutine()
        {
            yield return new WaitForSeconds(currentMemorizeTime);
            
            float fadeElapsed = 0f;
            Color ghostColor = ghostImage.color;
            while (fadeElapsed < currentGhostFadeTime)
            {
                fadeElapsed += Time.deltaTime;
                ghostColor.a = Mathf.Lerp(1f, 0f, fadeElapsed / currentGhostFadeTime);
                ghostImage.color = ghostColor;
                yield return null; 
            }
            ghostColor.a = 0f;
            ghostImage.color = ghostColor;
            
            currentState = GameState.Rewinding;
            float rewindElapsed = 0f;
            
            float actualDistance = customerInitialX - customerRect.anchoredPosition.x;

            while (rewindElapsed < currentRewindTime)
            {
                rewindElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(rewindElapsed / currentRewindTime);
                float easeT = 1f - Mathf.Pow(1f - t, 4f);

                float currentX = Mathf.Lerp(-actualDistance, 0f, easeT);
                
                backgroundRect.anchoredPosition = new Vector2(currentX + currentBgOffset, backgroundRect.anchoredPosition.y);

                float bobbingY = customerInitialY + Mathf.Sin(Time.time * currentWalkBobbingSpeed) * walkBobbingAmount;
                customerRect.anchoredPosition = new Vector2(customerInitialX + currentX, bobbingY);

                if (darkEdgesGroup != null) darkEdgesGroup.alpha = Mathf.Lerp(0f, 1f, easeT);
                if (leftDarkEdge != null)
                {
                    float leftX = Mathf.Lerp(leftDarkTargetX - darkEdgeSlideAmount, leftDarkTargetX, easeT);
                    leftDarkEdge.anchoredPosition = new Vector2(leftX, leftDarkEdge.anchoredPosition.y);
                }
                if (rightDarkEdge != null)
                {
                    float rightX = Mathf.Lerp(rightDarkTargetX + darkEdgeSlideAmount, rightDarkTargetX, easeT);
                    rightDarkEdge.anchoredPosition = new Vector2(rightX, rightDarkEdge.anchoredPosition.y);
                }

                yield return null; 
            }

            backgroundRect.anchoredPosition = new Vector2(currentBgOffset, backgroundRect.anchoredPosition.y);
            customerRect.anchoredPosition = new Vector2(customerInitialX, customerInitialY);
            
            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;
            yield return new WaitForSeconds(0.1f / ts);

            if (targetAreaRect != null) targetAreaRect.gameObject.SetActive(true);

            SEPlay("OB_Footstep");
            //if (audioSource != null && footstepSE != null) audioSource.PlayOneShot(footstepSE);

            currentState = GameState.Scrolling;
            StartCoroutine(ScrollingEffectsRoutine());
        }

        private IEnumerator ScrollingEffectsRoutine()
        {
            float elapsed = 0f;
            if (targetCanvasGroup == null || currentTargetFadeOutTime <= 0f) canInput = true;

            while (currentState == GameState.Scrolling)
            {
                elapsed += Time.deltaTime;

                if (targetCanvasGroup != null)
                {
                    if (elapsed <= currentTargetFadeOutTime)
                    {
                        float fadeRatio = 1f - (elapsed / currentTargetFadeOutTime);
                        float blink = Mathf.Abs(Mathf.Sin(elapsed * currentTargetBlinkSpeed));
                        targetCanvasGroup.alpha = blink * fadeRatio;
                    }
                    else
                    {
                        targetCanvasGroup.alpha = 0f;
                        canInput = true; 
                    }
                }
                yield return null;
            }
        }

        private void Update()
        {
            if (ghostRect != null && currentState != GameState.Result)
            {
                float floatY = ghostInitialY + Mathf.Sin(Time.time * currentGhostFloatSpeed) * ghostFloatAmount;
                ghostRect.anchoredPosition = new Vector2(ghostRect.anchoredPosition.x, floatY);
            }

            if (currentState != GameState.Scrolling) return;

            backgroundRect.anchoredPosition -= new Vector2(currentScrollSpeed * Time.deltaTime, 0);
            float newY = customerInitialY + Mathf.Sin(Time.time * currentWalkBobbingSpeed) * walkBobbingAmount;
            customerRect.anchoredPosition = new Vector2(customerInitialX, newY);

            if (canInput && Action.WasPerformedThisFrame())
            {
                JudgeTiming();
            }
        }

        private void JudgeTiming()
        {
            currentState = GameState.Result;
            canInput = false; 

            if (audioSource != null) audioSource.Stop();
            SEPlay("OB_Scare");
            //if (audioSource != null && scareSE != null) audioSource.PlayOneShot(scareSE);

            if (targetAreaRect != null) targetAreaRect.gameObject.SetActive(false);
            
            StartCoroutine(GhostScareRoutine());
            
            customerRect.anchoredPosition = new Vector2(customerInitialX, customerInitialY);

            float targetCenterX = customerInitialX; 
            float targetHalfWidth = perfectDistance; 

            if (targetAreaRect != null)
            {
                targetCenterX = targetAreaRect.anchoredPosition.x;
                targetHalfWidth = targetAreaRect.rect.width / 2f;
            }

            float ghostScreenX = backgroundRect.anchoredPosition.x + ghostRect.anchoredPosition.x;
            float minX = targetCenterX - targetHalfWidth;
            float maxX = targetCenterX + targetHalfWidth;

            bool isInsideTarget = (ghostScreenX >= minX && ghostScreenX <= maxX);
            bool isOffScreen = Mathf.Abs(ghostScreenX - targetCenterX) > offScreenThreshold;

            if (isInsideTarget)
            {
                customerImage.sprite = surprisedCustomerSprite; 

                SEPlay("OB_Surprise");
                //if (audioSource != null && surpriseSE != null) audioSource.PlayOneShot(surpriseSE);

                MGManager.ClearGame(); 
            }
            else
            {
                // 早すぎる＝お化けは客より右側にいる
                bool isTooEarly = ghostScreenX > maxX;

                customerImage.sprite = normalCustomerSprite; 
                
                if (questionMarkObj != null)
                {
                    StartCoroutine(ShowQuestionMarkRoutine());
                }

                if (isOffScreen)
                {
                    // 画面外の場合：画面端に固定配置したUIをそのまま表示
                    if (isTooEarly && rightBubbleObj != null) 
                    {
                        rightBubbleObj.SetActive(true);
                        StartCoroutine(BubbleAppearRoutine(rightBubbleObj));
                    }
                    else if (!isTooEarly && leftBubbleObj != null) 
                    {
                        leftBubbleObj.SetActive(true);
                        StartCoroutine(BubbleAppearRoutine(leftBubbleObj));
                    }
                }
                else
                {
                    // 画面内の場合：アンカー依存を避けるため、お化けのワールド座標に合わせてからズラす
                    // 早すぎる＝お化けは右にいる＝吹き出しは左に出す
                    if (isTooEarly && ghostLeftBubbleObj != null)
                    {
                        ghostLeftBubbleObj.SetActive(true);
                        
                        RectTransform rt = ghostLeftBubbleObj.GetComponent<RectTransform>();
                        rt.position = ghostRect.position; 
                        rt.anchoredPosition += new Vector2(-ghostBubbleOffset.x, ghostBubbleOffset.y); 
                        
                        StartCoroutine(BubbleAppearRoutine(ghostLeftBubbleObj));
                    }
                    // 遅すぎる＝お化けは左にいる＝吹き出しは右に出す
                    else if (!isTooEarly && ghostRightBubbleObj != null)
                    {
                        ghostRightBubbleObj.SetActive(true);
                        
                        RectTransform rt = ghostRightBubbleObj.GetComponent<RectTransform>();
                        rt.position = ghostRect.position; 
                        rt.anchoredPosition += new Vector2(ghostBubbleOffset.x, ghostBubbleOffset.y); 
                        
                        StartCoroutine(BubbleAppearRoutine(ghostRightBubbleObj));
                    }
                }

                StartCoroutine(FailSequence(isTooEarly, isOffScreen));
            }
        }

        private IEnumerator GhostScareRoutine()
        {
            float elapsed = 0f;
            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;
            float duration = 0.2f / ts; 

            Color c = ghostImage.color;
            Vector3 defaultScale = Vector3.one;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                c.a = Mathf.Lerp(0f, 1f, t * 2f);
                ghostImage.color = c;

                float scaleVal;
                if (t < 0.6f)
                {
                    float nt = t / 0.6f;
                    scaleVal = Mathf.Lerp(0.5f, 1.2f, 1f - Mathf.Pow(1f - nt, 3f)); 
                }
                else
                {
                    float nt = (t - 0.6f) / 0.4f;
                    scaleVal = Mathf.Lerp(1.2f, 1f, nt); 
                }
                
                ghostRect.localScale = defaultScale * scaleVal;
                yield return null;
            }

            c.a = 1f;
            ghostImage.color = c;
            ghostRect.localScale = defaultScale;
        }

        private IEnumerator BubbleAppearRoutine(GameObject bubbleObj)
        {
            if (bubbleObj == null) yield break;
            
            RectTransform rt = bubbleObj.GetComponent<RectTransform>();
            if (rt == null) yield break;

            float elapsed = 0f;
            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;
            float duration = 0.2f / ts;
            
            Vector3 targetScale = Vector3.one;
            if (bubbleObj == rightBubbleObj) targetScale = rightBubbleInitialScale;
            else if (bubbleObj == leftBubbleObj) targetScale = leftBubbleInitialScale;
            else if (bubbleObj == ghostRightBubbleObj) targetScale = ghostRightBubbleInitialScale;
            else if (bubbleObj == ghostLeftBubbleObj) targetScale = ghostLeftBubbleInitialScale;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                float scaleVal;
                if (t < 0.6f)
                {
                    float nt = t / 0.6f;
                    scaleVal = Mathf.Lerp(0f, 1.2f, 1f - Mathf.Pow(1f - nt, 3f));
                }
                else
                {
                    float nt = (t - 0.6f) / 0.4f;
                    scaleVal = Mathf.Lerp(1.2f, 1f, nt);
                }
                
                rt.localScale = targetScale * scaleVal;
                yield return null;
            }
            rt.localScale = targetScale;
        }

        private IEnumerator ShowQuestionMarkRoutine()
        {
            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;
            yield return new WaitForSeconds(0.5f / ts);

            questionMarkObj.SetActive(true);
            RectTransform qRect = questionMarkObj.GetComponent<RectTransform>();
            
            if (qRect != null)
            {
                qRect.localScale = Vector3.zero; 
                
                float elapsed = 0f;
                float duration = 0.3f / ts; 

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    float easeT = 1f - Mathf.Pow(1f - t, 3f); 
                    qRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, easeT);
                    yield return null;
                }
                
                qRect.localScale = Vector3.one; 
            }
        }

        private IEnumerator FailSequence(bool isTooEarly, bool isOffScreen)
        {
            float ts = Time.timeScale > 0f ? Time.timeScale : 1f;
            yield return new WaitForSeconds(0.3f / ts);

            float elapsed = 0f;
            float duration = 1.0f / ts;

            RectTransform targetRect = ghostRect;
            RectTransform ghostBubbleRect = null; 

            if (isOffScreen)
            {
                targetRect = isTooEarly ? rightBubbleObj.GetComponent<RectTransform>() : leftBubbleObj.GetComponent<RectTransform>();
            }
            else
            {
                if (isTooEarly && ghostLeftBubbleObj != null) ghostBubbleRect = ghostLeftBubbleObj.GetComponent<RectTransform>();
                else if (!isTooEarly && ghostRightBubbleObj != null) ghostBubbleRect = ghostRightBubbleObj.GetComponent<RectTransform>();
            }

            Vector3 initialPos = targetRect.anchoredPosition;
            Vector3 bubbleInitialPos = ghostBubbleRect != null ? ghostBubbleRect.anchoredPosition : Vector3.zero;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float shake = Mathf.Sin(Time.time * 60f * ts) * 15f; 
                targetRect.anchoredPosition = new Vector2(initialPos.x + shake, initialPos.y);
                
                if (ghostBubbleRect != null)
                {
                    ghostBubbleRect.anchoredPosition = new Vector2(bubbleInitialPos.x + shake, bubbleInitialPos.y);
                }

                yield return null;
            }

            targetRect.anchoredPosition = initialPos;
            if (ghostBubbleRect != null) ghostBubbleRect.anchoredPosition = bubbleInitialPos;

            yield return new WaitForSeconds(0.5f / ts);
        }
    }
}