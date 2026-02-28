using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UT
{
    public class UT_wifigenerator : MiniGameBase
    {
        public UT_playermove pm;
        public GameObject bulletPrefab;
        public GameObject enemy;

        [Header("Wi-Fi弾幕（塊）の設定")]
        [Tooltip("塊を撃つ間隔（秒）")]
        public float shootInterval = 3.0f;
        [Tooltip("Wi-Fiの弧（線）の数")]
        public int waveCount = 3;
        [Tooltip("1番内側の弧の弾数")]
        public int baseBulletCount = 5;
        [Tooltip("弧の広がる角度（度）")]
        public float arcAngle = 90f;
        [Tooltip("弧と弧の間の隙間の広さ")]
        public float distanceBetweenArcs = 0.5f;
        [Tooltip("飛んでいくスピード")]
        public float bulletSpeed = 5f;

        [Tooltip("発射位置のX座標")]
        public float spawnX = 0f;
        [Tooltip("発射位置のY座標")]
        public float spawnY = 4f;

        public override void OnGameStart()
        {
            pm = GameObject.Find("Player").GetComponent<UT_playermove>();
            pm.generator = gameObject;
            pm.timelimit = 15f;

            if (enemy != null)
            {
                Instantiate(enemy, Vector3.zero, Quaternion.identity);
            }

            StartCoroutine(ShootingLoop());
        }

        IEnumerator ShootingLoop()
        {
            while (true)
            {
                Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

                // プレイヤーの現在位置を取得して、飛んでいく方向（ベクトル）を計算
                Vector3 playerPos = pm.gameObject.transform.position;
                Vector3 dirToPlayer = (playerPos - spawnPos).normalized;

                // 方向を角度（度）に変換
                float aimAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

                // 塊として1回撃つ
                ShootWifiBlock(spawnPos, dirToPlayer, aimAngle);

                // 指定した秒数だけ待つ
                float t = 0;
                while (t < shootInterval)
                {
                    t += Time.deltaTime * Time.timeScale;
                    yield return null;
                }
            }
        }

        void ShootWifiBlock(Vector3 center, Vector3 direction, float aimAngle)
        {
            // 1. 中心の点（Wi-Fiの一番下の丸）
            SpawnBullet(center, direction, bulletSpeed);

            // 2. 外側の弧をまとめて配置
            for (int i = 1; i <= waveCount; i++)
            {
                // 中心からの距離
                float currentRadius = distanceBetweenArcs * i;
                // 弾の数（外側ほど増やす）
                int count = baseBulletCount + (i * 2);

                float startAngle = aimAngle - (arcAngle / 2f);
                float angleStep = arcAngle / (count - 1);

                for (int j = 0; j < count; j++)
                {
                    float a = startAngle + (angleStep * j);

                    // 中心の点から見て、弾をどこに配置するか計算
                    Vector3 offset = new Vector3(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad), 0) * currentRadius;

                    // 弾を生成（位置をズラして配置し、全員同じ direction へ飛ばす）
                    SpawnBullet(center + offset, direction, bulletSpeed);
                }
            }
        }

        void SpawnBullet(Vector3 pos, Vector3 moveDirection, float speed)
        {
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
            bullet.tag = "bullet";

            // 個別の弾を動かすコルーチンを開始
            StartCoroutine(BulletMove(bullet, moveDirection, speed));
        }

        IEnumerator BulletMove(GameObject bullet, Vector3 dir, float speed)
        {
            while (bullet != null)
            {
                bullet.transform.position += dir * speed * Time.deltaTime * Time.timeScale;

                if (Mathf.Abs(bullet.transform.position.x) > 13f || Mathf.Abs(bullet.transform.position.y) > 6f)
                {
                    Destroy(bullet);
                    break;
                }
                yield return null;
            }
        }
    }
}