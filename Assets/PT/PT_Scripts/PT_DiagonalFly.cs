using UnityEngine;

namespace PTgame
{
    public class PT_DiagonalFly : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;
        [SerializeField] private float scaleStart = 0.5f;   // 飛び出すときの小さいスケール
        [SerializeField] private float scaleEnd = 1f;       // 到着時の標準スケール

        private Vector2 moveDir;
        private float totalDistance;
        private Vector3 startPos;
        private Vector3 targetPos;

        public void Init(float dir)
        {
            // dir = 1 なら右へ、-1 なら左へ
            // 上から斜めに飛んでくる
            moveDir = new Vector2(dir, -1f).normalized;

            startPos = transform.position;

            // だいたい画面中心まで届くように targetPos 設定
            float screenCenterX = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;
            targetPos = new Vector3(screenCenterX, Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y, 0f);

            totalDistance = Vector3.Distance(startPos, targetPos);

            // 初期スケールを小さく
            transform.localScale = Vector3.one * scaleStart;
        }

        void Update()
        {
            // 移動
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);

            // スケール補間
            float traveled = Vector3.Distance(startPos, transform.position);
            float t = Mathf.Clamp01(traveled / totalDistance);
            float scale = Mathf.Lerp(scaleStart, scaleEnd, t);
            transform.localScale = Vector3.one * scale;

            // 画面外に出たら削除
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x < -0.2f || viewPos.x > 1.2f || viewPos.y < -0.2f)
            {
                Destroy(gameObject);
            }
        }
    }
}