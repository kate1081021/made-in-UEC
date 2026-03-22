using UnityEngine;

namespace BOSS
{
    public class BOSS_TestPlayerController : MonoBehaviour
    {
        [Header("プレイヤーの移動速度")]
        public float moveSpeed = 5f;

        void Update()
        {
            // キーボードの入力（WASDキー または 矢印キー）を取得
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            // 移動方向を決定
            Vector3 moveDirection = new Vector3(x, y, 0f);

            // プレイヤーを移動させる（Time.deltaTimeを掛けて滑らかに）
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}