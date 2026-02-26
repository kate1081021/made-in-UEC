using UnityEngine;

namespace UT
{
    public class UT_umbrella : MiniGameBase
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {

        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("bullet") && collision.name == "UT_rain(Clone)")
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
