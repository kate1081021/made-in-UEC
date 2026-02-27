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

        private PT_Dog parentDog; 

        public void Init(float dir,PT_Dog dog)
        {
            direction = dir;
            parentDog = dog;

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