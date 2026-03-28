using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.Universal.ShaderGUI;
using Unity.VisualScripting;

namespace garbage
{
    public class GB_GameManagingScript : MiniGameBase
    {
        public Dictionary<string, int> positions = new Dictionary<string, int>(){    //各ゴミ箱の位置のidを管理する，左から順に0, 1, 2, 3
            {"glassbottles", 0},
            {"cans", 1},
            {"plasticbottles", 2},
            {"burnables", 3},
        };
        [SerializeField] GameObject glassbottleBin, canBin, plasticbottleBin, burnableBin;
        [SerializeField] GameObject Arrow;
        [SerializeField] GB_ArrowMover ArrowScript;
        [SerializeField] int arrowpos;
        [SerializeField] GameObject spawnpoint;  //GB_garbageSpawnPoint関係
        GB_GarbageSpawner spawner;    //ここも
        [SerializeField] GameObject background;    //GB_BackgroundManager関連
        GB_BackgroundManager bgmanager;    //ここも
        [SerializeField] GameObject arrow;
        [SerializeField] GameObject textobject;
        public bool judge;
        public int SuccessOrFailure = 0;
        public void MoveByPositionkey(string targetkey)    //keyによってゴミ箱の位置を動かす関数
        {
            GameObject target = glassbottleBin;
            switch (targetkey)
            {
                case "glassbottles":
                    target = glassbottleBin;
                break;
                case "cans":
                    target = canBin;
                break;
                case "plasticbottles":
                    target = plasticbottleBin;
                break;
                case "burnables":
                    target = burnableBin;
                break;
            }
            int id = positions[targetkey];
            target.transform.position = new Vector2(-6f + 4 * id, -3.3f);
            return;
        }
        public override void OnGameStart()
        {
            MGManager.Load();
            //MGManager.TestPlay(31);    //テストプレイ用，必要なければすぐに消す
            BGMPlay(true);
            ArrowScript = Arrow.GetComponent<GB_ArrowMover>();
            arrowpos = ArrowScript.mypos;
            spawner = spawnpoint.GetComponent<GB_GarbageSpawner>();
            bgmanager = background.GetComponent<GB_BackgroundManager>();
            MoveByPositionkey("glassbottles");
            MoveByPositionkey("cans");
            MoveByPositionkey("plasticbottles");
            MoveByPositionkey("burnables");
        }
        public override void OnGameEnd()
        {

        }
        void Update()
        {
            arrowpos = ArrowScript.mypos;
            if (Action.WasPerformedThisFrame() && SuccessOrFailure == 0)    //決定キーが押されたとき，隣り合った2つを入れ替える
            {
                int first = arrowpos, second = arrowpos + 1;
                string firstkey = positions.First(x => x.Value == first).Key, secondkey = positions.First(x => x.Value == second).Key;
                positions[firstkey] = second; positions[secondkey] = first;
                MoveByPositionkey(firstkey); MoveByPositionkey(secondkey);
                SEPlay("exchange");
            }
            if (judge)
            {
                judge = false;
                bool flag = false;
                foreach (var key in positions.Keys)
                {
                    if (spawner.currentTrash[key] == -1)
                    {
                        continue;
                    }
                    if (positions[key] != spawner.currentTrash[key])
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    SuccessOrFailure = 1;
                    MGManager.ClearGame();
                    textobject.gameObject.SetActive(true);
                    SEPlay("twinkle");
                    Destroy(arrow);
                } else
                {
                    SuccessOrFailure = -1;
                    textobject.gameObject.SetActive(true);
                    bgmanager.cloudify();
                    SEPlay("shocked");
                    Destroy(arrow);
                }
            }
        } 
    }
}