using System;
using UnityEngine;
using System.Collections.Generic;

namespace EL
{
	public class Circle
	{
		private float radius;
		private Vector2 center;
		private int segments;

		public Circle(float radius, Vector2 center, int segments)
		{
			this.radius = radius;
			this.center = center;
			this.segments = segments;
		}

		public void Draw(GameObject gameObject, float lineThickness)
		{
			LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
			lineRenderer.startWidth = lineThickness;
			lineRenderer.endWidth = lineThickness;
			lineRenderer.positionCount = segments + 1;

			float deltaTheta = 2f * Mathf.PI / segments;
			float theta = 0f;

			for (int i = 0; i < segments + 1; i++)
			{
				float x = radius * Mathf.Cos(theta);
				float y = radius * Mathf.Sin(theta);
				Vector3 pos = new Vector3(x, y, 0) + new Vector3(center.x, center.y, 0);
				lineRenderer.SetPosition(i, pos);
				theta += deltaTheta;
			}
		}

		// マスク範囲内を切り取るためのDraw関数（欠けた座標をtargetPointsとして取得）
		public void Draw(GameObject parent, float lineThickness, Bounds maskBounds, ref List<Vector3> targetPoints)
		{
			Shader shader = Shader.Find("Sprites/Default");
			if (shader == null) { Debug.LogError("Shaderが見つかりません"); return; }

			float deltaTheta = 2f * Mathf.PI / segments;
			float theta = 0f;

			List<List<Vector3>> segments_list = new List<List<Vector3>>();
			List<Vector3> currentSegment = new List<Vector3>();

			for (int i = 0; i < segments + 1; i++)
			{
				float x = radius * Mathf.Cos(theta);
				float y = radius * Mathf.Sin(theta);
				Vector3 pos = new Vector3(x + center.x, y + center.y, 0);

				if (maskBounds.Contains(pos))
				{
					// 現在のセグメントを区切る
					if (currentSegment.Count >= 2)
						segments_list.Add(currentSegment);
					currentSegment = new List<Vector3>();
					targetPoints.Add(pos);
				}
				else
				{
					currentSegment.Add(pos);
				}

				theta += deltaTheta;
			}
			if (currentSegment.Count >= 2)
				segments_list.Add(currentSegment);

			// セグメントごとにLineRendererを生成
			foreach (var seg in segments_list)
			{
				GameObject child = new GameObject("Segment");
				child.transform.SetParent(parent.transform);

				LineRenderer lr = child.AddComponent<LineRenderer>();
				lr.material = new Material(shader);
				lr.startWidth = lineThickness;
				lr.endWidth = lineThickness;
				lr.positionCount = seg.Count;
				lr.SetPositions(seg.ToArray());
			}
		}
	}

	public class EL_LineDrawer : MiniGameBase
	{
		[SerializeField] private EL_VoltCalculator voltCalculator;
		[SerializeField] private int circleSegmentsFactor = 10; // 円の滑らかさを決めるセグメント数の係数
		[SerializeField] private float lineThickness = 0.05f;

		// 欠けた部分の全座標を保持するリスト
		public List<Vector3> allTargetPoints;

		public override void OnGameStart()
		{
			allTargetPoints = new List<Vector3>();

			Transform electroA = voltCalculator.electroA;
			Transform electroB = voltCalculator.electroB;
			float V_0 = voltCalculator.V_0;
			float D = Vector2.Distance(electroA.position, electroB.position);
			float R = 0.5f; // 電極の半径

			for (float V = 0.5f; V < (int)V_0; V += 0.5f)
			{
				if (V != V_0 / 2f)
				{
					float k = (2f * V / V_0 - 1f) * Mathf.Log(D / R);

					// 中心と半径を計算
					float centerX = (D / 2f) * (1f / MathF.Tanh(k));
					float radius = (D / 2f) / Mathf.Abs(MathF.Sinh(k));

					// 電極AとBの中点を基準にした中心座標
					Vector2 midPoint = (electroA.position + electroB.position) / 2f;
					Vector2 direction = ((Vector2)electroB.position - (Vector2)electroA.position).normalized;
					Vector2 center = midPoint + direction * centerX;
					int circleSegments = Mathf.Max(10, (int)(circleSegmentsFactor * radius)); // 半径に応じてセグメント数を調整

					Circle circle = new Circle(radius, center, circleSegments);
					GameObject circleObj = new GameObject($"Circle_{V}");

					// targetPointsを渡して欠けた部分の座標を収集
					circle.Draw(circleObj, lineThickness, EL_GameManager.Instance.bounds, ref allTargetPoints);
				}
				else
				{
					// V=3の等電位線は特別なケースで、円ではなく直線になる
					GameObject lineObj = new GameObject("Line_V3");
					LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
					lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
					lineRenderer.startWidth = lineThickness;
					lineRenderer.endWidth = lineThickness;
					lineRenderer.positionCount = 2;

					Vector2 direction = ((Vector2)electroB.position - (Vector2)electroA.position).normalized;
					Vector2 normal = new Vector2(-direction.y, direction.x); // 電極間の法線ベクトル

					Vector2 midPoint = (electroA.position + electroB.position) / 2f;

					lineRenderer.SetPosition(0, midPoint + normal * 10f);
					lineRenderer.SetPosition(1, midPoint - normal * 10f);
				}
			}
		}
	}
}