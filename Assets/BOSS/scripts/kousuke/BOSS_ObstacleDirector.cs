using System.Collections.Generic;
using UnityEngine;

namespace BOSS
{
    public enum ObstacleType
    {
        FallingObjects, 
        GhostBlindness, 
        BombThrow, 
        Boomerang, 
        Baloon, 
        Snake, 
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

        [Header("ターゲット情報（空欄なら自動検索）")]
        public Transform playerTarget;

        [Header("【1】継続型のプレハブ（ON/OFF切り替え）")]
        public GameObject fallingObstacleManagerPrefab;

        [Header("【2】単発・生成型のプレハブ（ポンと出すだけ）")]
        public GameObject ghostPrefab;
        public GameObject bombPrefab; 
        public GameObject baloonPrefab; 

        [Header("【3】ブーメラン設定（ディレクター内蔵型）")]
        public GameObject boomerangPrefab; 
        [Tooltip("何秒に1個落とすか")]
        public float boomerangSpawnInterval = 2.0f; 

        [Header("【4】ヘビ（蛇行）設定（ディレクター内蔵型）")]
        public GameObject snakePrefab; 
        [Tooltip("何秒に1個落とすか")]
        public float snakeSpawnInterval = 2.0f; 

        private BOSS_ObstacleManager spawnedFallingManager;
        private float elapsedTime = 0f;
        private bool isTimelineRunning = false;

        private bool isBoomerangActive = false;
        private float boomerangTimer = 0f;

        private bool isSnakeActive = false;
        private float snakeTimer = 0f;

        void Start()
        {
            if (playerTarget == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) playerTarget = playerObj.transform;
            }

            if (fallingObstacleManagerPrefab != null)
            {
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

            if (isBoomerangActive)
            {
                boomerangTimer += Time.deltaTime;
                if (boomerangTimer >= boomerangSpawnInterval)
                {
                    SpawnBoomerang();
                    boomerangTimer = 0f;
                }
            }

            if (isSnakeActive)
            {
                snakeTimer += Time.deltaTime;
                if (snakeTimer >= snakeSpawnInterval)
                {
                    SpawnSnake();
                    snakeTimer = 0f;
                }
            }
        }

        public void StartTimeline() { isTimelineRunning = true; }
        public void PauseTimeline() { isTimelineRunning = false; }
        
        public void ActivateObstacle(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.FallingObjects: StartFallingObjects(); break;
                case ObstacleType.GhostBlindness: SpawnGhost(); break;
                case ObstacleType.BombThrow:
                    if (bombPrefab != null) Instantiate(bombPrefab, Vector3.zero, Quaternion.identity);
                    break;
                case ObstacleType.Boomerang: StartBoomerang(); break;
                
                // ★修正箇所：ど真ん中（Vector3.zero）に出すのをやめ、プレハブの保存位置を使います！
                case ObstacleType.Baloon: 
                    if (baloonPrefab != null) Instantiate(baloonPrefab, baloonPrefab.transform.position, baloonPrefab.transform.rotation);
                    break;

                case ObstacleType.Snake: StartSnake(); break; 
            }
        }

        public void StopObstacle(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.FallingObjects: StopFallingObjects(); break;
                case ObstacleType.Boomerang: StopBoomerang(); break; 
                case ObstacleType.Snake: StopSnake(); break; 
            }
        }

        public void StopAllObstacles()
        {
            StopFallingObjects();
            StopBoomerang();
            StopSnake(); 
        }

        private void StartFallingObjects() { if (spawnedFallingManager != null) spawnedFallingManager.StartSpawning(); }
        private void StopFallingObjects() { if (spawnedFallingManager != null) spawnedFallingManager.StopSpawning(); }

        private void StartBoomerang() { isBoomerangActive = true; boomerangTimer = boomerangSpawnInterval; }
        private void StopBoomerang() { isBoomerangActive = false; }

        private void StartSnake() { isSnakeActive = true; snakeTimer = snakeSpawnInterval; }
        private void StopSnake() { isSnakeActive = false; }

        private void SpawnSnake()
        {
            if (snakePrefab != null)
            {
                float topY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;
                float randomX = Random.Range(-6.0f, 6.0f); 
                Vector3 spawnPos = new Vector3(randomX, topY + 2.0f, 0);
                Instantiate(snakePrefab, spawnPos, Quaternion.identity);
            }
        }

        private void SpawnBoomerang()
        {
            if (boomerangPrefab != null)
            {
                float topY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;
                float randomX = Random.Range(-7.0f, 7.0f); 
                Vector3 spawnPos = new Vector3(randomX, topY + 2.0f, 0);
                Instantiate(boomerangPrefab, spawnPos, Quaternion.identity);
            }
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