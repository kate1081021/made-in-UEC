using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UT
{

    public class UT_playermove : MiniGameBase
    {
        [SerializeField] private float movespeed;
        private Rigidbody2D rb;
        private Vector2 moveInput;
        public int HP;
        public int damage;
        public float duration;
        bool Invincible = false;
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
            if (collision.CompareTag("bullet") && !Invincible)
            {
                HP -= damage;
                Debug.Log("HP = " + HP);
                StartCoroutine(muteki());
                if (HP <= 0) Debug.Log("failure");
            }
        }
        IEnumerator muteki()
        {
            Invincible = true;
            for (int i = 0; i < 4; i++)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 0.3f);
                yield return new WaitForSeconds(0.1f);
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 1);
                yield return new WaitForSeconds(0.1f);
            }
            Invincible = false;
        }
    }
}
