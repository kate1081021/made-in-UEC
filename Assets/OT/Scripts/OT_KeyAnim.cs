using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OpenTreasure {
public class OT_KeyAnim : MiniGameBase
{
    [SerializeField] List<Sprite> Key_picture;
    [SerializeField] Animator anim;
    OT_KeyMove keyMove;
    Image thisPicture;
    RectTransform pos;
    int nowSprite;
    [SerializeField] Animator animParent;

        public override void OnGameStart()
        {
            thisPicture = this.GetComponent<Image>();
            keyMove = GetComponent<OT_KeyMove>();
            pos = GetComponent<RectTransform>();
            anim = GetComponent<Animator>();
            nowSprite = 0;
            thisPicture.sprite = Key_picture[nowSprite];
            Debug.Log(animParent);
            animParent.ResetTrigger("Clear");
        }
        void Update()
        {
            var state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= (double)1.0 && !keyMove.gameStarted)
            {
                keyMove.gameStarted = true;
                Debug.Log("終わりました！");
            }
        }
        public void UpdatePicture()
        {
            ++nowSprite;
            if (nowSprite > Key_picture.Count - 1) { nowSprite = 0; }
            thisPicture.sprite =Key_picture[nowSprite];
        }
        public void ClearAnimation()
        {
            animParent.SetTrigger("Clear");
            StartCoroutine(PlaySound());
        }
        IEnumerator PlaySound()
        {
            float time = 0f;
            while (time <= 0.67)
            {
                yield return null;
                time += Time.deltaTime;
            }
            Debug.Log("now");
            SEPlay("OT_Clear");
        }
    }
}