using UnityEngine;

namespace UT
{
    public class UT_enemymove : MiniGameBase
    {
        float time = 0;
        [SerializeField]
        [Tooltip("単位のx座標")]
        float speed;
        [SerializeField]
        [Tooltip("単位のx座標")]
        float MinHeight;
        [SerializeField]
        [Tooltip("単位のx座標")]
        float offset;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {

        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            float scale = Mathf.Cos(time * speed);
            transform.localScale = new Vector3(0.6f, 0.6f*(MinHeight + (1 - MinHeight) * (scale +1)/2), 0);
            transform.position = new Vector3(transform.position.x, 2.65f - offset+ (scale + 1) / 2 * offset , 0);
        }
    }
}
