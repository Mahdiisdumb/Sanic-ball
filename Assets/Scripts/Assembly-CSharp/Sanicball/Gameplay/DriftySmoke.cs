using UnityEngine;

namespace Sanicball.Gameplay
{
	public class DriftySmoke : MonoBehaviour
	{
		public bool grounded;

		public Ball target;

		private ParticleSystem pSystem;

		public AudioSource DriftAudio { get; set; }

		private void Start()
		{
			pSystem = GetComponent<ParticleSystem>();
		}

		private void FixedUpdate()
		{
			if (target == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			Rigidbody component = target.GetComponent<Rigidbody>();
			AudioSource driftAudio = DriftAudio;
			float magnitude = component.velocity.magnitude;
			float num = component.angularVelocity.magnitude / 2f;
			float num2 = Vector3.Angle(component.velocity, Quaternion.Euler(0f, -90f, 0f) * component.angularVelocity);
			if (((num2 > 50f && (num > 10f || magnitude > 10f)) || (num > 30f && magnitude < 30f)) && grounded)
			{
				ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
				{
					position = target.transform.position - new Vector3(0f, 0.5f, 0f) + Random.insideUnitSphere * 0.25f,
					velocity = Vector3.zero,
					startSize = Random.Range(3f, 5f),
					startLifetime = 5f,
					startColor = Color.white
				};
				pSystem.Emit(emitParams, 1);
				if ((bool)driftAudio && driftAudio.volume < 1f)
				{
					driftAudio.volume = Mathf.Min(driftAudio.volume + 0.5f, 1f);
				}
			}
			else if ((bool)driftAudio && driftAudio.volume > 0f)
			{
				driftAudio.volume = Mathf.Max(driftAudio.volume - 0.2f, 0f);
			}
		}

		private float Rand()
		{
			return Random.Range(-1f, 1f);
		}
	}
}
