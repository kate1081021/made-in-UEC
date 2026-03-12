using UnityEngine;

namespace RS
{
    public class RS_Character : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Texture2D t2;
        [SerializeField] private Material mat;
        [SerializeField] private float spelled_time;
        [SerializeField] private float timer = 0;
        [SerializeField] private float start_position;
        [SerializeField] private float spelled_position;
        void Awake()
        {
            if (spelled_time <= 0)
                spelled_time = 0.5f;
            mat = sr.material;
            if (t2 != null)
                mat.SetTexture("_Texture2D", t2);
        }

        void Update()
        {
            timer += Time.deltaTime * Time.timeScale;
            if (timer < spelled_time)
            {
                //mat.SetFloat("_Float", timer / spelled_time);
            }
            else if (timer < spelled_time*2)
            {
                mat.SetFloat("_Float", (timer - spelled_time) / spelled_time);
            }
            else
            {
                mat.SetFloat("_Float", 1);
            }
        }
    }
}