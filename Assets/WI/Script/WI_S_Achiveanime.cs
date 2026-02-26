using System.Runtime.CompilerServices;
using UnityEngine;

namespace WI
{
    

    public class WI_S_Achiveanime : MiniGameBase
    {
        private float moveSpeed = 20.0f;
        private float delay = 1.0f;
        private float timer = 0.0f;
        private Vector2 targetPosition;
        private SpriteRenderer spriterender;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            spriterender = GetComponent<SpriteRenderer>();
            targetPosition = (Vector2)transform.position - new Vector2(6.3f, 0.0f);
        }
        public override void OnGameEnd()
        {
        }

        // Update is called once per frame
        void Update()
        {
            
            if(MGManager.IsClear){
                if(timer < delay)
            {
                timer += Time.deltaTime;
                return;
            }
                transform.position = Vector2.MoveTowards(
                transform.position, 
                targetPosition,
                moveSpeed * Time.deltaTime);
            }
        }
    }
}

