using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NT
{
    public class NT_stamp : MiniGameBase
    {
        [Header("Stamp Sprites")]
        [SerializeField] private Image stampImage;
        [SerializeField] private Image stampEffect;
        [SerializeField] private Sprite excellentSprite;
        [SerializeField] private Sprite yuuSprite;
        [SerializeField] private Sprite ryouSprite;
        [SerializeField] private Sprite kaSprite;
        [SerializeField] private Sprite badSprite;

        [SerializeField] private AnimationCurve stampCurve;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private RectTransform handImage;
        [SerializeField] private Vector3 handStartPosition;
        [SerializeField] private Vector3 handEndPosition;
        private AudioSource stampAudioSource;

private IEnumerator StampRoutine(float count)
{
    Debug.Log("Hand Position: " + handImage.localPosition);
    float timer = 0f;

    Vector3 handStart = handStartPosition;
    Vector3 handEnd = handEndPosition;

    handImage.localPosition = handStart;
    stampImage.GetComponent<RectTransform>().localPosition = new Vector2(0, 3000f);

    // 手が降りてくる
    while (timer < duration)
    {
        timer += Time.deltaTime;
        float ratio = timer / duration;

        handImage.localPosition =
            Vector3.Lerp(handStart, handEnd, ratio);

        yield return null;
    }
    stampEffect.gameObject.SetActive(true);
    stampAudioSource.Play();

    // ★ 押した瞬間
    ChangeStampByCount(count);
    stampImage.GetComponent<RectTransform>().localPosition = Vector2.zero;

    // バウンスアニメ
    float bounceTimer = 0f;
    Vector3 startScale = transform.localScale * 0.5f;
    Vector3 endScale = transform.localScale;

    transform.localScale = startScale;

    while (bounceTimer < duration)
    {
        bounceTimer += Time.deltaTime;
        float ratio = bounceTimer / duration;

        float curveValue = stampCurve.Evaluate(ratio);
        transform.localScale =
            Vector3.LerpUnclamped(startScale, endScale, curveValue);

        yield return null;
    }

    transform.localScale = endScale;
    handImage.localPosition = new Vector3(0, 3000f, 0);
    stampEffect.gameObject.SetActive(false);
}

        public override void OnGameStart()
        {
            stampAudioSource = GetComponent<AudioSource>();
            stampImage.GetComponent<RectTransform>().localPosition = new Vector2(0, 3000f);
            handImage.localPosition = new Vector3(0, 3000f, 0);
            stampEffect.gameObject.SetActive(false);
        }
        public override void OnGameEnd() {}

        public void PressStamp(float count)
        {
            Debug.Log("PressStamp called with count: " + count);
            // すでに動いている場合は二重起動しないように停止
            StopAllCoroutines(); // Allはこのスクリプト内のみが対象
            StartCoroutine(StampRoutine(count));
        }

                private void ChangeStampByCount(float count)
        {
            if (count >= 170 && count < 180)
            {
                stampImage.sprite = excellentSprite;
            }
            else if ((count >= 160 && count < 170) || (count >= 180 && count < 190))
            {
                stampImage.sprite = yuuSprite;
            }
            else if ((count >= 155 && count < 160) || (count >= 190 && count < 195))
            {
                stampImage.sprite = ryouSprite;
            }
            else if ((count >= 150 && count < 155) || (count >= 195 && count < 200))
            {
                stampImage.sprite = kaSprite;
            }
            else
            {
                stampImage.sprite = badSprite;
            }
        }
    }
}