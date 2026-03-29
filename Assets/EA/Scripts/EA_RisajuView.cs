using System.Collections;
using UnityEngine;

namespace EA
{
    public class EA_RisajuView : MonoBehaviour
    {
        public GameObject obj;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Start()
        {
            obj.SetActive(false);
        }

        // アニメーションとして管理
        public IEnumerator Animation()
        {
            yield return new WaitForSeconds(0.5f);
            obj.SetActive(true);
        }
    }
}
