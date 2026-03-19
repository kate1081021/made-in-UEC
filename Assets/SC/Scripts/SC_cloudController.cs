using UnityEngine;

namespace SC{
    public class SC_couldController : MiniGameBase
    {
        [Header("パラメータ(空欄で問題ない)")]

        public float speed;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;

        public override void OnGameStart() {}

        void Update()
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (transform.position.x < minX)
            {
                Vector3 pos = transform.position;
                pos.x = maxX;
                pos.y = Random.Range(minY, maxY);
                transform.position = pos;
            }
        }
    }
}