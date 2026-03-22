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
		private bool canSubmitFlag = false;
		public override void OnGameStart()
		{
			submitAnimation = GetComponent<Animator>();
			submitAnimation.SetBool("IsSubmitted", false);
			canSubmit = false;
			canSubmitFlag = false;
		}

		void Update()
		{
			if (ER_GameManager.Instance.isSubmitted)
			{
				submitAnimation.SetBool("IsSubmitted", true);
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

		protected override void OnMoveCanceled(Vector2 value)
		{
			if (transform.position.y >= maxPosY)
			{
				canSubmitFlag = true;
			}
			else
			{
				canSubmitFlag = false;
			}
		}

		protected override void OnMovePerformed(Vector2 value)
		{
			if (value.y > 0.9f && canSubmitFlag)
			{
				ER_GameManager.Instance.isSubmitted = true;
			}
		}
	}
}