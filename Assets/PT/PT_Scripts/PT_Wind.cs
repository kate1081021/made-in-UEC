using PTgame;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace PTgame
{
    public class PT_Wind : MiniGameBase
    {
        [SerializeField] private PT_Obstacle obstacle;
        [SerializeField] private float max_power;
        [SerializeField] private int direction;
        [SerializeField] private float max_time;
        [SerializeField] private float time;
        [SerializeField] private Camera game_camera;


        [SerializeField] private bool isse = true;
        [SerializeField] private AudioClip se;


        public override void OnGameStart()
        {
            if (game_camera == null) game_camera = Camera.main;

            while (direction == 0)
                direction = UnityEngine.Random.Range(-1, 2);

            max_time = UnityEngine.Random.Range(3f, 5f);
            time = max_time;
        }

        void Update()
        {
            if (game_camera == null) return;

            time -= Time.deltaTime * Time.timeScale;
            float sinWave = (float)Math.Sin(time * Math.PI / max_time);

            obstacle.power = direction * max_power * sinWave;

            if (isse)
            {
                isse = false;
                SEPlay("wind");
            }

            if (time < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}