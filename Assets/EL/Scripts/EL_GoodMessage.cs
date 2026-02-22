using UnityEngine;

public class EL_GoodMessage : MiniGameBase
{
	private RectTransform rt;
	private Animator animator;

	public override void OnGameStart()
	{
		rt = GetComponent<RectTransform>();
		rt.position = new Vector3(-rt.sizeDelta.x, rt.position.y, 0);
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		if (MGManager.IsClear)
		{
			animator.SetBool("IsClear", true);
		}
	}
}
