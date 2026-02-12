using UnityEngine;

namespace Sanicball
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleWarping : MonoBehaviour
	{
		public Transform target;

		public Vector3 limit;

		private ParticleSystem ps;

		private void Start()
		{
			ps = GetComponent<ParticleSystem>();
		}

		private void FixedUpdate()
		{
			if (!(Camera.main != null))
			{
				return;
			}
			target = Camera.main.transform;
			Vector3 position = target.position;
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[10000];
			int particles = ps.GetParticles(array);
			for (int i = 0; i < particles; i++)
			{
				Vector3 position2 = array[i].position;
				if (position2.x > position.x + limit.x)
				{
					array[i].position = new Vector3(position2.x - limit.x * 2f, position2.y, position2.z);
				}
				if (position2.x < position.x - limit.x)
				{
					array[i].position = new Vector3(position2.x + limit.x * 2f, position2.y, position2.z);
				}
				if (position2.y > position.y + limit.y)
				{
					array[i].position = new Vector3(position2.x, position2.y - limit.y * 2f, position2.z);
				}
				if (position2.y < position.y - limit.y)
				{
					array[i].position = new Vector3(position2.x, position2.y + limit.y * 2f, position2.z);
				}
				if (position2.z > position.z + limit.z)
				{
					array[i].position = new Vector3(position2.x, position2.y, position2.z - limit.z * 2f);
				}
				if (position2.z < position.z - limit.z)
				{
					array[i].position = new Vector3(position2.x, position2.y, position2.z + limit.z * 2f);
				}
			}
			ps.SetParticles(array, particles);
		}
	}
}
