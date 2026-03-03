using UnityEngine;
using System.Collections;

namespace WI
{

    public class WI_M_buttonManager : MiniGameBase
    {
        private bool isClosing = false;
        private GameObject closeButton;
        [SerializeField] private bool useAnimation = true;

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

            this.gameObject.SetActive(true);

            closeButton = this.transform.GetChild(0).gameObject;
            closeButton.GetComponent<WI_M_inputClose>().enabled = true;
            closeButton.GetComponent<WI_M_createPop>().enabled = false;
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {

        }

        public void setInputClose()
        {
            if (isClosing) return;
            // SEPlay("closeWindow", false);
            soundPlay(audioSource, destroySE);
            StartCoroutine(AnimateAndDestroy());

            if (useAnimation)
            {
                StartCoroutine(AnimateAndDestroy());
            }
            else
            {
                isClosing = true;
                DestroyImmediate();
            }
        }

        private void DestroyImmediate()
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

