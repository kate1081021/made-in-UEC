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

            Transform visual = transform.Find("Bike1_visual");

            if (visual != null)
            {
                // たまにbikeが曲芸する
                if (Random.Range(0f, 1f) > 0.95f)
                {
                    visual.rotation = Quaternion.Euler(0f, 0f, -15f);
                }

                Vector3 s = visual.localScale;
                if (Random.Range(0f,1f) > 0.05f)
                {
                    // dirが-1ならXスケールをマイナスにして反転させる
                    visual.localScale = new Vector3(-Mathf.Abs(s.x) * dir, s.y, s.z);
                }
                else
                {
                    visual.localScale = new Vector3(Mathf.Abs(s.x) * dir, s.y, s.z);
                }
            }

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