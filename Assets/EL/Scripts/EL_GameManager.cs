using TMPro;
using UnityEngine;

namespace EL
{
	public class EL_GameManager : MiniGameBase
	{
		public static EL_GameManager Instance { get; private set; } // シングルトンインスタンス

		[SerializeField] private EL_VoltCalculator voltCalculator;

		[SerializeField] private TextMeshProUGUI voltText; // 電位表示用のテキストUI
		[SerializeField] private float targetVolt = 3f; // 目標電位
		[SerializeField] private float tolerance = 0.2f; //クリア判定の許容範囲
		[SerializeField] private float clearTime = 0.5f; //目標電位を維持する必要のある時間
		[SerializeField] private Bounds bounds; // 電圧値を探す範囲
		private void OnDrawGizmosSelected()
		{
			// シーンビューで範囲を視覚化
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}

		private float clearTimer = 0f; //クリアタイマー
		[HideInInspector] public bool isVoltInRange = false; //電位が許容範囲内かどうかのフラグ
		public override void OnGameStart()
		{
			// MGManager.TestPlay(100);
			MGManager.Load();

			if (voltCalculator == null)
			{
				voltCalculator = FindAnyObjectByType<EL_VoltCalculator>();
			}

			isVoltInRange = false;

			// シングルトンインスタンスの設定
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Debug.LogWarning("Multiple instances of EL_GameManager detected. There should only be one instance.");
				Destroy(this);
			}
		}

		void Update()
		{
			// 電位表示の更新
			voltText.text = $"{voltCalculator.volt:F2}";

			if (MGManager.IsClear)
			{
				return; // ゲームがクリアされた後は判定を行わない
			}

			// クリア判定: 電位が許容範囲内であればクリア
			if (Mathf.Abs(voltCalculator.volt - targetVolt) < tolerance && bounds.Contains(new Vector3(voltCalculator.transform.position.x, voltCalculator.transform.position.y, 0)))
			{
				isVoltInRange = true;
				clearTimer += Time.deltaTime * Time.timeScale;
				if (clearTimer >= clearTime)
				{
					MGManager.ClearGame();
				}
			}
			else
			{
				isVoltInRange = false;
				clearTimer = 0f; // 電位が範囲外になったらタイマーリセット
			}
		}
	}
}