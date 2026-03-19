using UnityEngine;
using UnityEngine.EventSystems;

namespace garbage
{
    public class GB_GarbageMover : MiniGameBase
    {
    public override void OnGameStart()
    {
            
    }

    void Update()
    {
        garbageMovement(Vector2.down);

        if (transform.position.y < -3.3f)
        {
            Destroy(this.gameObject);
        }
    }
        private void garbageMovement(Vector3 moveDirection)
        {
            var pos = transform.position;

            var moveSpeed = 1;

            pos += moveDirection * moveSpeed * Time.deltaTime;

            transform.position = pos;
        }
    }
}