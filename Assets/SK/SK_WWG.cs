using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video; 

namespace SK
{
    public class SK_WWG : MiniGameBase
    {
        public enum ButtonType { Left, Right }

        [Header("Video Settings")]
        public VideoPlayer targetVideo; 

        [Header("References")]
        public SK_WarpedWallPlayer player; 
        public Transform spawnPoint;
        public Transform targetZone;
        public GameObject notePrefab; 

        [Header("Audio Settings")]
        // ★BGMは運営システムを使うため削除。SE（効果音）用のみ残します
        public AudioSource audioSource; 
        
        public AudioClip voiceStart;    
        public AudioClip sfxStepPerfect;
        public AudioClip sfxStepGood;   
        public AudioClip sfxMiss;       
        public AudioClip sfxJump;       

        [Header("Note Images")]
        public Sprite spriteLeft;   
        public Sprite spriteRight;  

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
            MGManager.TestPlay(100);
            MGManager.Load(); 

            if (targetVideo != null)
            {
                targetVideo.Play();
                targetVideo.playbackSpeed = 1.0f * Time.timeScale; 
            }

            isGameActive = true;
            timer = 0f;
            noteIndex = 0;
            currentMomentum = 0f;
            currentStep = 0;
            
            foreach(var note in activeNotes) { if(note) Destroy(note); }
            activeNotes.Clear();

            GenerateFixedPattern();

            if (player != null) player.Initialize(this);

            // ★BGMは運営システムを使用（falseなのでゲーム速度が上がってもBGMの速さは変わりません）
            BGMPlay(true); 
            
            // ★SEは独自のAudioSourceで再生（0.5fなどで音量調整可能）
            PlaySound(voiceStart, 1.0f); 
        }

        public override void OnGameEnd()
        {
            isGameActive = false;
        }

        void GenerateFixedPattern()
        {
            spawnTypes = new ButtonType[] {
                ButtonType.Left,  
                ButtonType.Right, 
                ButtonType.Left,  
                ButtonType.Right, 
                ButtonType.Left,  
                ButtonType.Right  
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

        protected override void OnTriggerStarted(float value) 
        { 
            if (!isGameActive) return;

            if (value < -0.1f) CheckInput(ButtonType.Left);
            else if (value > 0.1f) CheckInput(ButtonType.Right);
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

            if (targetNote.noteType != inputType)
            {
                PlaySound(sfxMiss, 1.0f); // 失敗音
                player.FallDown();
                
                
                OnGameEnd(); 
                return; 
            }

            targetNote.OnInputReceived();

            float distance = Mathf.Abs(targetObj.transform.localPosition.x - targetZone.localPosition.x);

            if (distance <= perfectDistance)
            {
                OnHitSuccess(distance, targetObj);
            }
            else
            {
                PlaySound(sfxMiss, 1.0f); 
                player.FallDown();
                
                OnGameEnd(); 
            }
        }

        void OnHitSuccess(float distance, GameObject note)
        {
            float ratio = distance / perfectDistance;
            float accuracy = 1.0f - ratio;
            if (accuracy < 0) accuracy = 0;
            
            currentMomentum += accuracy * 20f;

            if (ratio <= 0.4f) PlaySound(sfxStepPerfect, 1.0f);
            else PlaySound(sfxStepGood, 1.0f);

            if (currentStep == 5) // 最後
            {
                if (currentMomentum >= momentumThreshold)
                {
                    if (sfxJump != null) PlaySound(sfxJump, 1.0f);
                    
                    if (targetVideo != null) targetVideo.Pause(); 
                    
                    
                    player.StepForward(true);
                    MGManager.ClearGame(); 
                    OnGameEnd();
                }
                else
                {
                    
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
            Destroy(note, 0.1f); 
            activeNotes.RemoveAt(0);
        }

        public float GetSpeedMultiplier()
        {
            return Time.timeScale;
        }

        // ★第2引数で音量(volume)を指定できるPlaySound関数
        void PlaySound(AudioClip clip, float volume = 1.0f)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }
    }
}