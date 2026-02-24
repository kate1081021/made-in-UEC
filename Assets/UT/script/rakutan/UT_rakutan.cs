using System.Collections;
using UnityEngine;

namespace UT
{
    public class UT_rakutan : MiniGameBase
    {
        public UT_playermove pm;
        public GameObject tanni;
        public GameObject flask;
        public GameObject droplets;
        public GameObject enemy;
        [SerializeField]
        [Tooltip("単位のx座標")]
        float offsetT;
        [SerializeField]
        [Tooltip("単位の時間間隔")]
        float durationT;
        [SerializeField]
        [Tooltip("単位の速度")]
        float speedT;
        [SerializeField]
        [Tooltip("フラスコの出現時間")]
        float durationA;
        [SerializeField]
        [Tooltip("フラスコのx座標")]
        float offsetF;
        [SerializeField]
        [Tooltip("飛沫の時間間隔")]
        float durationD;
        [SerializeField]
        [Tooltip("飛沫の速度")]
        float speedD;
        [SerializeField]
        [Tooltip("飛沫の落下加速度")]
        float g;
        [SerializeField]
        [Tooltip("飛沫の初速度のランダム幅(百分率)")]
        float randomv;
        [SerializeField]
        [Tooltip("飛沫の初期角度のランダム幅(○○°)")]
        float randomtheta;

        GameObject flaskobj;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 15f;
            Instantiate(enemy, Vector3.zero, Quaternion.identity);
            flaskobj = Instantiate(flask, new Vector3(0, -10f, 0), Quaternion.identity);
            StartCoroutine(generatemanage(1));
            StartCoroutine(Flaskappear());
        }

        // Update is called once per frame
        void Update()
        {

        }
        IEnumerator Flaskappear()
        {
            float time = 0;
            float slider = 0;
            flaskobj.transform.rotation = Quaternion.Euler(0, 0, -15);
            while (slider < 1)
            {
                time += Time.deltaTime;
                slider = time / (durationA * Time.timeScale);
                flaskobj.transform.position = new Vector3(offsetF, -7 + 7 *Mathf.Sin(Mathf.PI/2*slider), 0);
                yield return null;
            }
            StartCoroutine(dropmanage());
        }

        IEnumerator dropmanage()
        {
            while (true)
            {
                GameObject dropobj = Instantiate(droplets, new Vector3(-6.8f, 0.72f, 0), Quaternion.Euler(0, 0, -15));
                float v = speedD * (1 + (Random.value - 0.5f) * randomv);
                StartCoroutine(drop(dropobj, v));
                yield return new WaitForSeconds(durationD / Time.timeScale);
            }
        }

        IEnumerator drop(GameObject obj, float v)
        {
            float time = 0;
            float theta = obj.transform.rotation.eulerAngles.z + (Random.value - 0.5f) * randomtheta;
            float deltaX = -Mathf.Sin(theta * Mathf.Deg2Rad);
            float deltay1 = Mathf.Cos(theta * Mathf.Deg2Rad);
            while (obj != null)
            {
                time += Time.deltaTime * Time.timeScale;
                float deltaY = deltay1 - time * g;
                obj.transform.position += v * Time.timeScale * Time.deltaTime * new Vector3(deltaX, deltaY, 0);
                obj.transform.rotation = Quaternion.Euler(0, 0, 90+Mathf.Atan(deltaY/deltaX)*Mathf.Rad2Deg);
                yield return null;
                if (obj != null && obj.transform.position.y < -7)
                {
                    Destroy(obj);
                }
            }
        }

        IEnumerator generatemanage(int place)
        {
            StartCoroutine(generate(place));
            yield return new WaitForSeconds(durationT / Time.timeScale);
            StartCoroutine(generatemanage(-place));
        }


        IEnumerator generate(int place)
        {
            GameObject obj = Instantiate(tanni, new Vector3(place * offsetT, 7f, 0), Quaternion.identity);
            while (obj != null)
            {
                obj.transform.Translate(speedT * Time.timeScale * Time.deltaTime * new Vector3(0, -1, 0));
                yield return null;
                if(obj != null && obj.transform.position.y < -7)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
