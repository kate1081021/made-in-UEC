using System.Collections;
using UnityEngine;

namespace UT
{
    public class UT_Lissajous : MiniGameBase
    {
        public UT_playermove pm;
        public GameObject enemy;
        [SerializeField] GameObject tri;
        [SerializeField] int num;
        [SerializeField] float RangeX;
        [SerializeField] float RangeY;
        [SerializeField] float accelerateTime;
        [SerializeField] float duration;
        [SerializeField] float speed;
        [SerializeField] float waittime;
        bool generated = false;
        bool two = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 15f;
            Instantiate(enemy, Vector3.zero, Quaternion.identity);
            StartCoroutine(generate());
        }

        IEnumerator generate()
        {
            generated = false;
            for (int i = 0; i < num/5; i++)
            {
                StartCoroutine(move(Instantiate(tri, new Vector3(-RangeX * Mathf.Cos(Mathf.PI * 5 * (5*i) / num), RangeY * Mathf.Cos(Mathf.PI * 6 * (5*i) / num), 0), Quaternion.Euler(0, 0, Random.value * 360))));
                StartCoroutine(move(Instantiate(tri, new Vector3(-RangeX * Mathf.Cos(Mathf.PI * 5 * (5*i+1) / num), RangeY * Mathf.Cos(Mathf.PI * 6 * (5*i+1) / num), 0), Quaternion.Euler(0, 0, Random.value * 360))));
                StartCoroutine(move(Instantiate(tri, new Vector3(-RangeX * Mathf.Cos(Mathf.PI * 5 * (5*i+2) / num), RangeY * Mathf.Cos(Mathf.PI * 6 * (5*i+2) / num), 0), Quaternion.Euler(0, 0, Random.value * 360))));
                StartCoroutine(move(Instantiate(tri, new Vector3(-RangeX * Mathf.Cos(Mathf.PI * 5 * (5*i+3) / num), RangeY * Mathf.Cos(Mathf.PI * 6 * (5*i+3) / num), 0), Quaternion.Euler(0, 0, Random.value * 360))));
                StartCoroutine(move(Instantiate(tri, new Vector3(-RangeX * Mathf.Cos(Mathf.PI * 5 * (5*i+4) / num), RangeY * Mathf.Cos(Mathf.PI * 6 * (5*i+4) / num), 0), Quaternion.Euler(0, 0, Random.value * 360))));
                yield return new WaitForSeconds(duration / Time.timeScale);
            }
            generated = true;
            StartCoroutine(wait());
        }
        IEnumerator move(GameObject obj)
        {
            float time = 0;
            while(!generated) yield return null;
            while (obj != null && Mathf.Abs(obj.transform.position.x) < 9 && Mathf.Abs(obj.transform.position.y) < 5)
            {
                if(time < accelerateTime) time += Time.deltaTime;
                obj.transform.Translate(0, Time.deltaTime * Time.timeScale * time / accelerateTime *speed, 0);
                yield return null;
            }
            Destroy(obj);
        }
        IEnumerator wait()
        {
            yield return new WaitForSeconds(waittime / Time.timeScale);
            if(!two)StartCoroutine(generate());
            two = true;
        }
    }
}
