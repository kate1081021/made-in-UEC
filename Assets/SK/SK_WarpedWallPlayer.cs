using UnityEngine;

namespace SK
{
    // ★ファイル名と一致させる
    public class SK_WarpedWallPlayer : MonoBehaviour
    {
        public Transform[] waypoints;
        public float baseMoveSpeed = 15f; 
        public Animator animator;

        // ★ここを修正：型を「SK_WWG」に変更
        private SK_WWG gameManager; 
        private int nextWaypointIndex = 0;
        private bool isMoving = false;

        // ★ここを修正：引数の型も「SK_WWG」に変更
        public void Initialize(SK_WWG manager)
        {
            gameManager = manager;
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
        }
    }
}