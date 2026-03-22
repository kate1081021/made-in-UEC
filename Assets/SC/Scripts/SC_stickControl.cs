using UnityEngine;

namespace SC
{

    public class SC_stickControl : MiniGameBase 
    {
        [System.Serializable]
        public class StickData
        {
            public Sprite sprite;
            public int ratio = 1;
            public float grabRange = 50f;
        }

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        [Header("オブジェクト類")]
        public SC_handControl handController;

        [Header("Stick Data")]  
        [SerializeField] private StickData[] stickDatas;

        public override void OnGameStart()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb != null) {
                rb.simulated = true;
            }

            int ratioSum = 0;
            for (int i = 0; i < stickDatas.Length; i++)
            {
                ratioSum += stickDatas[i].ratio;
            }
            int randomValue = Random.Range(0, ratioSum);
            Debug.Log("Random Value: " + randomValue);
            int nowRatio = 0;
            for (int i = 0; i < stickDatas.Length; i++)
            {
                nowRatio += stickDatas[i].ratio;
                if (randomValue < nowRatio)
                {
                    spriteRenderer.sprite = stickDatas[i].sprite;
                    handController.grabRange = stickDatas[i].grabRange;
                    break;
                }
            }
        }

        public override void OnGameEnd() { }

        public void StopStick()
        {
            if (rb != null)
            {
                rb.simulated = false;
            }
        }

        public void BounceStick()
        {
            if (rb != null)
            {
                float bounceForceY = 5f;
                float bounceForceX = Random.Range(-3f, 3f);
                rb.linearVelocity = new Vector2(bounceForceX, bounceForceY);

                float torqueForce = Random.Range(-150f, 150f); 
                rb.AddTorque(torqueForce);
            }
        }
    }
}
