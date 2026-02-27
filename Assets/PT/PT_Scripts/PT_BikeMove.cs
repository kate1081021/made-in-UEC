using UnityEngine;

namespace PTgame
{
    public class PT_BikeMove : MonoBehaviour
    {
        public float speed = 5f;
        private float direction; // 1 = 右, -1 = 左
        private float destroyX;

        public void Init(float dir)
        {
            direction = dir;

            if (dir > 0)
                destroyX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 1f;
            else
                destroyX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 1f;
        }

        void Update()
        {
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

            if (direction > 0 && transform.position.x > destroyX)
                Destroy(gameObject);

            if (direction < 0 && transform.position.x < destroyX)
                Destroy(gameObject);
        }
    }
}