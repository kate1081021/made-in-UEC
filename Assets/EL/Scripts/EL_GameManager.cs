using TMPro;
using UnityEngine;

namespace EL
{
	public class EL_GameManager : MiniGameBase
	{
		public static EL_GameManager Instance { get; private set; } // シングルトンインスタンス

		[SerializeField] private EL_VoltCalculator voltCalculator;

		[SerializeField] private TextMeshProUGUI voltText; // 電位表示用のテキストUI
		[SerializeField] private TextMeshProUGUI targetVoltText; // 目標電位表示用のテキストUI
		[SerializeField] private EL_StageData stageData; // ステージデータ
		private float targetVolt = 3f; // 目標電位
		private float tolerance = 0.2f; //クリア判定の許容範囲
		private float clearTime = 0.5f; //目標電位を維持する必要のある時間

		public Bounds bounds; // 電圧値を探す範囲
		private void OnDrawGizmosSelected()
		{
			// シーンビューで範囲を視覚化
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}

		private float clearTimer = 0f; //クリアタイマー
		[HideInInspector] public bool isVoltInRange = false; //電位が許容範囲内かどうかのフラグ

		private void Awake()
		{
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

		public override void OnGameStart()
		{
			// MGManager.TestPlay(100);
			MGManager.Load();

			if (voltCalculator == null)
			{
				voltCalculator = FindAnyObjectByType<EL_VoltCalculator>();
			}

			isVoltInRange = false;

			// ステージデータから目標電位と許容範囲を取得
			if (stageData != null && stageData.clearConditions.Count > 0)
			{
				int randomIndex = Random.Range(0, stageData.clearConditions.Count);
				targetVolt = stageData.clearConditions[randomIndex].targetVolt;
				tolerance = stageData.clearConditions[randomIndex].tolerance;
				bounds = stageData.clearConditions[randomIndex].bounds;
			}
			else
			{
				Debug.LogWarning("StageData is not set or has no clear conditions. Using default values.");
			}

			// 目標電位の表示更新
			targetVoltText.text = $"欠けた部分から\n({targetVolt:F2} ± {tolerance:F2}) Vを探せ！";
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