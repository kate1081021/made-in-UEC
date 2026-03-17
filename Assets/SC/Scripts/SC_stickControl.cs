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
        }

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;      
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
    }
}
