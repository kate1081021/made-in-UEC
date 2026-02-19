using System.Collections;
using UnityEngine;


namespace UT
{
    public class UT_ghost : MiniGameBase
    {
        public UT_playermove pm;
        public GameObject ghostlight;
        public GameObject pole;
        public GameObject ghost;
        [Tooltip("左下のライトの場所")]
        public Vector3 lightpos1;
        [Tooltip("右のライトの場所")]
        public Vector3 lightpos2;
        [Tooltip("上のライトの場所")]
        public Vector3 lightpos3;
        [Tooltip("左と右のライトの回転速度")]
        public float rotatespeed;
        [Tooltip("上のライトの揺れる速度")]
        public float swingspeed;
        [Tooltip("上のライトの揺れる幅")]
        public float swingwidth;
        [Tooltip("弾の速度")]
        public float ghostspeed;
        [Tooltip("弾の間隔")]
        public float duration;
        [Tooltip("弾の角度の幅")]
        public float ghostvectorrange;
        GameObject light1;
        GameObject light2;
        GameObject light3;
        GameObject pole1;
        GameObject pole2;
        float lightangle = 0;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 15f;
            light1 = Instantiate(ghostlight, lightpos1, Quaternion.Euler(0, 0, 0));
            light2 = Instantiate(ghostlight, lightpos2, Quaternion.Euler(0, 0, -60));
            light3 = Instantiate(ghostlight, lightpos3, Quaternion.Euler(0, 0, 90));
            light3.transform.localScale = Vector3.one;
            pole1 = Instantiate(pole, lightpos1-new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0));
            pole2 = Instantiate(pole, lightpos2-new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0));
            StartCoroutine(generatemanage());
        }

        // Update is called once per frame
        void Update()
        {
            lightangle += Time.deltaTime * Time.timeScale;
            light1.transform.rotation *= Quaternion.Euler(0, 0, Time.deltaTime*Time.timeScale * rotatespeed);
            light2.transform.rotation *= Quaternion.Euler(0, 0, -Time.deltaTime * Time.timeScale * rotatespeed);
            light3.transform.rotation = Quaternion.Euler(0, 0, 90+ swingwidth * Mathf.Sin(lightangle * swingspeed * Time.timeScale));
        }

        IEnumerator generatemanage()
        {
            while (true)
            {
                StartCoroutine(generate());
                yield return new WaitForSeconds(duration / Time.timeScale);
            }
        }
        IEnumerator generate()
        {
            float offset = Random.value * 20 - 10;
            GameObject ghostobj = Instantiate(ghost, new Vector3(offset, 5.5f, 0), Quaternion.identity);
            float theta = Mathf.Atan(6.5f/offset) + (Random.value-0.5f)*ghostvectorrange * Mathf.Deg2Rad;
            if (offset > 0)
            {
                theta += Mathf.PI;
            }
            while (true)
            {
                ghostobj.transform.Translate(ghostspeed * Time.timeScale * Time.deltaTime * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0));
                yield return null;
            }
        }
    }
}
