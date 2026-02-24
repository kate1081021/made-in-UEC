using System.Collections;
using UnityEngine;

namespace ER
{
	public class ER_Stamp : MiniGameBase
	{
		[SerializeField] private Sprite correctStamp;
		[SerializeField] private Sprite incorrectStamp;
		private SpriteRenderer spriteRenderer;
		private Animator animator;

		public override void OnGameStart()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			animator = GetComponent<Animator>();
			spriteRenderer.enabled = false; // 最初はスタンプを非表示にする
			animator.SetBool("IsSubmitted", false); // アニメーションの初期状態を設定する
		}

		public void ShowStamp(bool isCorrect)
		{
			if (isCorrect)
			{
				spriteRenderer.sprite = correctStamp;
			}
			else
			{
				spriteRenderer.sprite = incorrectStamp;
			}
			animator.SetBool("IsSubmitted", true); // アニメーションを開始する
		}
	}
}