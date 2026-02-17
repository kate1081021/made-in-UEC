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
        [SerializeField] private GameObject present_parent;
        [SerializeField] private GameObject present;
        [SerializeField] private PT_move move;
        [SerializeField] private Camera camera;
        public override void OnGameStart()
        {
            MGManager.Load();
            //present_count = 5;
            camera.orthographicSize = 2f + (float)present_count/1.25f;
            camera.transform.position = new Vector3(0, -2.5f+(float)present_count/2, -10);
            present_parent_move = 0;
            present_slope = UnityEngine.Random.Range(-0.5f, 0.5f);
            for (int i = 0; i < present_count; i++)
            {
                GameObject g = Instantiate(present, present_parent.transform);
                g.transform.localPosition = new Vector3 (present_slope*i, i, 0);
                presents.Add(g);
            }
        }

        public override void OnGameEnd()
        {
            if (present_slope*present_slope < 1)
            {
                MGManager.ClearGame(); //クリア
            }
        }

        void Update()
        {
            present_slope = Time.timeScale * (presents[1].transform.localPosition.x + presents[1].transform.localPosition.x*(float)present_count/500f);
            //present_slope += Time.timeScale*move.transform.position.x/500f;
            float movex = move.HandleMove().x;
            
            if (movex < 0)
            {
                if (ppm_x <= 0)
                {
                    ppm_x -= Time.timeScale/10;
                }
                else if (ppm_x > 0)
                {
                    ppm_x = ppm_x/10-Time.timeScale/10;
                }
                present_parent_move = (float)Math.Atan(ppm_x);
                present_parent.transform.position = new Vector3(present_parent_move, 0, 0);
            }
            else if (movex > 0)
            {
                if (ppm_x >= 0)
                {
                    ppm_x += Time.timeScale/10;
                }
                else if (ppm_x < 0)
                {
                    ppm_x = ppm_x/10+Time.timeScale/10;
                }
                present_parent_move = (float)Math.Atan(ppm_x);
                present_parent.transform.position = new Vector3(present_parent_move, 0, 0);
            }
            else
            {
                if (ppm_x > 0.01f)
                {
                    ppm_x = ppm_x/1.2f;
                }
                else if (ppm_x < 0.01f)
                {
                    ppm_x = ppm_x/1.2f;
                }
                else
                {
                    ppm_x = 0;
                }
                present_parent_move = (float)Math.Atan(ppm_x);
                present_parent.transform.position = new Vector3(present_parent_move, 0, 0);
            }
            
            for (int i = 0; i < presents.Count; i++)
            {
                GameObject g = presents[i];
                g.transform.localPosition = new Vector3 (Time.timeScale*(present_slope + movex*present_count*0.1f)*i, i, 0);
            }
            //Debug.Log("local" + presents[1].transform.localPosition);
            //Debug.Log("present_slope"+ present_slope +"delta" + (movex*0.01f));

            if (present_slope*present_slope >= 1)
            {
                Debug.Log("Lose...");
                //OnGameEnd();
            }
        }
    }
}
