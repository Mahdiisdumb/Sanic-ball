using System.Collections.Generic;
using UnityEngine;

namespace Sanicball
{
	[ExecuteInEditMode]
	public class Water : MonoBehaviour
	{
		public float noiseScaleX = 1f;

		public float noiseScaleY = 1f;

		public float noiseHeight = 1f;

		public float noiseOffsetX;

		public float noiseOffsetY;

		public float triangleSize = 1f;

		public int triangleCount = 64;

		public int noiseIterations = 2;

		public float speed = 1f;

		public float[,] points;

		private float t;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private void Start()
		{
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
		}

		private void Update()
		{
			t += Time.deltaTime * 10f;
			t = Time.time * speed;
			points = new float[triangleCount, triangleCount];
			for (int i = 0; i < points.GetLength(0); i++)
			{
				for (int j = 0; j < points.GetLength(1); j++)
				{
					float num = noiseScaleX;
					float num2 = noiseScaleY;
					float num3 = noiseHeight;
					for (int k = 0; k < noiseIterations; k++)
					{
						points[i, j] += Mathf.PerlinNoise((float)i * num + noiseOffsetX + t, (float)j * num2 + noiseOffsetY) * num3;
						num *= 2f;
						num2 *= 2f;
						num3 *= 0.8f;
					}
				}
			}
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			List<Vector2> list3 = new List<Vector2>();
			int length = points.GetLength(0);
			for (int l = 0; l < length; l++)
			{
				for (int m = 0; m < length; m++)
				{
					list.Add(new Vector3((float)l * triangleSize - (float)(length / 2) * triangleSize, points[l, m], (float)m * triangleSize - (float)(length / 2) * triangleSize));
					list3.Add(new Vector2((float)l / (float)length, (float)m / (float)length));
					if (l < length - 1 && m < length - 1)
					{
						int num4 = l % length + m * length;
						int num5 = num4 + 1;
						int item = num4 + length;
						int item2 = num5 + length;
						list2.Add(num4);
						list2.Add(num5);
						list2.Add(item2);
						list2.Add(num4);
						list2.Add(item2);
						list2.Add(item);
					}
				}
			}
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			mesh.name = "waterMesh";
			mesh.vertices = list.ToArray();
			mesh.triangles = list2.ToArray();
			mesh.uv = list3.ToArray();
			mesh.RecalculateNormals();
			meshFilter.sharedMesh = mesh;
		}
	}
}
