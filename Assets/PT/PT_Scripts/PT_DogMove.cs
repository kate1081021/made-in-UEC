using UnityEngine;
using System.Collections; 

namespace PTgame
{
    public class PT_DogMove : MonoBehaviour
    {
        public float speed = 5f;
        private float direction; // 1 = 右, -1 = 左
        private float destroyX;

        public float stopTime = 0.5f; 
        private bool isStopping = false;

        private float dog_big_height = 0.8f;

        private PT_Dog parentDog; 

        public void Init(float dir,PT_Dog dog)
        {
            direction = dir;
            parentDog = dog;

            // 子オブジェクト（Dog_Visual）を直接探して向きを変える
            Transform visual = transform.Find("Dog_Visual");
            if (visual != null)
            {
                Vector3 s = visual.localScale;
                // dirが-1ならXスケールをマイナスにして反転させる
                visual.localScale = new Vector3(-Mathf.Abs(s.x) * dir, s.y, s.z);
                
                // 犬巨大化
                if (Random.Range(0f, 1f) > 0.95f)
                {
                    Vector3 p = this.transform.localPosition;
                    this.transform.localPosition = new Vector3(p.x, p.y + dog_big_height, p.z);
                    visual.localScale = new Vector3(-Mathf.Abs(s.x) * dir * 2f, s.y * 2f, s.z);
                }
                
            }

            if (dir > 0)
                destroyX = Camera.main.ViewportToWorldPoint(new Vector3(0.4f, 0, 0)).x;
            else
                destroyX = Camera.main.ViewportToWorldPoint(new Vector3(0.6f, 0, 0)).x;
        }

        void Update()
        {
            if (isStopping) return; 

            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

            if (direction > 0 && transform.position.x > destroyX)
                StartCoroutine(StopAndDestroy());

            if (direction < 0 && transform.position.x < destroyX)
                StartCoroutine(StopAndDestroy());
        }

        IEnumerator StopAndDestroy()
        {
            isStopping = true; // 移動停止

            yield return new WaitForSeconds(stopTime);

            if (parentDog != null)
                parentDog.ActiveBark(direction);

            Debug.Log("呼び出し");

            Destroy(gameObject);
        }
    }
}