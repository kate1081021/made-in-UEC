using UnityEngine;

namespace WI
{
    

    public class WI_S_clearanimation : MiniGameBase
    {
        private float moveSpeed = 5.0f;
        private Vector2 targetPosition;
        private SpriteRenderer spriterender;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            spriterender = GetComponent<SpriteRenderer>();
            targetPosition = (Vector2)transform.position - new Vector2(0.0f, 3.0f);
        }
        public override void OnGameEnd()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(MGManager.IsClear){

            transform.position = Vector2.MoveTowards(
                transform.position, 
                targetPosition,
                moveSpeed * Time.deltaTime);
            }
        }
    }
}