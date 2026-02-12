using SanicballCore;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[RequireComponent(typeof(Camera))]
	public class OmniCamera : MonoBehaviour, IBallCamera
	{
		[SerializeField]
		private float orbitHeight = 0.5f;

		[SerializeField]
		private float orbitDistance = 4f;

		private Camera attachedCamera;

		private Quaternion currentDirection = Quaternion.Euler(0f, 0f, 0f);

		private Quaternion currentDirectionWithOffset = Quaternion.Euler(0f, 0f, 0f);

		private Vector3 up = Vector3.up;

		public float fovOffset;

		public Rigidbody Target { get; set; }

		public Camera AttachedCamera
		{
			get
			{
				if (!attachedCamera)
				{
					attachedCamera = GetComponent<Camera>();
				}
				return attachedCamera;
			}
		}

		public ControlType CtrlType { get; set; }

		public void SetDirection(Quaternion dir)
		{
			currentDirection = dir;
		}

		public void Remove()
		{
			Object.Destroy(base.gameObject);
		}

		private void Update()
		{
			Quaternion quaternion = Quaternion.identity;
			Vector2 vector = GameInput.CameraVector(CtrlType);
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
			if (vector2 != Vector3.zero)
			{
				Quaternion quaternion2 = Quaternion.Slerp(Quaternion.identity, Quaternion.LookRotation(vector2), vector2.magnitude);
				quaternion = quaternion2;
			}
			if (Target != null)
			{
				Vector3 b = Vector3.up;
				Ball component = Target.GetComponent<Ball>();
				if ((bool)component)
				{
					b = component.Up;
				}
				up = Vector3.Lerp(up, b, Time.deltaTime * 100f);
				Quaternion b2 = ((!(Target.velocity != Vector3.zero)) ? Quaternion.identity : Quaternion.LookRotation(Target.velocity, up));
				Quaternion b3 = Quaternion.Slerp(currentDirection, b2, Mathf.Max(0f, Mathf.Min(-10f + Target.velocity.magnitude, 20f) / 20f));
				currentDirection = Quaternion.Slerp(currentDirection, b3, Time.deltaTime * 4f);
				BallControlInput component2 = Target.GetComponent<BallControlInput>();
				if (component2 != null)
				{
					component2.LookDirection = currentDirection;
				}
				AttachedCamera.fieldOfView = Mathf.Lerp(AttachedCamera.fieldOfView, Mathf.Min(60f + Target.velocity.magnitude, 100f) + fovOffset, Time.deltaTime * 20f);
				currentDirectionWithOffset = Quaternion.Slerp(currentDirectionWithOffset, currentDirection * quaternion, Time.deltaTime * 6f);
				base.transform.position = Target.transform.position + Vector3.up * orbitHeight + currentDirectionWithOffset * (Vector3.back * orbitDistance);
				base.transform.rotation = currentDirectionWithOffset;
			}
		}
	}
}
