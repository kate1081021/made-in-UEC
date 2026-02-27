using UnityEngine;

namespace SK
{
    public class SK_WarpedWallPlayer : MonoBehaviour
    {
        [Header("Settings")]
        public Transform[] waypoints; // Point0 〜 Point5
        public float baseMoveSpeed = 15f; 
        
        [Header("Components")]
        public Animator animator;
        public Rigidbody2D rb; // 物理挙動停止用

        private SK_WWG gameManager;
        private int nextWaypointIndex = 0;
        private bool isMoving = false;

        public void Initialize(SK_WWG manager)
        {
            gameManager = manager;
            
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            // スタート位置へ移動
            if (waypoints.Length > 0)
            {
                transform.position = waypoints[0].position;
                nextWaypointIndex = 1;
            }
            isMoving = false;
        }

        void Start()
        {
            animator = GetComponentInChildren<Animator>();

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

            // 速度 = 初期値 * Time.timeScale
            float speed = baseMoveSpeed * gameManager.GetSpeedMultiplier();

            // 移動
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // 回転（進行方向を向く）
    //        Vector3 direction = target.position - transform.position;
    //        if (direction != Vector3.zero)
    //        {
    //            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //            transform.rotation = Quaternion.Euler(0, 0, angle);
    //        }

            // 到着判定
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
                // ★成功時：最後のジャンプ
                if (animator) animator.SetTrigger("Jump");
                
                nextWaypointIndex = waypoints.Length - 1; // ゴールへ直行

                // 物理挙動を完全に停止させる
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.isKinematic = true;
                    rb.simulated = false;
                }
            }
            else
            {
                nextWaypointIndex++;
            }
        }

        public void PlayPreJumpAction()
        {
            if (animator) animator.SetTrigger("prejump");
        }

        public void FallDown()
        {
            isMoving = false;
            if (animator) animator.SetTrigger("Fall");
            
            // 失敗時に落下させるなら物理有効化
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
}