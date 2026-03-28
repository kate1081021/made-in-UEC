using System.Collections.Generic;
using UnityEngine;

namespace BOSS
{
    public enum ObstacleType
    {
        FallingObjects, 
        GhostBlindness, 
    }

    [System.Serializable]
    public class StageEvent
    {
        public float triggerTime;
        public ObstacleType obstacleType;
        public bool isActivate = true;
        [HideInInspector] public bool hasTriggered = false; 
    }

    public class BOSS_ObstacleDirector : MonoBehaviour
    {
        [Header("ステージ進行（タイムライン）設定")]
        public List<StageEvent> stageEvents = new List<StageEvent>();

        [Header("ターゲット情報（空欄なら自動で探します）")]
        [Tooltip("Tagが「Player」のオブジェクトを自動検索します")]
        public Transform playerTarget;

        [Header("【1】継続型のプレハブ（降る・撃つなど）")]
        [Tooltip("※シーン上の物ではなく、Project内の「プレハブ（青い箱）」をセットしてください")]
        public GameObject fallingObstacleManagerPrefab;

        [Header("【2】単発・生成型のプレハブ（お化け・罠など）")]
        public GameObject ghostPrefab;

        // 内部で生成・管理するための変数
        private BOSS_ObstacleManager spawnedFallingManager;
        private float elapsedTime = 0f;
        private bool isTimelineRunning = false;

        void Start()
        {
            // ① プレイヤーがセットされていなければ、Tagを使って自動で探す
            if (playerTarget == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTarget = playerObj.transform;
                }
                else
                {
                    Debug.LogWarning("Playerタグのついたオブジェクトが見つかりません！");
                }
            }

            // ② 降る障害物のマネージャー（プレハブ）がセットされていれば、見えない所で自動生成する
            if (fallingObstacleManagerPrefab != null)
            {
                // ヒエラルキーに自動で生み出す
                GameObject managerObj = Instantiate(fallingObstacleManagerPrefab, Vector3.zero, Quaternion.identity);
                spawnedFallingManager = managerObj.GetComponent<BOSS_ObstacleManager>();
            }

            StartTimeline();
        }

        void Update()
        {
            if (!isTimelineRunning) return;

            elapsedTime += Time.deltaTime;

            foreach (var stageEvent in stageEvents)
            {
                if (!stageEvent.hasTriggered && elapsedTime >= stageEvent.triggerTime)
                {
                    if (stageEvent.isActivate) ActivateObstacle(stageEvent.obstacleType);
                    else StopObstacle(stageEvent.obstacleType);
                    
                    stageEvent.hasTriggered = true;
                }
            }
        }

        public void StartTimeline() { isTimelineRunning = true; }
        public void PauseTimeline() { isTimelineRunning = false; }
        
        public void ResetTimeline()
        {
            isTimelineRunning = false;
            elapsedTime = 0f;
            foreach (var stageEvent in stageEvents) stageEvent.hasTriggered = false;
            StopAllObstacles();
        }

        public void ActivateObstacle(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.FallingObjects: StartFallingObjects(); break;
                case ObstacleType.GhostBlindness: SpawnGhost(); break;
            }
        }

        public void StopObstacle(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.FallingObjects: StopFallingObjects(); break;
            }
        }

        public void StopAllObstacles()
        {
            StopFallingObjects();
        }

        private void StartFallingObjects()
        {
            if (spawnedFallingManager != null) spawnedFallingManager.StartSpawning();
        }

        private void StopFallingObjects()
        {
            if (spawnedFallingManager != null) spawnedFallingManager.StopSpawning();
        }

        private void SpawnGhost()
        {
            if (ghostPrefab != null && playerTarget != null)
            {
                GameObject ghostObj = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
                BOSS_GhostObstacle ghostScript = ghostObj.GetComponent<BOSS_GhostObstacle>();
                if (ghostScript != null) ghostScript.Init(playerTarget);
            }
        }
    }
}