using UnityEngine;

namespace BOSS
{
    public class BOSS_TestManager : MonoBehaviour
    {
        [Header("テスト用の設定")]
        public BOSS_ObstacleManager obstacleManager; // 降ってくる障害物のマネージャー
        public GameObject ghostPrefab;               // お化けのプレハブ
        public Transform targetObject;               // ★暗闇の中心にしたいオブジェクト（プレイヤーの代わり）

        void Start()
        {
            // ゲーム開始と同時に、上から降ってくる障害物をスタート！
            if (obstacleManager != null)
            {
                obstacleManager.StartSpawning();
            }
        }

        void Update()
        {
            // ★「Sキー」が押されたらお化けを出現させる
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (ghostPrefab != null)
                {
                    // お化けを生成
                    GameObject ghostObj = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
                    BOSS_GhostObstacle ghostScript = ghostObj.GetComponent<BOSS_GhostObstacle>();
                    
                    if (ghostScript != null)
                    {
                        // ★「このオブジェクトを中心に暗くして！」とターゲットを渡して起動
                        ghostScript.Init(targetObject);
                    }
                }
            }
        }
    }
}