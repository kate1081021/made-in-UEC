using UnityEngine;

namespace EL
{
	public class EL_VoltCalculator : MiniGameBase
	{
		//本スクリプトの仕様:電極A,Bの座標を取得し,それらに対するプローブとの距離から現在座標の電位を返すというもの.オブジェクト"Probe"にアタッチする想定
		public Transform electroA; //電位0の電極Aの座標
		public Transform electroB; //電位V_0の電極Bの座標
		public float V_0 = 6f; //V_0の値.実験時の値に近いものを採用しているが適宜変更してもらって構わない
		public float volt; //プローブの位置の電位を返す変数
		public override void OnGameStart()
		{

		}
		void Update()
		{
			volt = CalculateVolt(transform.position);
		}

		public float CalculateVolt(Vector2 pos)
		{
			float distanceA = Vector2.Distance(pos, electroA.position); //プローブと電極Aの距離r
			float distanceB = Vector2.Distance(pos, electroB.position); //プローブと電極Bの距離r'
			float D = Vector2.Distance(electroA.position, electroB.position); //電極間距離D

			if (distanceA <= 0.5f) //電極A内部はV=0で一定とするための処理.これは適当な値であり,電極A付近にわずかに0を下回る箇所あり.ぶっちゃけバレない
			{
				return 0;
			}
			else if (distanceB <= 0.5f) //上記同様に電極B内ではV=V_0で一定のため.こちらも,電極B付近にわずかにV_0を上回る箇所があることに注意
			{
				return V_0;
			}
			else
			{
				return V_0 / 2f * Mathf.Log(0.5f / D * distanceB / distanceA, Mathf.Exp(1)) / Mathf.Log(0.5f / D, Mathf.Exp(1));
				//基礎科学実験Aテキストp57より,理論式(4.2)をままで採用.前述の条件式にも登場した0.5という数は,電極の半径をR=0.5としている意を示す.変更可だが統一したい
			}
		}
	}
}