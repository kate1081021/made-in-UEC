using UnityEngine;

namespace MyMiniGame
{
    public class BOSS_damage : MiniGameBase
    {
        [SerializeField] private GameObject[] hearts;
        private int life;

        public override void OnGameStart()
        {
            MGManager.Load();
            life = hearts.Length;

            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null) hearts[i].SetActive(true);
            }
        }

        public override void OnGameEnd() { }
        public void hitdamage()
        {
            if (life > 0)
            {
                life--;
                hearts[life].SetActive(false);
            }
        }


    }
}