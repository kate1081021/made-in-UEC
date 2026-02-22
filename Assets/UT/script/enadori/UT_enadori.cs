using System.Collections;
using UnityEngine;


namespace UT
{
    public class UT_enadori : MiniGameBase
    {
        UT_playermove pm;
        GameObject player;
        [SerializeField]
        GameObject bottle;
        [SerializeField]
        GameObject drink1;
        [SerializeField]
        GameObject drink2;
        [SerializeField]
        GameObject drink3;
        [SerializeField]
        GameObject drink4;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì—‰º‰Á‘¬“x")]
        float g;
        [SerializeField]
        [Tooltip("ŠÊ‚ÌŠJnˆÊ’u")]
        Vector3 pos;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì“Š‚°‚éŠÔŠu")]
        float duration;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì”j—ôêŠ¶’[")]
        float Rx1;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì”j—ôêŠ‰E’[")]
        float Rx2;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì”j—ôêŠ‰º’[")]
        float Ry1;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì”j—ôêŠã’[")]
        float Ry2;
        [SerializeField]
        [Tooltip("ŠÊ‚Ì”j—ôŠÔ")]
        float expTime;
        [Header("Ô‚¢’e‚Ìİ’è")]
        [SerializeField]
        [Tooltip("Ô‚¢’e‚ÌÅ’x‘¬“x")]
        float RS1;
        [SerializeField]
        [Tooltip("Ô‚¢’e‚ÌÅ‘¬‘¬“x")]
        float RS2;

        [Header("Â‚¢’e‚Ìİ’è")]
        [SerializeField]
        [Tooltip("Â‚¢’e‚Ì”")]
        float numOfBlue;
        [SerializeField]
        [Tooltip("Â‚¢’e‚ÌŠgU‘¬“x")]
        float blueSpreadSpeed;
        [SerializeField]
        [Tooltip("Â‚¢’e‚Ì‰ñ“]‘¬“x")]
        float blueRotateSpeed;

        [Header("—Î’e‚Ìİ’è")]
        [SerializeField]
        [Tooltip("—Î’e‚Ì”")]
        float numOfGreen;
        [SerializeField]
        [Tooltip("—Î’e‚ÌŠgU‘¬“x")]
        float greenSpreadSpeed;
        [SerializeField]
        [Tooltip("—Î’e‚Ì’†SˆÚ“®‘¬“x")]
        float greenMoveSpeed;
        [SerializeField]
        [Tooltip("—Î’e‚Ì‰ñ“]‘¬“x")]
        float greenRotateSpeed;
        [SerializeField]
        [Tooltip("—Î’e‚Ì”¼ŒaãŒÀ")]
        float greenRadiusMax;

        [Header("‰©’e‚Ìİ’è")]
        [SerializeField]
        [Tooltip("‰©’e‚Ì”ò‚ÑU‚éŠp“x")]
        float yellowRandomTheta;
        [SerializeField]
        [Tooltip("‰©’e‚Ì”")]
        float numOfYellow;
        [SerializeField]
        [Tooltip("‰©’e‚ÌÅ‘¬‚Ì‘¬‚³")]
        float yellowSpeedMax;
        [SerializeField]
        [Tooltip("‰©’e‚ÌÅ’á‚Ì‘¬‚³")]
        float yellowSpeedMin;
        [SerializeField]
        [Tooltip("‰©’e‚É‚©‚©‚éd—Í")]
        float yellowG;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 15f;
            player = GameObject.Find("Player");
            StartCoroutine(throwmanage());
        }


        IEnumerator throwmanage()
        {
            while (true)
            {
                StartCoroutine(throwbottle());
                yield return new WaitForSeconds(duration / Time.timeScale);
            }
        }
        IEnumerator throwbottle()
        {
            GameObject obj = Instantiate(bottle, pos, Quaternion.identity);
            int kind = Random.Range(0, 4);
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            switch (kind)
            {
                case 0:sr.color = Color.red; break;
                case 1:sr.color = Color.blue; break;
                case 2:sr.color = Color.green; break;
                case 3:sr.color = Color.yellow; break;
            }
            float time = 0;
            float x = Rx1 + Random.value * (Rx2 - Rx1);
            float y = Ry1 + Random.value * (Ry2 - Ry1);
            float rotate = (0.5f - Random.value) * 360;
            while (obj != null)
            {
                time += Time.deltaTime * Time.timeScale;
                float slider = time / expTime;
                Vector3 moveVec = new Vector3(x - pos.x,y - pos.y + g * expTime,0);
                obj.transform.position = pos + moveVec * slider + new Vector3(0, -g*time * slider * slider, 0);
                obj.transform.rotation = Quaternion.Euler(0, 0, slider * rotate);
                //obj.transform.rotation = Quaternion.Euler(0, 0, 90 + Mathf.Atan(deltaY / deltaX) * Mathf.Rad2Deg);
                yield return null;
                if (obj != null && slider >= 1)
                {
                    explosion(kind, obj.transform.position);
                    Destroy(obj);
                }
            }
        }

        void explosion(int kind, Vector3 pos)
        {
            if (kind == 0)
            {
                StartCoroutine(red(RS1, pos));
                StartCoroutine(red((RS1 + RS2) / 2, pos));
                StartCoroutine(red(RS2, pos));
            }
            else if (kind == 1)
            {
                for (int i = 0; i < numOfBlue; i++)
                {
                    StartCoroutine(blue(Mathf.PI * 2 / numOfBlue * i, 1, pos));
                    StartCoroutine(blue(Mathf.PI * 2 / numOfBlue * i, -1, pos));
                }
            }
            else if (kind == 2)
            {
                for (int i = 0; i < numOfGreen; i++)
                {
                    StartCoroutine(green(Mathf.PI * 2 / numOfGreen * i, pos));
                }
            }
            else if (kind == 3)
            {
                for (int i = 0; i < numOfYellow; i++)
                {
                    StartCoroutine(yellow(pos));
                }
            }
        }
        IEnumerator red(float speed, Vector3 pos)
        {
            GameObject obj = Instantiate(drink1, pos, Quaternion.identity);
            float theta = Mathf.Atan((player.transform.position.y - obj.transform.position.y)/(player.transform.position.x - obj.transform.position.x));
            if (theta > 0) theta += Mathf.PI;
            while(obj != null && obj.transform.position.y > -7)
            {
                obj.transform.position += (speed * Time.timeScale * Time.deltaTime * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0));
                yield return null;
            }
        }
        IEnumerator blue(float Ftheta, int dir, Vector3 pos)
        {
            GameObject obj = Instantiate(drink2, pos, Quaternion.identity);
            float radius = 0;
            float theta = 0;
            float timer = 0;
            while (true)
            {
                timer += Time.deltaTime * Time.timeScale;
                radius = blueSpreadSpeed * timer;
                theta = blueRotateSpeed * timer;
                obj.transform.position = pos + new Vector3(radius * Mathf.Cos(dir * theta +Ftheta), radius * Mathf.Sin(dir * theta + Ftheta), 0);
                yield return null;
            }
        }
        IEnumerator green(float Ftheta, Vector3 pos)
        {
            GameObject obj = Instantiate(drink3, pos, Quaternion.identity);
            float radius = 0;
            float theta = 0;
            float timer = 0;
            float theta2 = Mathf.Atan((player.transform.position.y - obj.transform.position.y) / (player.transform.position.x - obj.transform.position.x));
            if (theta2 > 0) theta2 += Mathf.PI;
            while (true)
            {
                timer += Time.deltaTime * Time.timeScale;
                radius = greenSpreadSpeed * timer;
                if(radius > greenRadiusMax)radius = greenRadiusMax;
                theta = greenRotateSpeed * timer;
                obj.transform.position = pos + new Vector3(radius * Mathf.Cos(theta + Ftheta), radius * Mathf.Sin(theta + Ftheta), 0) + (greenMoveSpeed * timer * new Vector3(Mathf.Cos(theta2), Mathf.Sin(theta2), 0)); ;
                yield return null;
            }
        }
        IEnumerator yellow(Vector3 pos)
        {
            GameObject obj = Instantiate(drink4, pos, Quaternion.identity);
            float time = 0;
            float theta = (Random.value - 0.5f) * yellowRandomTheta;
            float deltaX = -Mathf.Sin(theta * Mathf.Deg2Rad);
            float deltay1 = Mathf.Cos(theta * Mathf.Deg2Rad);
            float yellowSpeed = yellowSpeedMin + (1 - Random.value) * (yellowSpeedMax - yellowSpeedMin);
            while (obj != null)
            {
                time += Time.deltaTime * Time.timeScale;
                float deltaY = deltay1 - time * g;
                obj.transform.position += yellowSpeed * Time.timeScale * Time.deltaTime * new Vector3(deltaX, deltay1, 0) - Time.timeScale * Time.deltaTime * new Vector3(0, time * yellowG, 0);
               // obj.transform.rotation = Quaternion.Euler(0, 0, 90 + Mathf.Atan(deltaY / deltaX) * Mathf.Rad2Deg);
                yield return null;
                if (obj != null && obj.transform.position.y < -7)
                {
                    Destroy(obj);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
