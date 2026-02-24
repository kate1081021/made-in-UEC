using Unity.VisualScripting;
using UnityEngine;

namespace UT
{
    public class UT_0bs3 : MiniGameBase
    {
        [SerializeField] GameObject mov;
        [SerializeField] float speed;
        public float sin;
        public float cos;
        float time;
        int childCount;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            childCount = transform.childCount;
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

            time += Time.deltaTime * Time.timeScale * speed;
            if (time > 2f) time = 0;
            float scale = Mathf.Abs(time - 1f);
            if (mov != null) mov.transform.localScale = new Vector3(1, 0.8f+0.8f*scale, 0);
        }
    }
}
