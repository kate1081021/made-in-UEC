using UnityEngine;

namespace NT {
    public class NT_dropingWater : MiniGameBase
    {
        private SpriteRenderer waterSpriteRenderer;
        AudioSource audioSource;
        public bool frag=false;
        [Header("NT")]
        [SerializeField] private float destroyPointY;
        [SerializeField] private float finishPointY;

        public override void OnGameStart() {
            waterSpriteRenderer = GetComponent<SpriteRenderer>();
            //audioSource = GetComponent<AudioSource>();
        }
        public override void OnGameEnd() {}

        void Update()
        {
            if (transform.position.y <= finishPointY && frag == false && audioSource != null)
            {
                frag = true;
                SEPlay("NT_dropping");
                //audioSource.PlayOneShot(audioSource.clip);
            }
            if (transform.position.y < finishPointY)
            {
                waterSpriteRenderer.color = new Color(waterSpriteRenderer.color.r, waterSpriteRenderer.color.g, waterSpriteRenderer.color.b, 0);
            }
            if (transform.position.y <= destroyPointY)
            {
                Destroy(gameObject);
            }
        }
    }
}