using UnityEngine;

namespace Sanicball.Gameplay
{
	public class LobbyPlatform : MonoBehaviour
	{
		public float moveDistance = 2f;

		public float moveTime = 0.5f;

		private float currentPos;

		private Vector3 basePos;

		public void Activate()
		{
			currentPos = 1f;
		}

		private void Start()
		{
			basePos = base.transform.position;
		}

		private void Update()
		{
			if (currentPos > 0f)
			{
				currentPos = Mathf.Max(currentPos - Time.deltaTime / moveTime, 0f);
			}
			float num = Mathf.SmoothStep(0f, 1f, currentPos);
			base.transform.position = basePos + new Vector3(0f, (0f - moveDistance) * num, 0f);
		}
	}
}
