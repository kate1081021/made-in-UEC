using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace PTgame
{
    public class PT_Manager : MiniGameBase
    {
        [SerializeField] private List<GameObject> presents = new List<GameObject>(); //プレゼント
        [SerializeField] private int present_count;
        [SerializeField] private float present_slope;
        [SerializeField] private float present_parent_move;
        [SerializeField] private float ppm_x;
        [SerializeField] public bool fall;
        [SerializeField] private GameObject present_parent;
        [SerializeField] private GameObject present;
        [SerializeField] private PT_move move;
        [SerializeField] private Camera camera;
        public override void OnGameStart()
        {
            MGManager.Load();
            //present_count = 5;
            camera.orthographicSize = 2f + (float)present_count / 1.25f;
            camera.transform.position = new Vector3(0, -2.5f + (float)present_count / 2, -10);
            present_parent_move = 0;
            present_slope = UnityEngine.Random.Range(-0.25f, 0.25f);
            fall = false;
            for (int i = 0; i < present_count; i++)
            {
                GameObject g = Instantiate(present, present_parent.transform);
                g.transform.localPosition = new Vector3(present_slope * i, i, 0);
                // 1. 生成したオブジェクトにある PT_FallPresent スクリプトを探す
                PT_FallPresent fallScript = g.GetComponent<PT_FallPresent>();

                // 2. そのスクリプトの manager 変数に、この Manager 自身 (this) を入れる
                if (fallScript != null)
                {
                    fallScript.manager = this;
                }
                presents.Add(g);
            }
        }

        public override void OnGameEnd()
        {
            if (present_slope * present_slope < 1)
            {
                MGManager.ClearGame(); //クリア
            }
        }

        void Update()
        {
            //Debug.Log("local" + presents[1].transform.localPosition);
            //Debug.Log("present_slope"+ present_slope +"delta" + (movex*0.01f));
            if (present_slope * present_slope < 1)
            {
                present_slope = Time.timeScale * (presents[1].transform.localPosition.x + presents[1].transform.localPosition.x * (float)present_count / 500f);
                //present_slope += Time.timeScale*move.transform.position.x/500f;
                float movex = move.HandleMove().x;

                if (movex < 0)
                {
                    if (ppm_x <= 0)
                    {
                        ppm_x -= Time.timeScale / 10;
                    }
                    else if (ppm_x > 0)
                    {
                        ppm_x = ppm_x / 10 - Time.timeScale / 10;
                    }
                }
                else if (movex > 0)
                {
                    if (ppm_x >= 0)
                    {
                        ppm_x += Time.timeScale / 10;
                    }
                    else if (ppm_x < 0)
                    {
                        ppm_x = ppm_x / 10 + Time.timeScale / 10;
                    }
                }
                else
                {
                    if (ppm_x * ppm_x > 0.0001f)
                    {
                        ppm_x = ppm_x / 1.2f;
                    }
                    else
                    {
                        ppm_x = 0;
                    }
                }
                present_parent.transform.position = new Vector3((float)Math.Atan(ppm_x) / 10, (float)Math.Atan(ppm_x) * (float)Math.Atan(ppm_x) / 10, 0);
                present_parent.transform.localEulerAngles = new Vector3(0, 0, (float)Math.Atan(ppm_x) * 10f);

                for (int i = 0; i < presents.Count; i++)
                {
                    GameObject g = presents[i];
                    float power = (float)Math.Atan((float)present_count/60);
                    if (power > 0.1f)
                        power = 0.1f;                
                    //Debug.Log("power"+power);
                    g.transform.localPosition = new Vector3(Time.timeScale * (present_slope - movex * 4 * power) * i, i, 0);
                }
            }
            else
            {
                Debug.Log("Lose...");
                fall = true;

                if (ppm_x * ppm_x > 0.0001f)
                {
                    ppm_x = ppm_x / 1.05f;
                }
                else
                {
                    ppm_x = 0;
                }
                present_parent.transform.position = new Vector3((float)Math.Atan(ppm_x) / 10, (float)Math.Atan(ppm_x) * (float)Math.Atan(ppm_x) / 10, 0);
                present_parent.transform.localEulerAngles = new Vector3(0, 0, (float)Math.Atan(ppm_x) * 10f);
            }
        }
    }
}
