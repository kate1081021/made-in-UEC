using UnityEngine;

namespace RS
{
    public class RS_Character : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private float spelled_time;
        [SerializeField] private float timer = 0;
        [SerializeField] private float start_position;
        [SerializeField] private float spelled_position;
        void Awake()
        {
            if (spelled_time <= 0)
                spelled_time = 0.5f;
        }

        void Update()
        {
            timer += Time.deltaTime * Time.timeScale;
            if (timer < spelled_time)
            {
                
            }
        }
    }
}