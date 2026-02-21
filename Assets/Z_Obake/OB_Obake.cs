using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// ※必要に応じて、別のネームスペースをusingしてください

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

        [Header("【お客さんの画像設定】")]
        [SerializeField] private Sprite normalCustomerSprite;    
        [SerializeField] private Sprite surprisedCustomerSprite; 

        [Header("【ゲームの進行・距離設定】")]
        // ▼追加：インスペクターでオバケまでの距離を自由に変えられます！
        [SerializeField] private float minGhostDistance = 3000f; 
        [SerializeField] private float maxGhostDistance = 5000f; 
        
        [SerializeField] private float memorizeTime = 1.0f;      // 記憶時間を少し短く（テンポアップ）
        [SerializeField] private float scrollSpeed = 800f;       // スクロール速度を速く
        [SerializeField] private float rewindSpeed = 4000f;      // 巻き戻しも速く
        
        // ▼追加：お客さんがサッと入場するように独立した速度を設定
        [SerializeField] private float customerEnterSpeed = 2000f; 
        
        [SerializeField] private float perfectDistance = 80f;  

        [Header("【歩行アニメーション設定】")]
        [SerializeField] private float walkBobbingSpeed = 15f;   
        [SerializeField] private float walkBobbingAmount = 10f;  

        private float customerInitialX; 
        private float customerInitialY; 
        
        private enum GameState { Memorize, Rewinding, CustomerEntering, Scrolling, Result }
        private GameState currentState;

        public override void OnGameStart()
        {
            MGManager.Load(); 
            customerInitialX = customerRect.anchoredPosition.x; 
            customerInitialY = customerRect.anchoredPosition.y; 
            ResetGame();
        }

        public override void OnGameEnd()
        {
        }

        private void ResetGame()
        {
            customerImage.sprite = normalCustomerSprite; 

            customerImage.enabled = true;
            customerRect.anchoredPosition = new Vector2(customerInitialX - 1500f, customerInitialY);

            // ◀変更：インスペクターで設定した距離の範囲でランダム配置！
            float randomX = Random.Range(minGhostDistance, maxGhostDistance); 
            ghostRect.anchoredPosition = new Vector2(randomX, ghostRect.anchoredPosition.y);
            backgroundRect.anchoredPosition = new Vector2(-randomX, backgroundRect.anchoredPosition.y);

            ghostImage.enabled = true;
            currentState = GameState.Memorize;
            StartCoroutine(MemorizeRoutine());
        }

        private IEnumerator MemorizeRoutine()
        {
            yield return new WaitForSeconds(memorizeTime);
            ghostImage.enabled = false;
            
            currentState = GameState.Rewinding;
            while (backgroundRect.anchoredPosition.x < 0)
            {
                backgroundRect.anchoredPosition = Vector2.MoveTowards(
                    backgroundRect.anchoredPosition, 
                    new Vector2(0, backgroundRect.anchoredPosition.y), 
                    rewindSpeed * Time.deltaTime
                );
                yield return null; 
            }
            backgroundRect.anchoredPosition = new Vector2(0, backgroundRect.anchoredPosition.y);

            currentState = GameState.CustomerEntering;
            while (customerRect.anchoredPosition.x < customerInitialX)
            {
                // ◀変更：入場専用の速いスピード（customerEnterSpeed）を使用！
                float newX = Mathf.MoveTowards(
                    customerRect.anchoredPosition.x, 
                    customerInitialX, 
                    customerEnterSpeed * Time.deltaTime
                );
                float newY = customerInitialY + Mathf.Sin(Time.time * walkBobbingSpeed) * walkBobbingAmount;
                customerRect.anchoredPosition = new Vector2(newX, newY);
                yield return null; 
            }

            customerRect.anchoredPosition = new Vector2(customerInitialX, customerInitialY);
            yield return new WaitForSeconds(0.2f);

            currentState = GameState.Scrolling;
        }

        private void Update()
        {
            if (currentState != GameState.Scrolling) return;

            backgroundRect.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);

            float newY = customerInitialY + Mathf.Sin(Time.time * walkBobbingSpeed) * walkBobbingAmount;
            customerRect.anchoredPosition = new Vector2(customerRect.anchoredPosition.x, newY);

            if (Action.WasPerformedThisFrame())
            {
                JudgeTiming();
            }
        }

        private void JudgeTiming()
        {
            currentState = GameState.Result;
            ghostImage.enabled = true;
            customerImage.sprite = surprisedCustomerSprite; 
            customerRect.anchoredPosition = new Vector2(customerInitialX, customerInitialY);

            float distance = Mathf.Abs(customerRect.position.x - ghostRect.position.x);
            Debug.Log("ズレの距離: " + distance);

            if (distance <= perfectDistance)
            {
                Debug.Log("完璧！");
                StartCoroutine(ClearSequence());
            }
            else
            {
                if (ghostRect.position.x > customerRect.position.x)
                    Debug.Log("早すぎ！");
                else
                    Debug.Log("遅すぎ！");
                    
                StartCoroutine(FailSequence());
            }
        }

        private IEnumerator ClearSequence()
        {
            yield return new WaitForSeconds(1.0f);
            MGManager.ClearGame(); 
        }

        private IEnumerator FailSequence()
        {
            yield return new WaitForSeconds(1.0f);
        }
    }
}