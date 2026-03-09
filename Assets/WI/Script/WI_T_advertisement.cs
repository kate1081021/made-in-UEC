using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace WI
{
    public class WI_T_advertisement : MiniGameBase 
    {
        private bool isClosing = false;
        [SerializeField] private bool useAnimation = true;

        private SortingGroup sr, parentSr;

        // SE
        private AudioSource audioSource;
        [SerializeField] private AudioClip destroySE;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart() 
        {
            if ((audioSource = this.GetComponent<AudioSource>()) == null)
            {
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

            if (this.GetComponent<SortingGroup>() != null)
            {
                sr = this.GetComponent<SortingGroup>();
            }
            if(this.transform.parent != null)
            {
                if(this.transform.parent.GetComponent<SortingGroup>() != null)
                {
                    parentSr = this.transform.parent.GetComponent<SortingGroup>();
                }
            }
            if(sr != null && parentSr != null)
            {
                sr.sortingOrder = parentSr.sortingOrder + 1;
            }
        }

        public override void OnGameEnd() { }
        void Update() { }

        public void setInputClose()
        {
            if (isClosing) return;
            SEPlay("WI_Close", false);
            //soundPlay(audioSource, destroySE);
            StartCoroutine(AnimateAndDestroy());

            if (useAnimation)
            {
                StartCoroutine(AnimateAndDestroy());
            }
            else
            {
                isClosing = true;
                CloseWindow();
            }
        }

        public void CloseWindow()
        {
            Destroy(this.gameObject);
        }

        private IEnumerator AnimateAndDestroy()
        {
            isClosing = true;

            float duration = 0.15f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / duration;

                float curve = 1f - percent;
                transform.localScale = new Vector3(startScale.x * curve, startScale.y * curve, startScale.z);

                yield return null;

            }

            Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }

        private void soundPlay(AudioSource audioSource, AudioClip audioClip)
        {
            if (audioSource != null && audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.PlayOneShot(audioClip);
            }
        }
    }
}
