using System.Collections.Generic;
using UnityEngine;

namespace EL
{
	[System.Serializable]
	public class ClearCondition
	{
		public float targetVolt; // 目標電位
		public float tolerance; // 許容範囲
		public Bounds bounds; // 電圧値を探す範囲
	}

	[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/EL/Stage Data")]
	public class EL_StageData : ScriptableObject
	{
		public List<ClearCondition> clearConditions;
	}
}