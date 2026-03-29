using UnityEngine;

namespace BOSS
{
    public class BOSS_MapMoveManager : MiniGameBase
    {
        [SerializeField] private BOSS_goalManager goalManager;

        [Header("マップオブジェクト")]
        [SerializeField] private GameObject loopingMap; // 今動いてるループ用マップ
        [SerializeField] private GameObject goalMap;    // 画面外に待機してるゴール用マップ

        public float BOSS_MapSpeed;
        Rigidbody2D BOSSMaprigidbody2D;
        Transform BOSSTransform;
        private bool isGoalTime = false;

        public override void OnGameStart()
        {
            BOSSMaprigidbody2D = this.GetComponent<Rigidbody2D>();
            BOSSTransform = this.GetComponent<Transform>();

            // 念のため最初はゴールマップを非表示にしておく
            if (goalMap != null) goalMap.SetActive(false);
        }

        void Update()
        {
            if (!isGoalTime)
            {
                BOSSMaprigidbody2D.linearVelocityY = BOSS_MapSpeed;

                if (BOSSTransform.position.y < -18.5f)
                {
                    BOSSTransform.position = new Vector3(BOSSTransform.position.x, 8.5f, BOSSTransform.position.z);
                }
            }
        }

        public void MoveToFinalGoal()
        {
            isGoalTime = true;
            BOSSMaprigidbody2D.linearVelocityY = 0; // 動きを止める
            if (loopingMap != null) loopingMap.SetActive(false); // 元のマップを消去

            if (goalMap != null)
            {
                goalMap.SetActive(true); // ゴールマップ召喚
                // 画面の真ん中（Vector3.zeroなど）に配置
                goalMap.transform.position = new Vector3(0, 0, 0);
            }

            // ちょっと余韻を作ってからリザルト出したいなら、
            // ここで少し待ってからCompleteGoalSequenceを呼ぶのもアリ
            // goalManager.CompleteGoalSequence();
        }
    }
}