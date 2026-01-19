using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace catchMochi
{
    public class mochiCatch : MonoBehaviour
    {
        public bool eating = false;
        public List<float> waitSeconds;
        public string status = "normal";
        public int ateMochi = 0;
        public int maximum;
        public bool isCancel = false;
        public float multiple;
        public Mother mother;
        public void clickMochiAction()
        {
            if (!eating) // 食事を始める
            {
                eating = true;
                status = "catch";
                Debug.Log("食事開始");
                StartCoroutine(startEating());
            }
        }
        public void endMochiAction()
        {
            if (eating)
            {
                isCancel = true;
                switch (status)
                {
                    case "catch":
                    Debug.Log("キャッチキャンセル");
                    break;
                    case "draw":
                    Debug.Log("口に運ぶのをキャンセル");
                    break;
                    case "eat":
                    Debug.Log("無理やり食べた");
                    break;
                }
            }
        }

        private bool KeyInput()
        {
            return Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);
        }

        private IEnumerator WaitPhase(float seconds)
        {
            float end = Time.time + seconds;
            while ((KeyInput() || status == "eat") && Time.time <= end)
            {
                yield return null;
            }
        }
        private void HandleCancel()
        {
            status = "normal";
            eating = false;
        }

        IEnumerator startEating() {
            while (KeyInput()){
                Debug.Log("eating");
                int speedUp = Math.Min(ateMochi, maximum);
                // multipleの標準値:0.1 0.1で大体50で収束
                float factor = 1 - 0.5f*(1 - (float)Math.Exp(-multiple*speedUp));
                status = "catch";
                string[] phases = {"catch","draw","eat"};
                for (int i = 0; i<3; i++)
                {
                    status = phases[i];
                    // statusが"eat"であるとき、食べるSEを鳴らす
                    if (status == "eat" && !mother.isMotherSeeing)
                    {
                        // SePlayer.Instance.Play("食べ物をパクッ");
                    }
                    yield return WaitPhase(waitSeconds[i] * factor);
                    if (!KeyInput())
                    {
                        // statusが"eat"である場合、ateMochiを一つ追加
                        if (status == "eat" && !mother.isMotherSeeing) { ateMochi++; }
                        HandleCancel();
                        isCancel = false;
                        yield break;
                    }
                }
                Debug.Log("食事成功！");
                status = "normal";
                eating = false;
                if (!mother.isMotherSeeing) { ateMochi++; }
            }
            isCancel = false; // キャンセルフラグのリセットが必要ならここで
        }
        //食べるほど速度上昇、上限あり
    }
}