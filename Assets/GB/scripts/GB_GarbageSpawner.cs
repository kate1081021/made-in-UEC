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

            Instantiate(burnable, new Vector2(-6f + 4 * a, 3.3f), Quaternion.identity);
            Instantiate(can, new Vector2(-6f + 4 * b, 3.3f), Quaternion.identity);
            Instantiate(glassbottle, new Vector2(-6f + 4 * c, 3.3f), Quaternion.identity);
            Instantiate(plasticbottle, new Vector2(-6f + 4 * d, 3.3f), Quaternion.identity);
        }
    }
}