using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace catchMochi
{
    public class GameManager : MonoBehaviour
    {
        public int score;  // スコアの管理
        public float time; // ゲーム開始からの経過時間
        public Mother mother;  // お母さんオブジェクトを参照
        public mochiCatch girl;  // 女の子を管理
        public Girl_Anim girlanim;
        public Image girl_picture;
        public Sprite girl_bikkuri_picture;
        public bool game_in_progress;
        public GameObject suggestion;
        public GameObject panelOfTitle;
        public GameObject panelOfGirl;
        public RectTransform Shouji;
        public Animator Shouji_anim;
        public bool isResult = false;
        // Update is called once per frame
        void Start()
        {
            girl_picture = girl.GetComponent<Image>();
            StartCoroutine(Title());
        }
        private IEnumerator Title()
        {
            // パラメータリセット
            game_in_progress = true;
            panelOfGirl.SetActive(true);
            mother.gameObject.SetActive(true);
            suggestion.SetActive(true);
            mother.enabled = true;
            time = 0;
            Shouji.position = new Vector2(262,90.15705f);

            // 呼び出されたことを確認する
            MGManager.Loaded();

            yield return null;
        }

        void Update()
        {
            // ユーザーガイドを消す
            if (time > 3f) { suggestion.SetActive(false); }
            
            // ゲームクリア
            if (girl.ateMochi == 3)
            {
                MGManager.ClearGame();
            }
            
            // GameOver
            if (mother.isMotherSeeing && girl.status == "eat" && game_in_progress)
            {
                isResult = true;
                game_in_progress = false;
                girl.enabled = false;
                girlanim.enabled = false;
                mother.finish = true;
                girl_picture.sprite = girl_bikkuri_picture;
                score = (int)Mathf.Round(6670f*((float)Math.Log10(Mathf.Pow(girl.ateMochi,2)/(time) + 1)));
                // StartCoroutine(end());
            }
            else if (game_in_progress)
            {
                time += Time.deltaTime;
            }
        }
    }
}
