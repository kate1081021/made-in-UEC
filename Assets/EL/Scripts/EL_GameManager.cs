using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EL
{
	public class EL_GameManager : MiniGameBase
	{
		public static EL_GameManager Instance { get; private set; } // シングルトンインスタンス

		[SerializeField] private EL_VoltCalculator voltCalculator;
		[SerializeField] private EL_LineDrawer lineDrawer;
		private List<Vector3> targetPoints; // 欠けた部分の座標リスト
		[SerializeField] private LineRenderer playerLineRenderer;

		[SerializeField] private TextMeshProUGUI voltText; // 電位表示用のテキストUI
		[SerializeField] private TextMeshProUGUI targetVoltText; // 目標電位表示用のテキストUI
		[SerializeField] private float clearRatio = 0.7f; // クリア判定のための一致率
		[SerializeField] private float errorMultiplier = 2f; // クリア判定のための距離閾値にかける倍率
		[SerializeField] private EL_StageData stageData; // ステージデータ
		private float targetVolt = 3f; // 目標電位
		private float tolerance = 0.2f; //クリア判定の許容範囲
		private float errorDistance; // 座標の一致判定のための距離閾値

		public Bounds bounds; // 電圧値を探す範囲
		private void OnDrawGizmosSelected()
		{
			// シーンビューで範囲を視覚化
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}

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
			// MGManager.TestPlay(500);
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
			targetVoltText.text = $"{targetVolt:F2} Vの欠けた等電位線を完成させろ！";


		}

		public void CheckClearCondition()
		{
			// 欠けた部分の座標をLineDrawerから取得
			targetPoints = lineDrawer.allTargetPoints;
			List<Vector3> playerDrawnPoints = new List<Vector3>();

			// 成功判定点の隣り合う点同士の距離の最小値のerrorMultiplier倍をerrorDistanceとする
			errorDistance = Vector2.Distance(targetPoints[0], targetPoints[1]);
			for (int i = 1; i < targetPoints.Count; i++)
			{
				for (int j = i + 1; j < targetPoints.Count; j++)
				{
					float distance = Vector2.Distance(targetPoints[i], targetPoints[j]);
					if (distance < errorDistance)
					{
						errorDistance = distance;
					}
				}
			}
			errorDistance *= errorMultiplier * Time.timeScale; // 時間が早くなったときにクリアが難しくなりすぎないようにTime.timeScaleをかける
			Debug.Log($"Error distance set to: {errorDistance}");

			for (int i = 0; i < playerLineRenderer.positionCount; i++)
			{
				playerDrawnPoints.Add(playerLineRenderer.GetPosition(i));
			}

			// クリア判定
			int pointsInRange = 0;
			foreach (var point in targetPoints)
			{
				foreach (var playerPoint in playerDrawnPoints)
				{
					float calculatedVolt = voltCalculator.CalculateVolt(playerPoint);
					if (Vector2.Distance(new Vector2(playerPoint.x, playerPoint.y), new Vector2(point.x, point.y)) < errorDistance)
					{
						pointsInRange++;
						break;
					}
				}
			}

			Debug.Log($"Player points in range: {pointsInRange} / {targetPoints.Count}");
			if (pointsInRange >= targetPoints.Count * clearRatio) // clearRatio以上の点が一致していたらクリア
			{
				MGManager.ClearGame();
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

			// 電位が許容範囲内かつ、Probeがbounds内にある場合を判定
			if (Mathf.Abs(voltCalculator.volt - targetVolt) < tolerance && bounds.Contains(new Vector3(voltCalculator.transform.position.x, voltCalculator.transform.position.y, 0)))
			{
				isVoltInRange = true;
			}
			else
			{
				isVoltInRange = false;
			}
		}
	}
}