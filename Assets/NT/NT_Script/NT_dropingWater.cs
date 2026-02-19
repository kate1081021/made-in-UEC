using UnityEngine;

namespace NT {
    public class NT_dropingWater : MiniGameBase
    {
        [Header("NT")]
        [SerializeField] private float finishPointY;

        public override void OnGameStart() {}
        public override void OnGameEnd() {}

        void Update()
        {
            if (transform.position.y < finishPointY)
            {
                Destroy(gameObject);
            }
        }
    }
}