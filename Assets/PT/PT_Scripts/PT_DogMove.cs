using UnityEngine;
using System.Collections;

namespace PTgame
{
    public class PT_DogMove : MonoBehaviour
    {
        public float speed = 5f;
        private float direction; // 1 = 右, -1 = 左
        private float stopX;
        private float destroyX;
        private bool isBack = false;

        public float stopTime = 0.5f;
        private bool isStopping = false;

        private float dog_big_height = 0.8f;

        private PT_Dog parentDog;

        public void Init(float dir, PT_Dog dog)
        {
            direction = dir;
            parentDog = dog;

            if (dir > 0)
                stopX = Camera.main.ViewportToWorldPoint(new Vector3(0.4f, 0, 0)).x;
            else
                stopX = Camera.main.ViewportToWorldPoint(new Vector3(0.6f, 0, 0)).x;

            // --- 修正箇所：消える位置を「進む方向の先」に設定 ---
            if (dir > 0)
                destroyX = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0)).x; // 右端の外
            else
                destroyX = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0, 0)).x; // 左端の外

            Transform visual = transform.Find("Dog_Visual");
            if (visual != null)
            {
                Vector3 s = visual.localScale;
                visual.localScale = new Vector3(-Mathf.Abs(s.x) * dir, s.y, s.z);
                
                // 犬巨大化
                if (Random.Range(0f, 1f) > 0.95f)
                {
                    Vector3 p = this.transform.localPosition;
                    this.transform.localPosition = new Vector3(p.x, p.y + dog_big_height, p.z);
                    visual.localScale = new Vector3(-Mathf.Abs(s.x) * dir * 2.0f, s.y * 2.0f, s.z);

                    float offset = 1.5f;
                    stopX -= direction * offset;
                }
            }
        }

        void Update()
        {
            if (isStopping) return;

            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

            if (direction > 0 && transform.position.x > stopX && !isBack)
                StartCoroutine(StopAndDestroy());

            if (direction < 0 && transform.position.x < stopX && !isBack)
                StartCoroutine(StopAndDestroy());

            // --- 修正箇所：判定の向き（ < と > ）を修正 ---
            if (direction > 0 && transform.position.x > destroyX && isBack)
            {
                Destroy(gameObject);
            }
            if (direction < 0 && transform.position.x < destroyX && isBack)
            {
                Destroy(gameObject);
            }
        }

        IEnumerator StopAndDestroy()
        {
            isStopping = true; 

            yield return new WaitForSeconds(stopTime);

            if (parentDog != null)
                parentDog.ActiveBark(direction);

            Debug.Log("呼び出し");

            isBack = true;
            isStopping = false; // 移動再開（directionはそのままで進み続ける）
        }
    }
}