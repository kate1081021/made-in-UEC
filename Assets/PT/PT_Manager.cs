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

        [SerializeField] private GameObject present_parent;
        [SerializeField] private GameObject present;
        [SerializeField] private GameObject move;
        public override void OnGameStart()
        {
            MGManager.Load();
            present_count = 5;
            present_slope = UnityEngine.Random.Range(-0.5f, 0.5f);
            for (int i = 0; i < present_count; i++)
            {
                GameObject g = Instantiate(present);
                g.transform.position = new UnityEngine.Vector3 (present_slope*i, i, 0);
                presents.Add(g);
            }
        }

        void Update()
        {
            present_slope += Time.timeScale*presents[1].transform.position.x*(float)(present_count/500f);
            present_slope += Time.timeScale*move.transform.position.x/100;
            Debug.Log("+"+(float)(present_count/100f));
            
            for (int i = 0; i < presents.Count; i++)
            {
                GameObject g = presents[i];
                g.transform.position = new Vector3 (present_slope*i, i, 0);
                Debug.Log("max"+present_slope);
            }

            if (present_slope*present_slope >= 1)
            {
                //失敗
            }
        }
    }
}
