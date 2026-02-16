using UnityEngine;
using System.Collections.Generic;

namespace SK
{
    // ★ファイル名と一致させる
    public class SK_WWG : MiniGameBase
    {
        [Header("References")]
        // ★ここを修正：型を「SK_WarpedWallPlayer」に変更
        public SK_WarpedWallPlayer player; 
        
        public Transform spawnPoint;
        public Transform targetZone;
        public GameObject noteRunPrefab;
        public GameObject noteGrabPrefab;

        [Header("Game Settings")]
        public float[] spawnTimings = { 1.0f, 2.0f, 2.8f, 3.5f, 4.1f, 4.8f };
        public float perfectDistance = 80f;
        public float momentumThreshold = 60f;

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

            // プレイヤー初期化
            if (player != null) player.Initialize(this);
        }

        public override void OnGameEnd()
        {
            isGameActive = false;
        }

        void Update()
        {
            if (!isGameActive) return;

            timer += Time.deltaTime;

            if (noteIndex < spawnTimings.Length)
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
            CheckTiming();
        }

        protected override void OnTriggerPerformed(float value) { }
        protected override void OnTriggerCanceled(float value) { }

        void SpawnNote(int index)
        {
            GameObject prefab = (index == 5) ? noteGrabPrefab : noteRunPrefab;
            
            GameObject note = Instantiate(prefab, spawnPoint);
            note.transform.SetParent(spawnPoint.parent, false);

            // ★ここを修正：型を「SK_NoteMover」に変更
            SK_NoteMover mover = note.GetComponent<SK_NoteMover>();
            if (mover != null) mover.Setup(this);

            activeNotes.Add(note);
        }

        void CheckTiming()
        {
            if (activeNotes.Count == 0) return;

            GameObject targetNote = activeNotes[0];
            if (targetNote == null)
            {
                activeNotes.RemoveAt(0);
                return;
            }

            float distance = Mathf.Abs(targetNote.transform.localPosition.x - targetZone.localPosition.x);

            if (distance <= perfectDistance)
            {
                OnHitSuccess(distance, targetNote);
            }
            else
            {
                Debug.Log("Miss timing");
            }
        }

        void OnHitSuccess(float distance, GameObject note)
        {
            float accuracy = 1.0f - (distance / perfectDistance);
            if (accuracy < 0) accuracy = 0;
            float points = accuracy * 20f;
            currentMomentum += points;

            if (currentStep == 5)
            {
                if (currentMomentum >= momentumThreshold)
                {
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
            else
            {
                player.StepForward(false);
            }

            currentStep++;
            Destroy(note);
            activeNotes.RemoveAt(0);
        }

        public float GetSpeedMultiplier()
        {
            return Time.timeScale;
        }
    }
}