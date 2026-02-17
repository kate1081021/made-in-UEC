using UnityEngine;
using UnityEngine.InputSystem;

namespace PTgame
{
    public class PT_move : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;

        // InputAction（Inspectorから設定）
        [SerializeField]
        private InputActionReference Move;
        private void Awake()
        {
            moveSpeed *= Time.timeScale;
        }

        private void OnEnable()
        {
            Move.action.Enable();
        }

        private void OnDisable()
        {
            Move.action.Disable();
        }

        void Update()
        {
            HandleMove();
        }

        private void HandleMove()
        {
            // Vector2 で入力取得
            Vector2 input = Move.action.ReadValue<Vector2>();

            // 左右成分のみ使用（x）
            float moveX = input.x;

            Vector3 movement = new Vector3(moveX * moveSpeed * Time.deltaTime, 0f, 0f);

            transform.Translate(movement);
        }
    }
}

