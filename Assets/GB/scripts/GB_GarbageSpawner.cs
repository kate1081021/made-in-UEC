using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NUnit.Framework;

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
        //int num = -10;
        float initialtime;
        float currenttime;
        bool isCreated = false;

        public override void OnGameStart()
        {
            initialtime = Time.time;
        }

        void Update()
        {
            currenttime = Time.time;
            if (!isCreated && currenttime - initialtime > 0.3f)
            {
                CreateGarbage();
                isCreated = true;
            }
            /*
            num++;

            if (num % 3000 == 0)
            {
                CreateGarbage();
            }
            */
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
            if (MGManager.stage <= 15)
            {
                int index1 = Random.Range(0, values.Count);
                int index2;

                do
                {
                    index2 = Random.Range(0, values.Count);
                } while (index2 == index1);

                values[index1] = -1;
                values[index2] = -1;

            }
            /*else if (MGManager.stage <= 30)
            {
                int index1 = Random.Range(0, values.Count);
                values[index1] = -1;
            }*/
            int index = 0;
            foreach (var key in currentTrash.Keys.ToList())
            {
                currentTrash[key] = values[index];
                index++;
            }
            if (currentTrash["burnables"] != -1)
            {
                Instantiate(burnable, new Vector2(-6f + 4 * currentTrash["burnables"], 3.3f), Quaternion.identity);
            }
            if (currentTrash["cans"] != -1)
            {
                Instantiate(can, new Vector2(-6f + 4 * currentTrash["cans"], 3.3f), Quaternion.identity);
            }
            if (currentTrash["glassbottles"] != -1)
            {
                Instantiate(glassbottle, new Vector2(-6f + 4 * currentTrash["glassbottles"], 3.3f), Quaternion.identity);
            }
            if (currentTrash["plasticbottles"] != -1)
            {
                Instantiate(plasticbottle, new Vector2(-6f + 4 * currentTrash["plasticbottles"], 3.3f), Quaternion.identity);
            }
            /*
            Instantiate(burnable, new Vector2(-6f + 4 * currentTrash["burnables"], 3.3f), Quaternion.identity);
            Instantiate(can, new Vector2(-6f + 4 * currentTrash["cans"], 3.3f), Quaternion.identity);
            Instantiate(glassbottle, new Vector2(-6f + 4 * currentTrash["glassbottles"], 3.3f), Quaternion.identity);
            Instantiate(plasticbottle, new Vector2(-6f + 4 * currentTrash["plasticbottles"], 3.3f), Quaternion.identity);
            */
        }
    }
}