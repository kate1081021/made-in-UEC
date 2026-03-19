using System;
using UnityEngine;

namespace catchMochi
{
    public class MC_GirlModel
    {
        // 食べた餅の個数
        public int ateMochi;

        // ステータス
        public string status;

        // 餅を食べているかどうか
        public bool eating;

        // 餅を食べるスピード
        public int eatingSpeed;

        // 餅を食べるスピードの最大値
        public static int maximumSpeed = 50;

        // 餅を食べるスパンを計算するための倍率
        public static float multiple = 0.1f;

        // ステータスが変化した
        public Action<string> OnStatusChanged;

        // 初期化
        public MC_GirlModel()
        {
            ateMochi = 0;
            ChangeStatus("normal");
            eating = false;
            eatingSpeed = Math.Min(ateMochi, maximumSpeed);
        }

        // ステータスを変更&それをイベントして伝える
        public void ChangeStatus(string s)
        {
            // 変更前と後が異なる場合
            if (s != status) { OnStatusChanged?.Invoke(s); }

            // 変更を反映
            status = s;

        }

        // 食べ始める
        public void StartEating()
        {
            ChangeStatus("catch");
            eating = true;
        }

        // 食べるのを途中でキャンセルする
        public void CancelEating()
        {
            ChangeStatus("normal");
            eating = false;
        }

        // 餅を食べる間隔を計算する(補正をかける)
        public float CaliculateEatingSpan(float seconds)
        {
            // multipleの標準値:0.1 0.1で大体50で収束
            float factor = 1 - 0.5f*(1 - (float)Math.Exp(-multiple*eatingSpeed));
            return seconds * factor;
        }
    }
}
