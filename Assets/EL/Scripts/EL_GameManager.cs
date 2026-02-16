using UnityEngine;

namespace EL
{
	public class EL_GameManager : MiniGameBase
	{
		public override void OnGameStart()
		{
			// MGManager.TestPlay(100);
			MGManager.Load();
		}

		void Update()
		{
			// TODO: クリア条件を満たしたらMGManager.Clear()を呼び出す
		}
	}
}