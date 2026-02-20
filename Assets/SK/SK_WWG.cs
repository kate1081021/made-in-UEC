using UnityEngine;
using System.Collections.Generic;

namespace SK
{
    public class SK_WWG : MiniGameBase
    {
        // ★ボタンタイプは L と R のみ
        public enum ButtonType { Left, Right }

        [Header("References")]
        public SK_WarpedWallPlayer player; 
        public Transform spawnPoint;
        public Transform targetZone;
        public GameObject notePrefab; 

        [Header("Audio Settings")]
        public AudioSource bgmSource;   // BGM
        public AudioSource audioSource; // SE
        public AudioClip voiceStart;    
        public AudioClip sfxStepPerfect;
        public AudioClip sfxStepGood;   
        public AudioClip sfxMiss;       
        public AudioClip sfxJump;       

        [Header("Note Images")]
        public Sprite spriteLeft;   // L画像
        public Sprite spriteRight;  // R画像

        [Header("Game Settings")]
        public float[] spawnTimings = { 1.0f, 2.0f, 2.8f, 3.5f, 4.1f, 4.8f };
        public float perfectDistance = 80f;
        public float momentumThreshold = 60f;

        private ButtonType[] spawnTypes;
        private float timer = 0f;
        private int noteIndex = 0;
        private List<GameObject> activeNotes = new List<GameObject>();
        
        private float currentMomentum = 0f;
        private int currentStep = 0;
        private bool isGameActive = false;

        public override void OnGameStart()
        {
            MGManager.TestPlay(1);
            MGManager.Load();

            isGameActive = true;
            timer = 0f;
            noteIndex = 0;
            currentMomentum = 0f;
            currentStep = 0;
            
            foreach(var note in activeNotes) { if(note) Destroy(note); }
            activeNotes.Clear();

            // パターン生成 (L, R, L, R...)
            GenerateFixedPattern();

            if (player != null) player.Initialize(this);
            PlaySound(voiceStart);
        }

        public override void OnGameEnd()
        {
            isGameActive = false;
        }

        void GenerateFixedPattern()
        {
            // LとRのみの交互配置
            spawnTypes = new ButtonType[] {
                ButtonType.Left,  
                ButtonType.Right, 
                ButtonType.Left,  
                ButtonType.Right, 
                ButtonType.Left,  
                ButtonType.Right  // 最後
            };
        }

        void Update()
        {
            if (!isGameActive) return;

            timer += Time.deltaTime;

            if (noteIndex < spawnTimings.Length && noteIndex < spawnTypes.Length)
            {
                if (timer >= spawnTimings[noteIndex])
                {
                    SpawnNote(noteIndex);
                    noteIndex++;
                }
            }
        }

        // 入力受け取り (Left / Right)
        protected override void OnTriggerStarted(float value) 
        { 
            if (!isGameActive) return;

            if (value < -0.1f) // 左入力
            {
                CheckInput(ButtonType.Left);
            }
            else if (value > 0.1f) // 右入力
            {
                CheckInput(ButtonType.Right);
            }
        }

        protected override void OnTriggerPerformed(float value) { }
        protected override void OnTriggerCanceled(float value) { }

        void SpawnNote(int index)
        {
            GameObject note = Instantiate(notePrefab, spawnPoint);
            note.transform.SetParent(spawnPoint.parent, false);

            SK_NoteMover mover = note.GetComponent<SK_NoteMover>();
            
            ButtonType type = spawnTypes[index];
            Sprite targetSprite = (type == ButtonType.Left) ? spriteLeft : spriteRight;

            if (mover != null) mover.Setup(this, type, targetSprite);

            activeNotes.Add(note);
        }

        void CheckInput(ButtonType inputType)
        {
            if (activeNotes.Count == 0) return;

            GameObject targetObj = activeNotes[0];
            if (targetObj == null) { activeNotes.RemoveAt(0); return; }

            SK_NoteMover targetNote = targetObj.GetComponent<SK_NoteMover>();

            // 1. タイプ不一致 (LなのにRを押した)
            if (targetNote.noteType != inputType)
            {
                PlaySound(sfxMiss);
                player.FallDown();
                return; 
            }

            // ★正解タイプなので、見た目を変える
            targetNote.OnInputReceived();

            // 2. 距離判定
            float distance = Mathf.Abs(targetObj.transform.localPosition.x - targetZone.localPosition.x);

            if (distance <= perfectDistance)
            {
                OnHitSuccess(distance, targetObj);
            }
            else
            {
                player.FallDown();
                PlaySound(sfxMiss);
            }
        }

        void OnHitSuccess(float distance, GameObject note)
        {
            float ratio = distance / perfectDistance;
            float accuracy = 1.0f - ratio;
            if (accuracy < 0) accuracy = 0;
            
            currentMomentum += accuracy * 20f;

            if (ratio <= 0.4f) PlaySound(sfxStepPerfect);
            else PlaySound(sfxStepGood);

            if (currentStep == 5) // 最後
            {
                if (currentMomentum >= momentumThreshold)
                {
                    // 成功
                    if (bgmSource != null) bgmSource.Stop();
                    if (sfxJump != null) PlaySound(sfxJump);
                    
                    player.StepForward(true);
                    MGManager.ClearGame();
                    OnGameEnd();
                }
                else
                {
                    // 失敗
                    if (bgmSource != null) bgmSource.Stop();
                    player.FallDown();
                    OnGameEnd();
                }
            }
            else if (currentStep == 4)
            {
                player.StepForward(false);
                player.PlayPreJumpAction();
            }
            else
            {
                player.StepForward(false);
            }

            currentStep++;
            // 少し遅らせて消すと色が確認しやすいが、即消しでもOK
            Destroy(note, 0.1f); 
            activeNotes.RemoveAt(0);
        }

        public float GetSpeedMultiplier()
        {
            return Time.timeScale;
        }

        void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}