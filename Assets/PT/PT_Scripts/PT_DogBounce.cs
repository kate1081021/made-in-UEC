using UnityEngine;

namespace PTgame
{
    public class DogBounce : MonoBehaviour
    {
        [SerializeField] public float bounceHeight = 0.1f;
        [SerializeField] public float bounceSpeed = 10f;

        private Vector3 startPosition;
        private PT_DogMove parentScript;

        public void Awake()
        {
            // 最初にセットされた位置（(0,0,0)付近）を覚える
            startPosition = transform.localPosition;
            // 親の移動スクリプトを取得
            parentScript = GetComponentInParent<PT_DogMove>();
        }

        void Update()
        {
            // 親が止まっている（isStoppingがtrue）ときは揺らさない
            // ※元のスクリプトのisStoppingを参照したいですが、
            // privateなので今回はTime.deltaTimeが動いている間だけ揺れるようにします

            float yOffset = Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed)) * bounceHeight;

            // 親の移動に合わせて上下だけ上書き
            transform.localPosition = startPosition + new Vector3(0, yOffset, 0);
        }
    }
}
