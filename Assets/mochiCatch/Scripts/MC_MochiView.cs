using UnityEngine;
using UnityEngine.UI;

namespace catchMochi
{
    public class MC_MochiView : MonoBehaviour
    {
        // アニメーション
        public GameObject[] mochi;

        public void Start()
        {
            mochi[1].SetActive(true);
            mochi[2].SetActive(true);
            mochi[3].SetActive(true);
        }

        // 指定餅の表示
        public void ShowMochi(int target)
        {
            mochi[target].SetActive(true);
        }
        // 指定餅の非表示
        public void HideMochi(int target)
        {
            mochi[target].SetActive(false);
        }

    }
}
