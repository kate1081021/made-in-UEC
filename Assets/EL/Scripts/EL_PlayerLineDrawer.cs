using UnityEngine;

namespace EL
{
    public class EL_PlayerLineDrawer : MiniGameBase
    {
        private LineRenderer lr;
        private EdgeCollider2D edge;
        private int positionCount;
        private Camera mainCamera;
        [SerializeField] private Transform Probe;
        [SerializeField] private EL_GameManager gameManager;
        public override void OnGameStart()
        {
            lr = GetComponent<LineRenderer>();
            //ラインの座標指定を,このラインオブジェクトのローカル座標系を基準にするように設定を変更
            lr.useWorldSpace = false;
            positionCount = 0;
            mainCamera = Camera.main;
            edge = gameObject.AddComponent<EdgeCollider2D>();
            edge.isTrigger = true;
        }

        void Update()
        {
            //このラインオブジェクトを,位置はカメラ前方10m,回転はカメラと同じになるようにキープさせる
            transform.position = mainCamera.transform.position + mainCamera.transform.forward * 10;
            transform.rotation = mainCamera.transform.rotation;

            //Probeがbounds内にある場合,Actionボタンが入力されている間,毎フレームLineRendererに座標を格納し続ける
            if (Action.IsPressed() && gameManager.bounds.Contains(Probe.transform.position))
            {
                //ライン生成時に格納する座標がProbeと一致するようにする
                Vector3 pos = Probe.transform.position;
                pos.z = 10.0f;

                positionCount++;
                lr.positionCount = positionCount;
                lr.SetPosition(positionCount -1, pos);
            }

            //Actionボタンが離されたとき,またはProbeがbounds外に出たとき,リセットしつつ生成ラインにコライダーを付与
            //bounds内で再度再度ボタンが押されたとき,古いラインが消滅する
            if (Action.WasReleasedThisFrame() || !gameManager.bounds.Contains(Probe.transform.position))
            {
                positionCount = 0;
                BuildCollider();
            }
        }

        //コライダー生成の関数.今回EdgeCollider2Dを採用
        public void BuildCollider()
        {
            int count = lr.positionCount;
            //EdgeCollider2Dは二次元配列
            Vector2[] points = new Vector2[count];

            for(int i = 0; i < count; i++)
            {
                Vector3 p = lr.GetPosition(i);
                points[i] = new Vector2(p.x, p.y);
            }

            edge.points = points;
        }
    }
}
//※LineRendereの仕組みとしては,Positionに座標を格納し,格納された座標同士をCount順に直線で結ぶというもののようだ
//したがって,座標に変化が必要なので,Probeの子オブジェクトとするのはタブー