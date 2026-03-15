using UnityEngine;

namespace MyMiniGame
{
    public class BoomerangObstacle : MonoBehaviour
    {
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float topY = 3.0f;
        [SerializeField] private float bottomY = -3.0f;

        private bool movingUp = true;

        void Update()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (movingUp)
            {

                if (transform.position.y >= topY)
                {
                    movingUp = false;
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else
            {
                if (transform.position.y <= bottomY)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}