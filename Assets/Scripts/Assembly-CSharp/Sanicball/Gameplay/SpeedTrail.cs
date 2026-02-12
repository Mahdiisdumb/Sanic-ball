using Sanicball.Data;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(TrailRenderer))]
	public class SpeedTrail : MonoBehaviour
	{
		private TrailRenderer tr;

		private void Start()
		{
			tr = GetComponent<TrailRenderer>();
			tr.enabled = ActiveData.GameSettings.trails;
		}

		private void Update()
		{
			if (tr.enabled)
			{
				float num = Mathf.Max(0f, GetComponent<Rigidbody>().velocity.magnitude - 60f);
				tr.time = Mathf.Clamp(num / 20f, 0f, 5f);
				tr.startWidth = Mathf.Clamp(num / 80f, 0f, 0.8f);
				tr.material.mainTextureScale = new Vector2(tr.time * 100f, 1f);
				tr.material.mainTextureOffset = new Vector2((tr.material.mainTextureOffset.x - 2f * Time.deltaTime) % 1f, 0f);
			}
		}
	}
}
