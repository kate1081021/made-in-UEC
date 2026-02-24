using UnityEngine;
using System.Collections;

namespace NT
{
    public class NT_stamp : MiniGameBase
    {
        [Header("NT")]
        [SerializeField] private AnimationCurve stampCurve; 
        [SerializeField] private float duration = 0.2f; // アニメーションにかける時間
        [SerializeField] private GameObject stampMarkPrefab;

        public override void OnGameStart() {}
        public override void OnGameEnd() {}

        public void PressStamp(bool isSuccess)
        {
            // すでに動いている場合は二重起動しないように停止
            StopAllCoroutines(); // Allはこのスクリプト内のみが対象
            StartCoroutine(StampRoutine());
        }

        private IEnumerator StampRoutine()
        {
            float timer = 0f;
            Vector3 startScale = Vector3.one * 0.5f; // 開始サイズ
            Vector3 endScale = Vector3.one;          // 最終サイズ

            // 少しだけ角度をランダムに？
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float ratio = timer / duration;
                
                // Curveから、その時間の「勢い」を取得
                float curveValue = stampCurve.Evaluate(ratio);

                // サイズを計算して反映
                transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue);

                yield return null;
            }

            // 最後のサイズ合わせ
            transform.localScale = endScale;

            //LeaveMark();
        }

        void LeaveMark()
        {
            if (stampMarkPrefab != null)
            {
                Instantiate(stampMarkPrefab, transform.position, transform.rotation, transform.parent);
            }
        }
    }
}