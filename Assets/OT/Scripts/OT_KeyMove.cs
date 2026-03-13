using UnityEngine;

namespace OpenTreasure{
    public class OT_KeyMove : MiniGameBase
    {
        [SerializeField] int spinGoal;
        [SerializeField] OT_KeyAnim keyAnim;
        [SerializeField] GameObject Arrow;
        float angle = 0f;
        float prevAngle = 0f;
        float rotationSum = 0f;
        float partialSum;
        float deadZone = 0.8f;
        bool clearGame;
        public bool gameStarted = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            MGManager.Load();
            rotationSum = 0f;
            angle = 0f;
            prevAngle = 0f;
            partialSum = 0f;
            keyAnim = GetComponent<OT_KeyAnim>();
            gameStarted = false;
            clearGame = false;
            Arrow.SetActive(false);
            BGMPlay();
        }
        public void OnArrow()
        {
            Arrow.SetActive(true);
        }
        // Update is called once per frame
        void Update()
        {
            if (!gameStarted) { return; } // 開始演出など用
            Vector2 move = Move.ReadValue<Vector2>();

            if (move.magnitude < deadZone) { return; } // 入力が弱かったら終了

            angle = Mathf.Atan2(move.y,move.x)*Mathf.Rad2Deg;
            Debug.Log(angle + "," + prevAngle);
            float delta = Mathf.DeltaAngle(prevAngle,angle);
            prevAngle = angle;

            if (delta < 0f) { return; } // 時計回りじゃなかったら終了
            if (delta > 90f) { return; } // 90度以上の角度変更は対応しない(ちゃんとまわしてもらう)

            rotationSum += delta; partialSum += delta;
            while (partialSum >= 90f)
            {
                Debug.Log("90度回ったわ");
                keyAnim.UpdatePicture();
                partialSum -= 90f;
                SEPlay("OT_Kati");
            }
            if (rotationSum >= (float)spinGoal * 360f && !clearGame)
            {
                MGManager.ClearGame();
                keyAnim.ClearAnimation();
                Debug.Log("クリアしたわ");
                clearGame = true;
                Arrow.SetActive(false);
                SEPlay("OT_Open");
            }
        }
    }
}
