using Unity.VisualScripting;
using UnityEngine;

namespace RS
{
    public class RS_SpritesManager : MonoBehaviour
    {
        public bool is_spelled = false;
        public bool is_clear = false;
        [SerializeField] private GameObject start_background;
        [SerializeField] private GameObject[] clear_backgrounds;
        [SerializeField] private GameObject[] clear_characters;
        [SerializeField] private GameObject failed_background;
        [SerializeField] private GameObject[] failed_characters;
        private SpriteRenderer sr;
        private int rand;
        private float timer = 0;
        private float flash_time;

        void Awake()
        {
            rand = Random.Range(0, 3);
            sr = start_background.GetComponent<SpriteRenderer>();
            if (flash_time <= 0)
                flash_time = 0.5f;
        }

        void Update()
        {
            if (is_spelled)
            {
                if (is_clear)
                {
                    clear_backgrounds[rand].SetActive(true);
                    clear_characters[rand].SetActive(true);
                }
                else
                {
                    failed_background.SetActive(true);
                    failed_characters[rand].SetActive(true);
                }
                if (timer < flash_time)
                {
                    float c = timer / flash_time;
                    sr.color = new Color(c, c, c, 1);
                }
                else
                {
                    float c = 1f - (timer - flash_time) / flash_time;
                    sr.color = new Color(1, 1, 1, c);
                }
                timer += Time.deltaTime * Time.timeScale;
            }
        }
    }
}
