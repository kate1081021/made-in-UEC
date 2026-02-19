using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace UT
{
    public class UT_star : MiniGameBase
    {
        UT_stargenerator gene;
        float radius;
        float centerx;
        float centery;
        float scale;
        float timeScaleRad;
        float timeScaleRot;
        float dis;
        float theta;
        float timer = 0;
        public override void OnGameStart()
        {
            gene = GameObject.Find("stargenerator").GetComponent<UT_stargenerator>();
            radius = gene.radius;
            centerx = gene.centerx;
            centery = gene.centery;
            scale = gene.scale;
            timeScaleRad = gene.timeScaleRad;
            timeScaleRot = gene.timeScaleRot;
            transform.localScale = new Vector3(scale, scale, scale);
            dis = Mathf.Sqrt(Mathf.Pow(centerx-transform.position.x, 2) + Mathf.Pow(centery-transform.position.y, 2));
            theta = (transform.position.x == centerx) ? Mathf.PI / 2 : Mathf.Atan((centery -transform.position.y) / (centerx - transform.position.x));
            if (transform.position.x < centerx) theta += Mathf.PI;
        }
        void FixedUpdate()
        {
            if (timer < Mathf.PI)
            {
                timer += Time.deltaTime * timeScaleRad * Time.timeScale;
                theta += Time.deltaTime * 0.1f * timeScaleRot * (1+2*Mathf.Sin(timer)) * Time.timeScale;
                transform.rotation = Quaternion.Euler(0, 0, 200*timer);
                float r = dis * radius * 10 * Mathf.Sin(timer);
                transform.position = new Vector2(centerx +r * Mathf.Cos(theta),centery + r * Mathf.Sin(theta));
                while (transform.position.x > 9) transform.position += new Vector3(-18f, 0, 0);
                while (transform.position.x < -9) transform.position += new Vector3(18f, 0, 0);
                while (transform.position.y > 5.1f) transform.position += new Vector3(0, -10.2f, 0);
                while (transform.position.y < -5.1f) transform.position += new Vector3(0, 10.2f, 0);
            }else Destroy(gameObject);
        }
    }
}
