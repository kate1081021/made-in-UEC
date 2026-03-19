using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace garbage
{
    public class GB_GarbageSpawner : MiniGameBase
    {
        [SerializeField] GameObject burnable, can, glassbottle, plasticbottle;
        public Dictionary<string, int> currentTrash = new Dictionary<string, int>()
        {
            {"burnables", 0},
            {"cans", 0},
            {"glassbottles", 0},
            {"plasticbottles", 0},
        };
        int num = -10;

        public override void OnGameStart()
        {
            
        }

        void Update()
        {
            num++;

            if (num % 3000 == 0)
            {
                CreateGarbage();
            }
        }

        void CreateGarbage()
        {
            List<int> values = new List<int>() { 0, 1, 2, 3 };
            for (int i = 0; i < values.Count; i++)
            {
                int rand = Random.Range(0, values.Count);
                int temp = values[i];
                values[i] = values[rand];
                values[rand] = temp;
        }
        int index = 0;
        foreach (var key in currentTrash.Keys.ToList())
        {
            currentTrash[key] = values[index];
            index++;
        }
        Instantiate(burnable, new Vector2(-6f + 4 * currentTrash["burnables"], 3.3f), Quaternion.identity);
        Instantiate(can, new Vector2(-6f + 4 * currentTrash["cans"], 3.3f), Quaternion.identity);
        Instantiate(glassbottle, new Vector2(-6f + 4 * currentTrash["glassbottles"], 3.3f), Quaternion.identity);
        Instantiate(plasticbottle, new Vector2(-6f + 4 * currentTrash["plasticbottles"], 3.3f), Quaternion.identity);
        }
    }
}