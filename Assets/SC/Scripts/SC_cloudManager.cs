using UnityEngine;

namespace SC{
    public class SC_cloudManager : MiniGameBase
    {
        [Header("オブジェクト類")]
        public GameObject cloudPrefab;

        [Header("パラメータ")]
        public int cloudAmount = 3;
        public float minSpeed = 0.5f;
        public float maxSpeed = 1.5f;
        public float minX = 0f;
        public float maxX = 1f;
        public float minY = 0f;
        public float maxY = 1f;
        public float minAlpha = 0.3f;
        public float maxAlpha = 0.7f;

        public override void OnGameStart()
        {
            int stage = MGManager.stage;
            if (stage >= 30)
            {
                minY -= 4;
                maxY += 4;
                cloudAmount += 30;
            }
            else if (stage >= 20)
            {
                minY -= 2;
                cloudAmount += 10;
            }
            else if (stage >= 10)
            {
                minY -= 1;
            }

            for (int i = 0; i < cloudAmount; i++)
            {
                SpawnCloud();
            }
        }

        private void SpawnCloud()
        {
            float yPos = Random.Range(minY, maxY);
            float xPos = Random.Range(minX, maxX);
            GameObject cloud = Instantiate(cloudPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, transform);

            SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = Random.Range(minAlpha, maxAlpha);
                sr.color = c;
            }
            SC_couldController cloudController = cloud.GetComponent<SC_couldController>();
            cloudController.speed = Random.Range(minSpeed, maxSpeed);
            cloudController.minX = minX;
            cloudController.maxX = maxX;
            cloudController.minY = minY;
            cloudController.maxY = maxY;
        }
    }
}