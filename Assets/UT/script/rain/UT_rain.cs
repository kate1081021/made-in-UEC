using System.Collections;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


namespace UT
{
    public class UT_rain : MiniGameBase
    {

        public UT_playermove pm;
        public GameObject umbrella;
        GameObject uobj;
        public GameObject rain;
        public GameObject pigeon;
        GameObject player;
        [SerializeField]
        [Tooltip("雨粒の速度")]
        float rainspeed;
        [SerializeField]
        [Tooltip("鳩1の出現時間")]
        float p1t;
        [SerializeField]
        [Tooltip("鳩1の速度")]
        float p1s;
        [SerializeField]
        [Tooltip("鳩2の出現時間")]
        float p2t;
        [SerializeField]
        [Tooltip("鳩2の速度")]
        float p2s;
        [SerializeField]
        [Tooltip("1回目の風の変わり始め")]
        float w1s;
        [SerializeField]
        [Tooltip("1回目の風の変わり終わり")]
        float w1e;
        [SerializeField]
        [Tooltip("2回目の風の変わり始め")]
        float w2s;
        [SerializeField]
        [Tooltip("2回目の風の変わり終わり")]
        float w2e;
        float theta = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 15f;
            player = GameObject.Find("Player");
            uobj = Instantiate(umbrella, new Vector3(0, -1, 0), Quaternion.identity);
            StartCoroutine(rainmanager());
            StartCoroutine(wind());
        }

        IEnumerator wind()
        {
            float time = 0;
            float theta1 =  (0.5f - Random.value) * 120;
            float theta2 =  (0.5f - Random.value) * 120;
            StartCoroutine(pigeongenerate(p1t, p1s));
            StartCoroutine(pigeongenerate(p2t, p2s));
            while (true)
            {
                time += Time.deltaTime * Time.timeScale;
                if(w1s < time && time < w1e)
                {
                    theta = (time-w1s)/(w1e-w1s) * theta1;
                }
                else if (w2s < time && time < w2e)
                {
                    theta = theta1 + (time - w2s) / (w2e - w2s) * (theta2-theta1);
                }
                yield return null;
            }
        }

        IEnumerator pigeongenerate(float time, float speed)
        {
            yield return new WaitForSeconds(time);
            GameObject pigeonobj = Instantiate(pigeon, new Vector3(10, 5, 0),Quaternion.identity);
            Vector3 v = Vector3.zero;
            while (true)
            {
                Vector3 a = new Vector3(player.transform.position.x - pigeonobj.transform.position.x, player.transform.position.y - pigeonobj.transform.position.y, 0);
                if (a.magnitude > 7) v *= 0.995f * Time.timeScale;
                if (a.x > 0) pigeonobj.transform.rotation = Quaternion.Euler(0, 180, 0);
                else pigeonobj.transform.rotation = Quaternion.identity;
                    v += Time.deltaTime * a * Time.timeScale;
                pigeonobj.transform.position +=Time.deltaTime * speed * v * Time.timeScale;
                yield return null;
            }
        }
        IEnumerator rainmanager()
        {
            while (true)
            {
                StartCoroutine(raingenerate());
                yield return null;
            }
        }
        IEnumerator raingenerate()
        {
            float x = (Random.value - 0.5f) * 60;
            float y = 7;
            GameObject rainobj = Instantiate(rain, new Vector3(x, y, 0), Quaternion.identity);

            while (rainobj != null && rainobj.transform.position.y > -7)
            {
                rainobj.transform.position += (rainspeed * Time.timeScale * Time.deltaTime * new Vector3(Mathf.Sin(theta*Mathf.Deg2Rad), -Mathf.Cos(theta * Mathf.Deg2Rad), 0));
                rainobj.transform.rotation= Quaternion.Euler(0, 0,theta);
                yield return null;
            }
            Destroy(rainobj);
        }

            // Update is called once per frame
            void Update()
        {
            uobj.transform.rotation = Quaternion.Euler(0, 0, theta);
        }
    }
}
