using Sanicball.Data;
using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
	public class RaceBallSpawner : BallSpawner
	{
		[SerializeField]
		private int editorBallCount = 8;

		[SerializeField]
		private float editorBallSize = 1f;

		[SerializeField]
		private int columns = 10;

		[SerializeField]
		private LayerMask ballSpawningMask = default(LayerMask);

		public Ball SpawnBall(int position, BallType ballType, ControlType ctrlType, int character, string nickname)
		{
			float ballSize = ActiveData.Characters[character].ballSize;
			return SpawnBall(GetSpawnPoint(position, ballSize / 2f), base.transform.rotation, ballType, ctrlType, character, nickname);
		}

		public Vector3 GetSpawnPoint(int position, float offsetY)
		{
			int num = position / columns;
			Vector3 vector = ((position % 2 != 0) ? (Vector3.left * ((float)(position % columns / 2) + 0.5f) * 2f) : (Vector3.right * ((float)(position % columns / 2) + 0.5f) * 2f));
			vector += Vector3.back * 2f * num;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.TransformPoint(vector + Vector3.up * 100f), Vector3.down, out hitInfo, 200f, ballSpawningMask))
			{
				vector = base.transform.InverseTransformPoint(hitInfo.point);
			}
			return base.transform.TransformPoint(vector) + Vector3.up * offsetY;
		}

		private void Start()
		{
			GetComponent<Renderer>().enabled = false;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(0.2f, 0.6f, 1f);
			columns = Mathf.Max(1, columns);
			for (int i = 0; i < editorBallCount; i++)
			{
				Gizmos.DrawSphere(GetSpawnPoint(i, editorBallSize / 2f), editorBallSize / 2f);
			}
		}
	}
}
