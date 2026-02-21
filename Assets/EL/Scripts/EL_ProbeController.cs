using UnityEngine;

namespace EL
{
	public class EL_ProbeController : MiniGameBase
	{
		[SerializeField] private float force = 100f;
		[SerializeField] private float margin = 0.5f; // 画面端から移動範囲を少し内側にするためのマージン
		[SerializeField] private float originforce = 100f; //初期移動速度
		[SerializeField] private float slowforce = 30f; //bounds内での低下移動速度
		[SerializeField] private EL_GameManager gameManager;

		private Rigidbody2D rb;

		public override void OnGameStart()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			if (EL_GameManager.Instance.isVoltInRange)
			{
				// 電圧値が許容範囲内であれば何かしらのフィードバックを与える（例: 色を変える、エフェクトを出すなど）
				// TODO: 仮なのでUpdate内でGetComponentしていますが，重くなるのでフィードバックの方針が決まったら変更します
				GetComponentInChildren<SpriteRenderer>().color = Color.green; // 仮：緑色にする
			}
			else
			{
				GetComponentInChildren<SpriteRenderer>().color = Color.white; // 元の色に戻す
			}

			if (gameManager.bounds.Contains(transform.position) && Action.IsPressed())
			{
				force = slowforce;
			}
			else
			{
				force = originforce;
			}
		}

		void FixedUpdate()
		{
			Vector2 moveValue = Move.ReadValue<Vector2>();
			MoveProbe(moveValue);
		}

		private void MoveProbe(Vector2 direction)
		{
			rb.AddForce(direction * Time.timeScale * force);
			// 画面外に出ないようにする
			Vector2 maxPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)); // 画面右上のワールド座標を取得
			Vector2 minPosition = Camera.main.ScreenToWorldPoint(Vector2.zero); // 画面左下のワールド座標を取得
			Vector3 clampedPosition = new Vector3(
				Mathf.Clamp(transform.position.x, minPosition.x + margin, maxPosition.x - margin),
				Mathf.Clamp(transform.position.y, minPosition.y + margin, maxPosition.y - margin),
				0
			); // プローブの位置をクランプして画面内に収める(マージンを考慮)
			transform.position = clampedPosition;
		}
	}
}