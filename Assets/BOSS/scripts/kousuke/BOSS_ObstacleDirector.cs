using System.Collections.Generic;
using UnityEngine;

namespace BOSS
{
    public enum ObstacleType
    {
        FallingObjects, 
        GhostBlindness, 
        // BombThrow は削除しました
        Boomerang, 
        Baloon, 
        Snake, 
        Wind, 
    }

    [System.Serializable]
    public class StageEvent
    {
        public float triggerTime;
        public ObstacleType obstacleType;
        public bool isActivate = true;
        [HideInInspector] public bool hasTriggered = false; 
    }

    public class BOSS_ObstacleDirector : MiniGameBase
    {
        [Header("ステージ進行設定")]
        public List<StageEvent> stageEvents = new List<StageEvent>();
        public Transform playerTarget;

        [Header("【1】継続型のプレハブ")]
        public GameObject fallingObstacleManagerPrefab;
        public GameObject windControllerPrefab; 

        [Header("【2】単発型のプレハブ")]
        public GameObject ghostPrefab;
        // 爆弾のプレハブ枠は削除しました
        public GameObject baloonPrefab; 

        [Header("【3】ブーメラン設定")]
        public GameObject boomerangPrefab; 
        public float boomerangSpawnInterval = 2.0f; 

        [Header("【4】ヘビ設定")]
        public GameObject snakePrefab; 
        public float snakeSpawnInterval = 2.0f; 

        private BOSS_ObstacleManager spawnedFallingManager;
        private BOSS_WindController spawnedWindController; 

        private float elapsedTime = 0f;
        private bool isTimelineRunning = false;
        private bool isBoomerangActive = false;
        private float boomerangTimer = 0f;
        private bool isSnakeActive = false;
        private float snakeTimer = 0f;

        public override void OnGameStart()
        {
            MGManager.Load();

            if (playerTarget == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) playerTarget = playerObj.transform;
            }

            if (fallingObstacleManagerPrefab != null)
            {
                spawnedFallingManager = Instantiate(fallingObstacleManagerPrefab, Vector3.zero, Quaternion.identity).GetComponent<BOSS_ObstacleManager>();
            }
            
            if (windControllerPrefab != null)
            {
                spawnedWindController = Instantiate(windControllerPrefab, Vector3.zero, Quaternion.identity).GetComponent<BOSS_WindController>();
                spawnedWindController.Init(); 
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
                if (boomerangTimer >= boomerangSpawnInterval) { SpawnBoomerang(); boomerangTimer = 0f; }
            }

            if (isSnakeActive)
            {
                snakeTimer += Time.deltaTime;
                if (snakeTimer >= snakeSpawnInterval) { SpawnSnake(); snakeTimer = 0f; }
            }
        }

        public void StartTimeline() { isTimelineRunning = true; }
        public void PauseTimeline() { isTimelineRunning = false; }
        
        public void ActivateObstacle(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.FallingObjects: if(spawnedFallingManager) spawnedFallingManager.StartSpawning(); break;
                case ObstacleType.GhostBlindness: SpawnGhost(); break;
                // 爆弾の処理は削除しました
                case ObstacleType.Boomerang: isBoomerangActive = true; boomerangTimer = boomerangSpawnInterval; break;
                case ObstacleType.Baloon: if (baloonPrefab) Instantiate(baloonPrefab, baloonPrefab.transform.position, baloonPrefab.transform.rotation); break;
                case ObstacleType.Snake: isSnakeActive = true; snakeTimer = snakeSpawnInterval; break; 
                case ObstacleType.Wind: if(spawnedWindController) spawnedWindController.StartWind(playerTarget); break;
            }
        }

        public void StopObstacle(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.FallingObjects: if(spawnedFallingManager) spawnedFallingManager.StopSpawning(); break;
                case ObstacleType.Boomerang: isBoomerangActive = false; break; 
                case ObstacleType.Snake: isSnakeActive = false; break; 
                case ObstacleType.Wind: if(spawnedWindController) spawnedWindController.StopWind(); break;
            }
        }

        private void SpawnSnake()
        {
            if (snakePrefab != null)
            {
                float topY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;
                float randomX = Random.Range(-6.0f, 6.0f); 
                Instantiate(snakePrefab, new Vector3(randomX, topY + 2.0f, 0), Quaternion.identity);
            }
        }

        private void SpawnBoomerang()
        {
            if (boomerangPrefab != null)
            {
                float topY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;
                float randomX = Random.Range(-7.0f, 7.0f); 
                Instantiate(boomerangPrefab, new Vector3(randomX, topY + 2.0f, 0), Quaternion.identity);
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