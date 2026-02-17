using UnityEngine;
using UnityEngine.InputSystem;

namespace UT
{

    public class UT_playermove : MiniGameBase
    {
        [SerializeField] private float movespeed;
        private Rigidbody2D rb;
        private Vector2 moveInput;
        public override void OnGameStart()
        {
            MGManager.Load();
            rb = GetComponent<Rigidbody2D>();
        }
        void FixedUpdate()
        {
            Vector2 pos = Move.ReadValue<Vector2>() * movespeed * Time.timeScale * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + pos);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("bullet"))
            {
                Debug.Log("hit");
            }
        }
    }
}
