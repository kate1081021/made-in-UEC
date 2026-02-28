using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace ER
{
	public class ER_GameManager : MiniGameBase
	{
		public static ER_GameManager Instance { get; private set; }
		[SerializeField] private ER_ReportController reportController;
		[SerializeField] private SpriteRenderer reportRenderer;
		[SerializeField] private GameObject submitUI;
		[SerializeField] private ER_StageData stageData;
		[SerializeField] private ER_Stamp stamp;
		private Sprite currentReport;
		private bool isCorrectReport;
		private bool isNotClear = false;

		[HideInInspector] public bool isSubmitted = false;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
		}

		public override void OnGameStart()
		{
			// MGManager.TestPlay(100);
			MGManager.Load();
			BGMPlay(false);
			AudioSource audioSource = GetComponent<AudioSource>();
			if (audioSource != null)
			{
				audioSource.loop = true;
			}

			isSubmitted = false;
			submitUI.SetActive(false);

			// ランダムにレポートデータを選択して表示
			float randomValue = Random.Range(0f, 1f);
			if (randomValue <= stageData.correctAnswerRate)
			{
				currentReport = stageData.correctReportList[Random.Range(0, stageData.correctReportList.Count)];
				isCorrectReport = true;
			}
			else
			{
				currentReport = stageData.incorrectReportList[Random.Range(0, stageData.incorrectReportList.Count)];
				isCorrectReport = false;
			}
			reportRenderer.sprite = currentReport;
			isNotClear = false;
		}

		void Update()
		{
			if (reportController.canSubmit)
			{
				submitUI.SetActive(true);
			}
			else
			{
				submitUI.SetActive(false);
			}

			if (isSubmitted && isCorrectReport && !MGManager.IsClear)
			{
				Debug.Log("正解");
				stamp.ShowStamp(true);
				MGManager.ClearGame();
			}
			else if (isSubmitted && !isCorrectReport && !isNotClear)
			{
				Debug.Log("不正解");
				isNotClear = true;
				stamp.ShowStamp(false);
			}
		}

		public override void OnGameEnd()
		{
			if (!isCorrectReport && !isNotClear)
			{
				// 異変があるレポートを提出しなかったらクリア
				MGManager.ClearGame();
			}
		}
	}
}