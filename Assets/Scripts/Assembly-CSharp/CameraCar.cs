using UnityEngine;

public class CameraCar : MonoBehaviour
{
	public GameObject target;

	public float distance = 10f;

	public float targetHeightOffset;

	public float cameraHeightOffset;

	private Camera cameraComponent;

	private ArcadeCar carComponent;

	public float yaw;

	private Vector3 curPos;

	[Tooltip("Y - Fov (degrees). X - Vehicle speed (km/h)")]
	public AnimationCurve fovCurve = AnimationCurve.Linear(0f, 60f, 120f, 40f);

	private void Start()
	{
		cameraComponent = GetComponent<Camera>();
		if (target != null)
		{
			carComponent = target.GetComponent<ArcadeCar>();
		}
		curPos = base.transform.position;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.F1))
		{
			yaw = 60f;
		}
		if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.F2))
		{
			yaw = -60f;
		}
		if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.F3))
		{
			yaw = 0f;
		}
		if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.F4))
		{
			yaw = 180f;
		}
		Vector3 vector = curPos;
		Vector3 position = target.transform.position;
		position.y = 0f;
		vector.y = 0f;
		Vector3 vector2 = vector - position;
		float magnitude = vector2.magnitude;
		vector2.Normalize();
		Vector3 vector3 = vector;
		if (magnitude > distance)
		{
			vector3 = position + vector2 * distance;
		}
		vector3.y = target.transform.position.y + cameraHeightOffset;
		base.transform.position = vector3;
		Vector3 position2 = target.transform.position;
		position2.y += targetHeightOffset;
		Vector3 forward = position2 - vector3;
		Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
		base.transform.rotation = rotation;
		if (carComponent != null)
		{
			float speed = carComponent.GetSpeed();
			float time = speed * 3.6f;
			float fieldOfView = fovCurve.Evaluate(time);
			cameraComponent.fieldOfView = fieldOfView;
		}
		curPos = base.transform.position;
		base.transform.RotateAround(position2, Vector3.up, yaw);
	}
}
