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
        [SerializeField] private GameObject present_parent;
        [SerializeField] private GameObject present;
        [SerializeField] private PT_move move;
        public override void OnGameStart()
        {
            MGManager.Load();
            present_count = 5;
            present_parent_move = 0;
            present_slope = Random.Range(-0.5f, 0.5f);
            for (int i = 0; i < present_count; i++)
            {
                GameObject g = Instantiate(present, present_parent.transform);
                g.transform.localPosition = new Vector3 (present_slope*i, i, 0);
                presents.Add(g);
            }
        }

        void Update()
        {
            present_slope = Time.timeScale * (presents[1].transform.localPosition.x + presents[1].transform.localPosition.x*(float)present_count/500f);
            //present_slope += Time.timeScale*move.transform.position.x/500f;
            float movex = move.HandleMove().x;
            
            if (movex < 0)
            {
                //present_parent_move 
                present_parent.transform.position = new Vector3(-1, 0, 0);
            }
            else if (movex > 0)
            {
                //present_parent_move 
                present_parent.transform.position = new Vector3(1, 0, 0);
            }
            else
            {
                //present_parent_move 
                present_parent.transform.position = new Vector3(0, 0, 0);
            }
            
            for (int i = 0; i < presents.Count; i++)
            {
                GameObject g = presents[i];
                g.transform.localPosition = new Vector3 (Time.timeScale*(present_slope + movex*1f)*i, i, 0);
            }
            Debug.Log("local" + presents[1].transform.localPosition);
            Debug.Log("present_slope"+ present_slope +"delta" + (movex*0.01f));

            if (present_slope*present_slope >= 1)
            {
                //失敗
            }
            
        }
    }
}
