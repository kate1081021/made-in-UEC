using UnityEngine;

namespace BOSS
{
    public class BOSS_BoomerangObstacle : MonoBehaviour
    {
        [Header("ブーメラン障害物の設定")]
        [SerializeField] public float BOSS_initialDownSpeed = 5.0f; // 最初に降りてくる速さ
        [SerializeField] public float BOSS_upwardAcceleration = 8.0f; // 上に戻るための加速度
        
        [Header("回転の設定")]
        [SerializeField] public float BOSS_rotationSpeed = 360.0f;    // 1秒間に回転する角度（度数法）

        private float BOSS_currentVerticalVelocity; // 現在の垂直速度
        private float BOSS_screenTop;               // 画面の上端（消去判定用）

        void Start()
        {
            // 初速をマイナスにセット
            BOSS_currentVerticalVelocity = -BOSS_initialDownSpeed;

            // 画面の上端を計算（戻っていって画面外に出たら削除）
            // ViewportToWorldPoint(new Vector2(0, 1)) が画面の最上
            BOSS_screenTop = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;
        }

        void Update()
        {
            // 0. 回転の更新：Z軸を中心に回転させる
            transform.Rotate(0, 0, BOSS_rotationSpeed * Time.deltaTime);

            // 1. 速度の更新：上向きの加速度を加え続ける
            BOSS_currentVerticalVelocity += BOSS_upwardAcceleration * Time.deltaTime;

            // 2. 位置の更新：現在の速度をY座標に反映
            Vector3 BOSS_pos = transform.position;
            BOSS_pos.y += BOSS_currentVerticalVelocity * Time.deltaTime;
            transform.position = BOSS_pos;

            // 3. 画面外（上）に戻っていったら自分を消去
            if (BOSS_currentVerticalVelocity > 0 && transform.position.y > BOSS_screenTop + 2.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}