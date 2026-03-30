using UnityEngine;

// ★修正：ネームスペースを「BOSS」に変更しました
namespace BOSS 
{
    // ★修正：仕様書の命名規則に従い、クラス名に「BOSS_」をつけました
    public class BOSS_Objects : MonoBehaviour
    {
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float topY = 3.0f;
        [SerializeField] private float bottomY = -3.0f;

        private bool movingUp = true;

        void Update()
        {
            // ★追加：演出監督から「スタート！」の合図が出るまでピタッと待機（フライング防止）
            if (!BOSS_StartSequence.isGamePlaying) return;

            // ローカルの上方向（Y軸）に向かって進む
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (movingUp)
            {
                // 上の限界に達したら
                if (transform.position.y >= topY)
                {
                    movingUp = false;
                    // 180度クルッと回転させる（これでTranslateの「上」が、実際の「下」になります）
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else
            {
                // 下の限界に達したら自分自身を削除
                if (transform.position.y <= bottomY)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}