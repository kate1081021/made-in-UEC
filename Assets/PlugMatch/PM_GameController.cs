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

        // ▼ 追加：プログラムから直接音量を指定するスライダー
        [Range(0f, 1f)] public float bgmVolume = 0.2f; // BGMの音量（デフォルト0.2）
        [Range(0f, 1f)] public float seVolume = 0.4f;  // SEの音量（デフォルト0.4）

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

        private Transform bubbleParentTransform; 
        private Vector3 bubbleFixedPos; 
        private bool isBubbleFixed = false; 

        public override void OnGameStart() 
        {
            MGManager.Load(); 
            
            isBubbleFixed = false;
            BGMPlay();
            Debug.Log(MGManager.pitchScale);
            /*
            if (bgmSource != null && bgmClip != null)
            {
                bgmSource.clip = bgmClip;
                bgmSource.loop = true; 
                // ★追加：BGMの音量を強制的に指定した数値にする
                bgmSource.volume = bgmVolume; 
                bgmSource.Play();
            }
            */

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
                    SEPlay("PM_Move");
                    //PlaySE(moveSeClip); 
                }
                else if (h < -0.5f && !isKeyPushed) 
                {
                    currentIndex--;
                    if (currentIndex < 0) currentIndex = outlets.Length - 1; 
                    isKeyPushed = true;
                    UpdatePlugPosition();
                    SEPlay("PM_Move");
                    //PlaySE(moveSeClip); 
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
                        SEPlay("PM_Clear");
                        //PlaySE(clearSeClip); 

                        if (applianceObject != null)
                        {
                            applianceObject.SetActive(true); 
                        }
                    }
                    else
                    {
                        targetPos = startPos;
                        currentState = GameState.Rejected;
                        
                        SEPlay("PM_Miss");
                        //PlaySE(missSeClip); 

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
                    if (!isBubbleFixed && bubbleParentTransform != null)
                    {
                        bubbleFixedPos = bubbleParentTransform.position;
                        isBubbleFixed = true; 
                    }

                    float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
                    plugObject.transform.position = targetPos + new Vector3(shakeOffset, 0, 0);

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
                // ★変更：指定したSEの音量（seVolume）で強制的に鳴らす
                seSource.PlayOneShot(clip, seVolume);
            }
        }

        public override void OnGameEnd() 
        {
        }
    }
}