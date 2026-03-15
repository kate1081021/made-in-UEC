using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace SL
{
    public class SL_AudioManager : MiniGameBase
    {

        [Header("SL_サウンド")]
        public AudioSource SL_audioSource = null;
        public AudioClip SL_BGM;
        public AudioClip SL_AttackAudioClip;
        public AudioClip SL_AttackAudioClip2;
        public AudioClip SL_BreakAudioClip;
        public AudioClip SL_Fanfare;
        public AudioClip SL_Shine;

        public override void OnGameStart()
        {
            //SL_audioSource = GetComponent<AudioSource>();
        }

        public void SL_BGMStart()
        {
            //SL_audioSource.clip = SL_BGM;
            //SL_audioSource.loop = true;
            BGMPlay(true);
            //SL_audioSource.Play();
        }
        public void SL_BGMStop()
        {
            //SL_audioSource.Stop();
        }
        public void AttackSe()
        {
            int AudioChoice = Random.Range(0,2);
            if (AudioChoice == 0)
            {
                SEPlay("SL_Tsutsuku");
                //SL_audioSource.PlayOneShot(SL_AttackAudioClip);
            }
            else
            {
                SEPlay("SL_Tsutsuku2");
                //SL_audioSource.PlayOneShot(SL_AttackAudioClip2);
            }
            }
        public void GameClearSe()
        {
            SEPlay("SL_Break");
            SEPlay("SL_Fanfare");
            SEPlay("SL_Shine");
            //SL_audioSource.PlayOneShot(SL_BreakAudioClip);
            //SL_audioSource.PlayOneShot(SL_Fanfare);
            //SL_audioSource.PlayOneShot(SL_Shine);
        }
    }
}