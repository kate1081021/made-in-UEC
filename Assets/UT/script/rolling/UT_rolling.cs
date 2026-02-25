using System.Collections;
using UnityEngine;

namespace UT
{
    public class UT_rolling : MiniGameBase
    {
        public UT_playermove pm;
        public GameObject enemy;
       [SerializeField] GameObject obs1;
       [SerializeField] GameObject obs2;
       [SerializeField] GameObject obs3;
       [SerializeField] int num;
       [SerializeField] float rollingspeed;
        float speed;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 13f;
            Instantiate(enemy, Vector3.zero, Quaternion.identity);
            for (int i = 0; i < num; i++)
            {
                int r = Random.Range(0, 3);
                float a = Mathf.PI / num;
                if (r == 0)StartCoroutine(obsgene1(a+Mathf.PI * 2 / num * i));
                else if(r == 1)StartCoroutine(obsgene2(a+Mathf.PI * 2 / num * i));
                else if(r == 2)StartCoroutine(obsgene3(a+Mathf.PI * 2 / num * i));
            }
            StartCoroutine(speedmanager());
        }

        IEnumerator speedmanager()
        {
            float time = 0;
            float time1 = 4 + Random.value * 1;
            float time2 = time1 + 2 + Random.value * 2;
            float time3 = time2 + 2 + Random.value;
            while(time < 3)
            {
                time += Time.deltaTime * Time.timeScale;
                speed = 0.5f - 0.5f *Mathf.Cos(Mathf.PI / 3 * time);
                yield return null;
            }//1
            while (time < time1)
            {
                time += Time.deltaTime * Time.timeScale;
                yield return null;
            }
            if (Random.Range(0, 2) == 0)
            {
                while (time < time2)
                {
                    time += Time.deltaTime * Time.timeScale;
                    speed = Mathf.Cos(Mathf.PI * (time - time1) / (time2 - time1));
                    yield return null;
                }//-1
                while (time < time2 + 1)
                {
                    time += Time.deltaTime * Time.timeScale;
                    yield return null;
                }

                if (Random.Range(0, 2) == 0)
                {
                    while (time < time3)
                    {
                        time += Time.deltaTime * Time.timeScale;
                        speed = -1.3f + 0.3f*Mathf.Cos(Mathf.PI * (time - time2-1) / (time3 - time2-1));
                        yield return null;
                    }
                }//-1.6
                else
                {
                    while (time < time3)
                    {
                        time += Time.deltaTime * Time.timeScale;
                        speed = 0.1f - 1.1f * Mathf.Cos(Mathf.PI * (time - time2-1) / (time3 - time2-1));
                        yield return null;
                    }
                }//1.2
            }
            else
            {
                while (time < time2)
                {
                    time += Time.deltaTime * Time.timeScale;
                    speed = 1.3f - 0.3f * Mathf.Cos(Mathf.PI  * (time - time1) / (time2 - time1));
                    yield return null;
                }//1.6
                while (time < time2 + 1)
                {
                    time += Time.deltaTime * Time.timeScale;
                    yield return null;
                }

                if (Random.Range(0, 2) == 0)
                {
                    while (time < time3)
                    {
                        time += Time.deltaTime * Time.timeScale;
                        yield return null;
                    }
                }
                else
                {
                    while (time < time3)
                    {
                        time += Time.deltaTime * Time.timeScale;
                        speed = 1.1f + 0.5f * Mathf.Cos(Mathf.PI * (time - time2-1) / (time3 - time2-1));
                        yield return null;
                    }
                }
            }
                yield return null;
        }
        IEnumerator obsgene1(float theta)
        {
            GameObject obs = Instantiate(obs1 , Vector3.zero, Quaternion.identity);
            while (true)
            {
                theta -= Time.deltaTime * rollingspeed * Time.timeScale * speed;
                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);
                obs.GetComponent<UT_0bs1>().sin = sin;
                obs.GetComponent<UT_0bs1>().cos = cos;
                obs.transform.localScale = Vector3.one * (2+cos)/3;
                obs.transform.position = new Vector3(10* sin * (2 + cos) / 3, -cos * (2 + cos) / 3, 0);
                yield return null;
            }
        }

        IEnumerator obsgene2(float theta)
        {
            GameObject obs = Instantiate(obs2 , Vector3.zero, Quaternion.identity);
            while (true)
            {
                theta -= Time.deltaTime * rollingspeed * Time.timeScale * speed;
                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);
                obs.GetComponent<UT_0bs1>().sin = sin;
                obs.GetComponent<UT_0bs1>().cos = cos;
                obs.transform.localScale = Vector3.one * (2+cos)/3;
                obs.transform.position = new Vector3(10* sin * (2 + cos) / 3, -cos * (2 + cos) / 3, 0);
                yield return null;
            }
        }

        IEnumerator obsgene3(float theta)
        {
            GameObject obs = Instantiate(obs3 , Vector3.zero, Quaternion.identity);
            while (true)
            {
                theta -= Time.deltaTime * rollingspeed * Time.timeScale * speed;
                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);
                obs.GetComponent<UT_0bs3>().sin = sin;
                obs.GetComponent<UT_0bs3>().cos = cos;
                obs.transform.localScale = Vector3.one * (2+cos)/3;
                obs.transform.position = new Vector3(10* sin * (2 + cos) / 3, -cos * (2 + cos) / 3, 0);
                yield return null;
            }
        }
    }
}
