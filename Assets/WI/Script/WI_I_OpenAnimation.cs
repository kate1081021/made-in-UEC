using UnityEngine;
using System.Collections;
namespace WI
{
    public class WI_I_OpenAnimation : MiniGameBase
    {
        [Header("アニメーション設定")]
        public float duration = 0.5f;
        public AnimationCurve spawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 _targetScale;
        private bool _isClone = false;

        void Awake()
        {
            _targetScale = transform.localScale;

            if (gameObject.name == "WI_M_popup(Clone)")
            {
                _targetScale = new Vector3(0.4697029f, 0.4697029f, 1.0f);
            }

            if (gameObject.name.Contains("(Clone)"))
            {
                _isClone = true;
                transform.localScale = Vector3.zero;
            }
        }

        public override void OnGameStart()
        {
            if (_isClone)
            {
                StartCoroutine(AnimateIn());
            }
        }

        private IEnumerator AnimateIn()
        {
            float elapsed = 0f;

            if (_targetScale == Vector3.zero) _targetScale = Vector3.one;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);
                float curveValue = spawnCurve.Evaluate(percent);


                transform.localScale = _targetScale * curveValue;

                yield return null;
            }

            transform.localScale = _targetScale;
        }
    }
}