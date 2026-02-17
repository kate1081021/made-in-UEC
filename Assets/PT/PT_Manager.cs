using System.Collections.Generic;
using System.Numerics;
using System;
using TMPro;
using UnityEngine;

namespace PTgame
{
    public class PT_Manager : MiniGameBase
    {        
        public List<GameObject> presents = new List<GameObject>(); //プレゼント
        public int present_count;
        public float present_slope;
        public GameObject present;
        public override void OnGameStart()
        {
            MGManager.Load();
            present_count = 5;
            present_slope = UnityEngine.Random.Range(-1f, 1f);
            for (int i = 0; i < present_count; i++)
            {
                GameObject g = Instantiate(present);
                g.transform.position = new UnityEngine.Vector3 (present_slope*i, i, 0);
                presents.Add(g);
            }
        }

        void Update()
        {
            present_slope += Time.deltaTime*present_slope/10;
            
            for (int i = 0; i < presents.Count; i++)
            {
                GameObject g = presents[i];
                g.transform.position = new UnityEngine.Vector3 (present_slope*i, i, 0);
            }

            if (present_slope*present_slope >= 1)
            {
                //失敗
            }
        }
    }
}
