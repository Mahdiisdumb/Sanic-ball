using Sanicball.Gameplay;
using UnityEngine;

namespace Sanicball
{
	public class SpeedFire : MonoBehaviour
	{
		private Ball ball;

		private Rigidbody rb;

		private MeshRenderer mr;

		private AudioSource asrc;

		private float rot;

		public void Init(Ball ball)
		{
			this.ball = ball;
			rb = ball.GetComponent<Rigidbody>();
			base.transform.localScale = ball.transform.localScale;
		}

		private void Start()
		{
			mr = GetComponent<MeshRenderer>();
			asrc = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (!ball)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			float num = Mathf.InverseLerp(120f, 500f, rb.velocity.magnitude);
			num *= num;
			rot += Time.deltaTime * 1000f;
			base.transform.position = ball.transform.position;
			Vector3 vector = rb.velocity;
			if (vector == Vector3.zero)
			{
				vector = Vector3.forward;
			}
			Quaternion quaternion = Quaternion.LookRotation(vector);
			quaternion = Quaternion.AngleAxis(Random.Range(0, 360), rb.velocity) * quaternion;
			base.transform.rotation = quaternion;
			mr.material.color = new Color(1f, 1f, 1f, num);
			asrc.volume = Mathf.Lerp(0f, 0.4f, num);
			asrc.pitch = Mathf.Lerp(1.5f, 7f, num);
		}
	}
}
