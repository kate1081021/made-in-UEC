using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
            ArrowScript = Arrow.GetComponent<GB_ArrowMover>();
            arrowpos = ArrowScript.mypos;
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
            if (Action.WasPerformedThisFrame())    //決定キーが押されたとき，隣り合った2つを入れ替える
            {
                int first = arrowpos, second = arrowpos + 1;
                string firstkey = positions.First(x => x.Value == first).Key, secondkey = positions.First(x => x.Value == second).Key;
                positions[firstkey] = second; positions[secondkey] = first;
                MoveByPositionkey(firstkey); MoveByPositionkey(secondkey);
            }
        } 
    }
}