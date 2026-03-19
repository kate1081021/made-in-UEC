using UnityEngine;

namespace garbage
{
    public class GB_GarbageSpawner : MiniGameBase
    {
        [SerializeField] GameObject burnable, can, glassbottle, plasticbottle;
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
            int a = Random.Range(0, 4);
            int b = Random.Range(0, 4);
            int c = Random.Range(0, 4);
            int d = Random.Range(0, 4);

            while (a == b)
            {
                b = Random.Range(0, 4);
            }
            while (c == a || c == b)
            {
                c = Random.Range(0, 4);
            }
            while (d == a || d == b || d == c)
            {
                d = Random.Range(0, 4);
            }
            CreateOne(burnable, a, "burnables");
            CreateOne(can, b, "cans");
            CreateOne(glassbottle, c, "glassbottles");
            CreateOne(plasticbottle, d, "plasticbottles");
        }
        void CreateOne(GameObject prefab, int lane, string type)
        {
            GameObject g = Instantiate(prefab, new Vector2(-6f + 4 * lane, 3.3f), Quaternion.identity);

            var mover = g.GetComponent<GB_GarbageMover>();
            mover.Init(lane, type);
        }
    }
}