using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace plugmatch 
{
    public class PM_GameController : MiniGameBase 
    {
        [Header("プラグの親オブジェクト")]
        public GameObject plugObject;
        
        [Header("吹き出しの中の画像(Image)")]
        public Image bubbleShapeImage;

        [Header("画像リスト（9種類ずつ同じ順番で登録！）")]
        public Sprite[] plugSprites;
        public Sprite[] outletSprites;

        [Header("コンセント穴のリスト（左から順に4つ登録）")]
        public PM_Outlet[] outlets;

        [Header("差し込みアニメーション設定")]
        public float insertDistance = 150f; 
        public float insertSpeed = 2500f;   

        [Header("失敗演出設定")]
        public float shakeAmount = 10f; 
        public float shakeSpeed = 50f;  
        public Image failurePanel; 
        public Color failureColor = new Color(0f, 0f, 0.3f, 0.6f); 

        [Header("成功演出（電気製品ポップ）設定")]
        public float popScale = 1.3f;   
        public GameObject applianceObject; 
        public float appliancePopSpeed = 25f; 

        [Header("オーディオ設定（独自実装）")]
        public AudioSource bgmSource; 
        public AudioSource seSource;  

        public AudioClip bgmClip;     
        public AudioClip clearSeClip; 
        public AudioClip missSeClip;  
        public AudioClip moveSeClip;  

        [HideInInspector]
        public PM_ShapeType myPlugShape;

        private enum GameState { Playing, Inserting, Rejected, Cleared }
        private GameState currentState = GameState.Playing;

        private int currentIndex = 0;
        private bool isKeyPushed = false;
        
        private Vector3 startPos; 
        private Vector3 targetPos; 
        private bool isCorrectChoice;
        private Vector3 originalScale; 
        private Vector3 applianceOriginalScale; 

        // 吹き出し全体を固定するための変数
        private Transform bubbleParentTransform; 
        private Vector3 bubbleFixedPos; 
        private bool isBubbleFixed = false; 

        public override void OnGameStart() 
        {
            MGManager.Load(); 
            
            // ゲーム開始時にフラグをリセット
            isBubbleFixed = false;

            if (bgmSource != null && bgmClip != null)
            {
                bgmSource.clip = bgmClip;
                bgmSource.loop = true; 
                bgmSource.Play();
            }

            SetupRandomShapes(); 

            originalScale = plugObject.transform.localScale;

            if (applianceObject != null)
            {
                applianceOriginalScale = applianceObject.transform.localScale;
                applianceObject.transform.localScale = Vector3.zero;
                applianceObject.SetActive(false);
            }

            if (failurePanel != null)
            {
                failurePanel.color = Color.clear;
                failurePanel.gameObject.SetActive(false);
            }

            // ★追加：吹き出しの中身から見て「親（＝空の吹き出し）」を取得しておく
            if (bubbleShapeImage != null)
            {
                bubbleParentTransform = bubbleShapeImage.transform.parent;
            }

            if (outlets.Length > 0) 
            {
                UpdatePlugPosition();
            }
        }

        private void SetupRandomShapes()
        {
            List<PM_ShapeType> allShapes = new List<PM_ShapeType>();
            for(int i = 0; i < 9; i++) 
            {
                allShapes.Add((PM_ShapeType)i);
            }

            List<PM_ShapeType> selectedShapes = new List<PM_ShapeType>();
            for(int i = 0; i < 4; i++) 
            {
                int randomIndex = Random.Range(0, allShapes.Count);
                selectedShapes.Add(allShapes[randomIndex]);
                allShapes.RemoveAt(randomIndex);
            }

            for(int i = 0; i < outlets.Length; i++) 
            {
                outlets[i].outletShape = selectedShapes[i];
                outlets[i].outletImage.sprite = outletSprites[(int)selectedShapes[i]];
            }

            myPlugShape = selectedShapes[Random.Range(0, outlets.Length)];
            bubbleShapeImage.sprite = plugSprites[(int)myPlugShape];
        }

        void Update()
        {
            if (bgmSource != null)
            {
                bgmSource.pitch = Time.timeScale;
            }

            if (currentState == GameState.Playing)
            {
                float h = Move.ReadValue<Vector2>().x;

                if (h > 0.5f && !isKeyPushed) 
                {
                    currentIndex++;
                    if (currentIndex >= outlets.Length) currentIndex = 0; 
                    isKeyPushed = true;
                    UpdatePlugPosition();
                    PlaySE(moveSeClip); 
                }
                else if (h < -0.5f && !isKeyPushed) 
                {
                    currentIndex--;
                    if (currentIndex < 0) currentIndex = outlets.Length - 1; 
                    isKeyPushed = true;
                    UpdatePlugPosition();
                    PlaySE(moveSeClip); 
                }
                else if (Mathf.Abs(h) < 0.1f) 
                {
                    isKeyPushed = false; 
                }

                if (Action.WasPerformedThisFrame()) 
                {
                    isCorrectChoice = (outlets[currentIndex].outletShape == myPlugShape);
                    startPos = plugObject.transform.position;
                    targetPos = startPos + new Vector3(0, insertDistance, 0);
                    currentState = GameState.Inserting;
                }
            }
            else if (currentState == GameState.Inserting)
            {
                plugObject.transform.position = Vector3.MoveTowards(
                    plugObject.transform.position, targetPos, insertSpeed * Time.deltaTime);

                if (Vector3.Distance(plugObject.transform.position, targetPos) < 0.1f)
                {
                    if (isCorrectChoice)
                    {
                        MGManager.ClearGame();
                        currentState = GameState.Cleared;
                        plugObject.transform.localScale = originalScale * popScale;

                        PlaySE(clearSeClip); 

                        if (applianceObject != null)
                        {
                            applianceObject.SetActive(true); 
                        }
                    }
                    else
                    {
                        targetPos = startPos;
                        currentState = GameState.Rejected;
                        
                        PlaySE(missSeClip); 

                        if (failurePanel != null)
                        {
                            failurePanel.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (currentState == GameState.Rejected)
            {
                plugObject.transform.position = Vector3.MoveTowards(
                    plugObject.transform.position, targetPos, insertSpeed * Time.deltaTime);
                
                if (Vector3.Distance(plugObject.transform.position, targetPos) < 0.1f)
                {
                    // ▼ 変更：「空の吹き出し（親）」の座標を記憶する
                    if (!isBubbleFixed && bubbleParentTransform != null)
                    {
                        bubbleFixedPos = bubbleParentTransform.position;
                        isBubbleFixed = true; 
                    }

                    // プラグ本体を揺らす
                    float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
                    plugObject.transform.position = targetPos + new Vector3(shakeOffset, 0, 0);

                    // ▼ 変更：「空の吹き出し（親）」を記憶した位置に上書き固定する
                    // （親を固定すれば、子である中身の図形も一緒に固定されます）
                    if (isBubbleFixed && bubbleParentTransform != null)
                    {
                        bubbleParentTransform.position = bubbleFixedPos;
                    }
                }

                if (failurePanel != null)
                {
                    failurePanel.color = Color.Lerp(failurePanel.color, failureColor, Time.deltaTime * 10f);
                }
            }
            else if (currentState == GameState.Cleared)
            {
                plugObject.transform.localScale = Vector3.Lerp(
                    plugObject.transform.localScale, originalScale, Time.deltaTime * 15f);

                if (applianceObject != null)
                {
                    applianceObject.transform.localScale = Vector3.Lerp(
                        applianceObject.transform.localScale, applianceOriginalScale, Time.deltaTime * appliancePopSpeed);
                }
            }
        }

        private void UpdatePlugPosition()
        {
            Vector3 targetHolePos = outlets[currentIndex].transform.position;
            plugObject.transform.position = new Vector3(targetHolePos.x, plugObject.transform.position.y, plugObject.transform.position.z);
        }

        private void PlaySE(AudioClip clip)
        {
            if (seSource != null && clip != null)
            {
                seSource.PlayOneShot(clip);
            }
        }

        public override void OnGameEnd() 
        {
        }
    }
}