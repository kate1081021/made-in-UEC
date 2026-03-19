using System.Collections;
using UnityEngine;

namespace EA
{
    public class EA_UIManager : MonoBehaviour
    {
        // ゲームクリア時の演出
        public void GameClear()
        {
            StartCoroutine(GameClearAnimation());
        }

        // ゲームクリア時に呼び出させるコルーチン
        private IEnumerator GameClearAnimation()
        {
            yield return null;
        }
        
    }
}
