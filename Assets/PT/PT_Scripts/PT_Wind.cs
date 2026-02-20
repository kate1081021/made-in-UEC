using System;
using PTgame;
using UnityEngine;

namespace PTgame
{
    public class PT_Wind : MonoBehaviour
    {
        [SerializeField] private PT_Obstacle obstacle;
        [SerializeField] private float max_power;
        [SerializeField] private int direction;
        [SerializeField] private float max_time;
        [SerializeField] private float time;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private Camera game_camera;

        // パーティクル書き換え用の配列キャッシュ
        private ParticleSystem.Particle[] particlesArray;

        void Awake()
        {
            if (game_camera == null) game_camera = Camera.main;

            while (direction == 0)
                direction = UnityEngine.Random.Range(-1, 2);

            max_time = UnityEngine.Random.Range(3f, 5f);
            time = max_time;

            SetupParticleSettings();

            // 配列を初期化（最大数分確保）
            particlesArray = new ParticleSystem.Particle[particle.main.maxParticles];
        }
        void SetupParticleSettings()
        {
            if (particle == null || game_camera == null) return;

            // 1. カメラから画面の高さと幅を計算
            float screenHeight = game_camera.orthographicSize * 2f;
            float screenWidth = screenHeight * game_camera.aspect;

            // 2. Shape（ボックス）の設定を変更
            var shape = particle.shape;
            shape.shapeType = ParticleSystemShapeType.Box; // 形状をボックスに固定

            // ボックスの大きさを画面の高さ・幅に合わせる
            // Xを画面幅、Yを画面高さに設定（パーティクルの飛ぶ方向がZの場合）
            shape.scale = new Vector3(1f, screenHeight, screenWidth);

            // 3. 発生位置（中心）をカメラの中央（0,0,0）に配置
            // これで「画面内のどこからでも」生成されるようになります
            particle.transform.localPosition = Vector3.zero;

            // 4. パーティクルの飛ぶ向きを調整（directionに合わせて回転）
            // Boxの面から放出されるよう、方向を決定
            particle.transform.localRotation = Quaternion.Euler(0, direction > 0 ? 90 : 270f, 0);

            // 5. 密度（Emission）の調整
            var emission = particle.emission;
            // 画面が広くなるほど、面積に合わせて放出量を増やす
            emission.rateOverTime = 10f * (screenWidth * screenHeight) / 10f;
        }

        void Update()
        {
            if (game_camera == null) return;

            time -= Time.deltaTime * Time.timeScale;
            float sinWave = (float)Math.Sin(time * Math.PI / max_time);
            float intensity = Mathf.Max(0, sinWave);

            obstacle.power = direction * max_power * sinWave;

            // 全粒子の見た目を一斉に更新
            SynchronizeAllParticles(intensity);

            if (time < 0)
            {
                Destroy(this.gameObject);
            }
        }

        void SynchronizeAllParticles(float intensity)
        {
            if (particle == null) return;

            // 1. 現在生存しているパーティクルを配列に取得
            int numParticlesAlive = particle.GetParticles(particlesArray);

            // 風の基本速度（適宜調整してください）
            float baseSpeed = 5f;

            // 2. 全ての粒子のパラメータを現在の強さに書き換える
            for (int i = 0; i < numParticlesAlive; i++)
            {
                // サイズを一斉に変更
                // 元のサイズに対して掛ける場合は、あらかじめ初期サイズを保持しておく必要がありますが、
                // ここではシンプルに intensity をベースにします
                particlesArray[i].startSize = intensity * intensity;

                // 速度（進み具合）を一斉に変更
                // 各粒子の進む方向に、現在の強度に応じた速度を上書きします
                // これにより、古い粒子も新しい粒子も同じ速度で動くため「群れ」になりません
                particlesArray[i].velocity = particlesArray[i].velocity.normalized * baseSpeed * intensity;
            }

            // 3. 書き換えた配列をパーティクルシステムに戻す
            particle.SetParticles(particlesArray, numParticlesAlive);
        }
    }
}