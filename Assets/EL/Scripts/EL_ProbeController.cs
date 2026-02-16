using UnityEngine;

namespace EL
{
	public class EL_ProbeController : MiniGameBase
	{
		[SerializeField] private float force = 10f;
		[SerializeField] private float margin = 0.5f; // 画面端から移動範囲を少し内側にするためのマージン
		private Rigidbody2D rb;

		public override void OnGameStart()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			Vector2 moveValue = Move.ReadValue<Vector2>();
			MoveProbe(moveValue);
		}

		private void MoveProbe(Vector2 direction)
		{
			rb.AddForce(direction * Time.timeScale * force);
			// 画面外に出ないようにする
			Vector2 maxPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
			Vector2 minPosition = Camera.main.ScreenToWorldPoint(Vector2.zero);
			Vector3 clampedPosition = new Vector3(
				Mathf.Clamp(transform.position.x, minPosition.x + margin, maxPosition.x - margin),
				Mathf.Clamp(transform.position.y, minPosition.y + margin, maxPosition.y - margin),
				0
			);
			transform.position = clampedPosition;
		}
	}
}