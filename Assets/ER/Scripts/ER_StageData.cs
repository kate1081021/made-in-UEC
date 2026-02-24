using UnityEngine;
using System.Collections.Generic;

namespace ER
{
	[CreateAssetMenu(fileName = "ER_StageData", menuName = "Scriptable Objects/ER/StageData")]
	public class ER_StageData : ScriptableObject
	{
		public List<Sprite> correctReportList; // 正解レポートのデータリスト
		public List<Sprite> incorrectReportList; // 不正解レポートのデータリスト
		public float correctAnswerRate; // 正解レポートが選ばれる確率（0～1の範囲）
	}
}