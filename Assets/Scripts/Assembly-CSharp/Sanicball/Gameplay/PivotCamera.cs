using Sanicball.Data;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Gameplay
{
	public class PivotCamera : MonoBehaviour, IBallCamera
	{
		[SerializeField]
		private Camera attachedCamera;

		[SerializeField]
		private Vector3 defaultCameraPosition = new Vector3(6f, 2.8f, 0f);

		private float cameraDistance = 1f;

		private float cameraDistanceTarget = 1f;

		[SerializeField]
		private int smoothing = 2;

		[SerializeField]
		public int yMin = -85;

		[SerializeField]
		public int yMax = 85;

		private float xtargetRotation = 90f;

		private float ytargetRotation;

		private float sensitivityMouse = 3f;

		private float sensitivityKeyboard = 10f;

		public Rigidbody Target { get; set; }

		public Camera AttachedCamera
		{
			get
			{
				return attachedCamera;
			}
		}

		public ControlType CtrlType { get; set; }

		public bool UseMouse { get; set; }

		public void SetDirection(Quaternion dir)
		{
			Vector3 vector = dir.eulerAngles + new Vector3(0f, 90f, 0f);
			xtargetRotation = vector.y;
			ytargetRotation = vector.z;
		}

		public void Remove()
		{
			Object.Destroy(base.gameObject);
		}

		private void Start()
		{
			if (UseMouse)
			{
				sensitivityMouse = ActiveData.GameSettings.oldControlsMouseSpeed;
				sensitivityKeyboard = ActiveData.GameSettings.oldControlsKbSpeed;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		private void Update()
		{
			BallControlInput component = Target.GetComponent<BallControlInput>();
			if ((bool)component)
			{
				component.LookDirection = base.transform.rotation * Quaternion.Euler(0f, -90f, 0f);
			}
			if (UseMouse)
			{
				if (Input.GetMouseButtonDown(0) && !GameInput.KeyboardDisabled && !PauseMenu.GamePaused)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
				if (Input.GetKeyDown(KeyCode.LeftAlt))
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
				if (Cursor.lockState == CursorLockMode.Locked)
				{
					float num = Input.GetAxis("Mouse Y") * sensitivityMouse;
					ytargetRotation += 0f - num;
					float num2 = Input.GetAxis("Mouse X") * sensitivityMouse;
					xtargetRotation += num2;
				}
			}
			Vector2 vector = GameInput.CameraVector(CtrlType);
			xtargetRotation += vector.x * 20f * sensitivityKeyboard * Time.deltaTime;
			ytargetRotation -= vector.y * 20f * sensitivityKeyboard * Time.deltaTime;
			ytargetRotation = Mathf.Clamp(ytargetRotation, yMin, yMax);
			xtargetRotation %= 360f;
			ytargetRotation %= 360f;
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(0f, xtargetRotation, ytargetRotation), Time.deltaTime * 10f / (float)smoothing);
		}

		private void LateUpdate()
		{
			if (Target == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			cameraDistanceTarget = Mathf.Clamp(cameraDistanceTarget - Input.GetAxis("Mouse ScrollWheel") * 2f, 0f, 10f);
			cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceTarget, Time.deltaTime * 4f);
			base.transform.position = Target.transform.position;
			Vector3 position = defaultCameraPosition * cameraDistance;
			attachedCamera.transform.position = base.transform.TransformPoint(position);
			AttachedCamera.fieldOfView = Mathf.Lerp(AttachedCamera.fieldOfView, Mathf.Min(60f + Target.velocity.magnitude, 100f), Time.deltaTime * 4f);
		}

		private void OnDestroy()
		{
			if (UseMouse)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}
}
