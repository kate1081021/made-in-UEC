using PTgame;
using UnityEngine;

namespace PTgame
{
    public class PT_FallPresent : MonoBehaviour
    {
        [SerializeField] public PT_Manager manager;
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private float pushForce = 2.0f; // 左右に飛ばす強さ
        private bool isFalling = false; // すでに落下開始したかどうかのフラグ
        // Update is called once per frame
        void Update()
        {
            if (manager.fall && !isFalling)
            {
                rigid.simulated = true;
                isFalling = true;
                // 左右に力を加える
                ApplyHorizontalForce();
            }
        }

        private void ApplyHorizontalForce()
        {
            // 自分のx座標がプラス（右側）なら右へ、マイナス（左側）なら左へ力を加える
            // 0に近いほど力は弱くなり、端にいるほど強く飛ぶようになります
            float direction = transform.localPosition.x;

            // AddForceで瞬間的な力(Impulse)を加える
            // 第1引数：方向と強さ (x, y)
            rigid.AddForce(new Vector2(direction * pushForce, pushForce), ForceMode2D.Impulse);

            // ついでに少し回転させるとよりリアルになります
            rigid.AddTorque(direction * -0.1f, ForceMode2D.Impulse);
        }
    }
}