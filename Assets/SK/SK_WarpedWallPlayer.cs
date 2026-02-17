using UnityEngine;

namespace SK
{
    // ★ファイル名と一致させる
    public class SK_WarpedWallPlayer : MonoBehaviour
    {
        [Header("Settings")]
        public Transform[] waypoints; // Point0 〜 Point5
        public float baseMoveSpeed = 15f; 
        
        [Header("Components")]
        public Animator animator;
        public Rigidbody2D rb; // ★追加：Rigidbody2Dへの参照

        private SK_WWG gameManager;
        private int nextWaypointIndex = 0;
        private bool isMoving = false;
        // ★ここを修正：引数の型も「SK_WWG」に変更
        public void Initialize(SK_WWG manager)
        {
            gameManager = manager;
            // もしInspectorで設定し忘れていても自動で取得
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (waypoints.Length > 0)
            {
                transform.position = waypoints[0].position;
                nextWaypointIndex = 1;
            }
            isMoving = false;
        }

        void Update()
        {
            if (isMoving && nextWaypointIndex < waypoints.Length && gameManager != null)
            {
                MoveProcess();
            }
        }

        void MoveProcess()
        {
            Transform target = waypoints[nextWaypointIndex];

            // SK_WWGのメソッドを呼ぶ
            float speed = baseMoveSpeed * gameManager.GetSpeedMultiplier();

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            Vector3 direction = target.position - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                isMoving = false;
                if (animator) animator.SetBool("IsRunning", false);
            }
        }

        public void StepForward(bool isFinalJump)
        {
            isMoving = true;
            if (animator) animator.SetBool("IsRunning", true);

            if (isFinalJump)
            {
                if (animator) animator.SetTrigger("Jump"); 
                nextWaypointIndex = waypoints.Length - 1;
                // ★追加：物理挙動を完全に停止させる
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;      // 移動速度を0に
                    rb.angularVelocity = 0f;         // 回転速度を0に
                    rb.isKinematic = true;           // 重力や衝突の影響を受けなくする
                    rb.simulated = false;            // (念の為) 物理シミュレーションから除外
                }
            }
            else
            {
                nextWaypointIndex++;
            }
        }

        public void FallDown()
        {
            isMoving = false;
            if (animator) animator.SetTrigger("Fall");
            // 失敗時は逆に物理挙動をONにして落下させる場合
            if (rb != null)
            {
                rb.isKinematic = false; // 重力を有効化
                // 必要であればここでランダムな回転などを加えても面白いです
            }
        }
    }
}