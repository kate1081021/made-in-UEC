using PTgame;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace PTgame
{
    public class PT_Wind : MonoBehaviour
    {
        [SerializeField] private PT_Obstacle obstacle;
        [SerializeField] private float max_power;
        [SerializeField] private int direction;
        [SerializeField] private float max_time;
        [SerializeField] private float time;
        [SerializeField] private Camera game_camera;

        [SerializeField] private AudioSource audioSource; // インスペクターでアサイン

        // パーティクル書き換え用の配列キャッシュ
        private ParticleSystem.Particle[] particlesArray;

        void Awake()
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

            if (audioSource != null)
            {
                audioSource.volume = Math.Abs(sinWave); // 0〜1の間で音量が変化
            }

            if (time < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}