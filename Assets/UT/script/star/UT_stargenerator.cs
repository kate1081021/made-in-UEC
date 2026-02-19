using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.EventSystems.EventTrigger;

namespace UT
{
    public class UT_stargenerator : MiniGameBase
    {
        public UT_playermove pm;
        public UT_star star;
        public GameObject obj;
        public GameObject obj2;
        [SerializeField]

        [Header("¯Œ`ŠgU’e‚Ìİ’è")]
        [Tooltip("1•Ó‚ ‚½‚è‚Ì’e‚Ì”i’¸“_‚ğœ‚­j")]
        public int d = 6;
        [Tooltip("‚Ç‚ê‚­‚ç‚¢‘å‚«‚­L‚ª‚é‚©B‘å‚«‚­‚µ‚·‚¬‚é‚ÆƒoƒJ‘‚­‚È‚é‚Ì‚Å’ˆÓ")]
        public float radius = 1;
        [Tooltip("¯‚Ì’†S‚ÌxÀ•W")]
        public float centerx = 0;
        [Tooltip("¯‚Ì’†S‚ÌyÀ•W")]
        public float centery = 0;
        [Tooltip("1ŒÂ1ŒÂ‚Ì¯‚Ì‘å‚«‚³")]
        public float scale = 1f;
        [Tooltip("ŠgUAûk‚Ì‘¬‚³B‘å‚«‚­‚·‚é‚ÆI‚í‚é‚Ì‚à‘‚­‚È‚é")]
        public float timeScaleRad = 1;
        [Tooltip("‰ñ“]‚Ì‘¬‚³")]
        public float timeScaleRot = 1;

        [Header("ÅŒã‚Ì’e‚Ìİ’è")]
        [Tooltip("”")]
        public int num = 12;
        [Tooltip("1ŒÂ1ŒÂ‚Ì¯‚Ì‘å‚«‚³")]
        public float size = 1f;
        [Tooltip("1ŒÂ1ŒÂ‚Ì¯‚Ì‘¬‚³")]
        public float speed = 1f;
        float time = 0;
        bool flag = true;
        // Start is called before the first frame update
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 13f;
            GameObject star1 = Instantiate(obj, new Vector3(centerx + Mathf.Sin(0 * Mathf.Deg2Rad), centery + Mathf.Cos(0 * Mathf.Deg2Rad), 0), Quaternion.identity);
            GameObject star2 = Instantiate(obj, new Vector3(centerx + Mathf.Sin(72 * Mathf.Deg2Rad), centery + Mathf.Cos(72 * Mathf.Deg2Rad), 0), Quaternion.identity);
            GameObject star3 = Instantiate(obj, new Vector3(centerx + Mathf.Sin(144 * Mathf.Deg2Rad), centery + Mathf.Cos(144 * Mathf.Deg2Rad), 0), Quaternion.identity);
            GameObject star4 = Instantiate(obj, new Vector3(centerx + Mathf.Sin(216 * Mathf.Deg2Rad), centery + Mathf.Cos(216 * Mathf.Deg2Rad), 0), Quaternion.identity);
            GameObject star5 = Instantiate(obj, new Vector3(centerx + Mathf.Sin(288 * Mathf.Deg2Rad), centery + Mathf.Cos(288 * Mathf.Deg2Rad), 0), Quaternion.identity);
            float x1 = star1.gameObject.transform.position.x;
            float y1 = star1.gameObject.transform.position.y;
            float x2 = star2.gameObject.transform.position.x;
            float y2 = star2.gameObject.transform.position.y;
            float x3 = star3.gameObject.transform.position.x;
            float y3 = star3.gameObject.transform.position.y;
            float x4 = star4.gameObject.transform.position.x;
            float y4 = star4.gameObject.transform.position.y;
            float x5 = star5.gameObject.transform.position.x;
            float y5 = star5.gameObject.transform.position.y;
            for (int i = 1; i < d; i++)
            {
                Instantiate(obj, new Vector2((i * x1 + (d - i) * x3) / d, (i * y1 + (d - i) * y3) / d), Quaternion.identity);
                Instantiate(obj, new Vector2((i * x2 + (d - i) * x4) / d, (i * y2 + (d - i) * y4) / d), Quaternion.identity);
                Instantiate(obj, new Vector2((i * x3 + (d - i) * x5) / d, (i * y3 + (d - i) * y5) / d), Quaternion.identity);
                Instantiate(obj, new Vector2((i * x4 + (d - i) * x1) / d, (i * y4 + (d - i) * y1) / d), Quaternion.identity);
                Instantiate(obj, new Vector2((i * x5 + (d - i) * x2) / d, (i * y5 + (d - i) * y2) / d), Quaternion.identity);
            }
        }


        void FixedUpdate()
        {
            if (time < Mathf.PI)
            {
                time += Time.deltaTime * timeScaleRad * Time.timeScale;
            }
            else 
            {
                if (flag)
                {
                    flag = false;
                float theta = Random.value * 360 / num;
                for (int i = 0; i < num; i++)
                    StartCoroutine(explosion(theta + 360f / num * i));
                }
            }
        }

        IEnumerator explosion(float theta)
        {
            GameObject star2 = Instantiate(obj2, new Vector2(centerx, centery), Quaternion.identity);
            star2.transform.localScale = size * Vector3.one;
            float v = Random.value/2;
            while (true)
            {
                star2.transform.rotation *= Quaternion.Euler(0, 0, 2 * v * Time.timeScale);
                star2.transform.position += speed * (1.5f + v) * new Vector3(Mathf.Sin(theta*Mathf.Deg2Rad), Mathf.Cos(theta * Mathf.Deg2Rad), 0) * Time.timeScale * Time.deltaTime;
                yield return null;
                if (Mathf.Abs(star2.transform.position.x) > 13f || Mathf.Abs(star2.transform.position.y) > 6f)
                {
                    Destroy(star2);
                    break;
                }
            }
        }
    }
}