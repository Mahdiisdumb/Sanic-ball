using UnityEngine;

namespace Sanicball.Gameplay
{
	public class AITarget : MonoBehaviour
	{
		public float stupidness = 100f;

		private Vector2 pos;

		private Vector2 velocity;

		private Vector2 target;

		private int timer;

		public Vector3 GetPos()
		{
			return base.transform.position + new Vector3(pos.x, 0f, pos.y);
		}

		private void Start()
		{
			pos = Vector2.zero;
			target = Random.insideUnitCircle * stupidness;
			timer = Random.Range(50, 200);
		}

		private void FixedUpdate()
		{
			pos = Vector2.Lerp(pos, target, 0.01f);
			timer--;
			if (timer <= 0)
			{
				target = Random.insideUnitCircle * stupidness;
				timer = Random.Range(50, 200);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position + new Vector3(pos.x, 0f, pos.y), 2f);
		}
	}
}
