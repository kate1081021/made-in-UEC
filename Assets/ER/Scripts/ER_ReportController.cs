using UnityEngine;

namespace ER
{
	public class ER_ReportController : MiniGameBase
	{
		[SerializeField] private float scrollSpeed = 50f; // スクロール速度
		[SerializeField] private float maxPosY = 100f; // 最大スクロール量
		[SerializeField] private float minPosY = 0f; // 最小スクロール量

		private Animator submitAnimation;
		[HideInInspector] public bool canSubmit = false;
		public override void OnGameStart()
		{
			submitAnimation = GetComponent<Animator>();
			submitAnimation.SetBool("isSubmitted", false);
			canSubmit = false;
		}

		void Update()
		{
			if (ER_GameManager.Instance.isSubmitted)
			{
				submitAnimation.SetBool("isSubmitted", true);
			}
			else
			{
				MoveReport();
			}
		}

		private void MoveReport()
		{
			Vector2 moveValue = Move.ReadValue<Vector2>();
			Vector3 currentPos = transform.position;
			currentPos.y += moveValue.y * scrollSpeed * Time.deltaTime * Time.timeScale;
			currentPos.y = Mathf.Clamp(currentPos.y, minPosY, maxPosY);
			transform.position = currentPos;
			if (transform.position.y >= maxPosY)
			{
				canSubmit = true;
			}
			else
			{
				canSubmit = false;
			}
		}

		protected override void OnMoveStarted(Vector2 value)
		{
			if (value.y > 0.9f && canSubmit)
			{
				ER_GameManager.Instance.isSubmitted = true;
			}
		}
	}
}