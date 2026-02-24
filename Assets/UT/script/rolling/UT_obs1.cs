using Unity.VisualScripting;
using UnityEngine;

namespace UT
{
    public class UT_0bs1 : MiniGameBase
    {
        [SerializeField] GameObject mov;
        [SerializeField] float speed;
        public float sin;
        public float cos;
        float time;
        int childCount;
        float r;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            childCount = transform.childCount;
            r = Random.value * 0.5f + 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (cos < 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                    sr.color = new Vector4(0.5f, 0.5f, 0.5f, 1 + cos * 0.7f);
                    sr.sortingOrder = -2;
                    child.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else
            {
                if (transform.GetChild(0).GetComponent<BoxCollider2D>().enabled == false)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        Transform child = transform.GetChild(i);
                        SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                        sr.color = Color.white; ;
                        sr.sortingOrder = 1;
                        child.GetComponent<BoxCollider2D>().enabled = true;
                    }
                }
            }

                time += Time.deltaTime * Time.timeScale * speed * r;
            if(time > 3f) time = 0;
            float pos = Mathf.Abs(time-1.5f) - 0.75f;
            if(mov != null)mov.transform.localPosition = new Vector3(0,  pos, 0);
        }
    }
}
