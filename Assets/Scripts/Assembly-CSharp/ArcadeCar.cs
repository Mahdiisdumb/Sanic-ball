using System;
using UnityEngine;

public class ArcadeCar : MonoBehaviour
{
	public class WheelData
	{
		[HideInInspector]
		public bool isOnGround;

		[HideInInspector]
		public RaycastHit touchPoint = default(RaycastHit);

		[HideInInspector]
		public float yawRad;

		[HideInInspector]
		public float visualRotationRad;

		[HideInInspector]
		public float compression;

		[HideInInspector]
		public float compressionPrev;

		[HideInInspector]
		public string debugText = "-";
	}

	[Serializable]
	public class Axle
	{
		[Header("Debug settings")]
		[Tooltip("Debug name of axle")]
		public string debugName;

		[Tooltip("Debug color for axle")]
		public Color debugColor = Color.white;

		[Header("Axle settings")]
		[Tooltip("Axle width")]
		public float width = 0.4f;

		[Tooltip("Axle offset")]
		public Vector2 offset = Vector2.zero;

		[Tooltip("Current steering angle (in degrees)")]
		public float steerAngle;

		[Header("Wheel settings")]
		[Tooltip("Wheel radius in meters")]
		public float radius = 0.3f;

		[Range(0f, 1f)]
		[Tooltip("Tire laterial friction normalized to 0..1")]
		public float laterialFriction = 0.1f;

		[Range(0f, 1f)]
		[Tooltip("Rolling friction, normalized to 0..1")]
		public float rollingFriction = 0.01f;

		[HideInInspector]
		[Tooltip("Brake left")]
		public bool brakeLeft;

		[HideInInspector]
		[Tooltip("Brake right")]
		public bool brakeRight;

		[HideInInspector]
		[Tooltip("Hand brake left")]
		public bool handBrakeLeft;

		[HideInInspector]
		[Tooltip("Hand brake right")]
		public bool handBrakeRight;

		[Tooltip("Brake force magnitude")]
		public float brakeForceMag = 4f;

		[Header("Suspension settings")]
		[Tooltip("Suspension Stiffness (Suspension 'Power'")]
		public float stiffness = 8500f;

		[Tooltip("Suspension Damping (Suspension 'Bounce')")]
		public float damping = 3000f;

		[Tooltip("Suspension Restitution (Not used now)")]
		public float restitution = 1f;

		[Tooltip("Relaxed suspension length")]
		public float lengthRelaxed = 0.55f;

		[Tooltip("Stabilizer bar anti-roll force")]
		public float antiRollForce = 10000f;

		[HideInInspector]
		public WheelData wheelDataL = new WheelData();

		[HideInInspector]
		public WheelData wheelDataR = new WheelData();

		[Header("Visual settings")]
		[Tooltip("Visual scale for wheels")]
		public float visualScale = 0.03270531f;

		[Tooltip("Wheel actor left")]
		public GameObject wheelVisualLeft;

		[Tooltip("Wheel actor right")]
		public GameObject wheelVisualRight;

		[Tooltip("Is axle powered by engine")]
		public bool isPowered;

		[Tooltip("After flight slippery coefficent (0 - no friction)")]
		public float afterFlightSlipperyK = 0.02f;

		[Tooltip("Brake slippery coefficent (0 - no friction)")]
		public float brakeSlipperyK = 0.5f;

		[Tooltip("Hand brake slippery coefficent (0 - no friction)")]
		public float handBrakeSlipperyK = 0.01f;
	}

	private const int WHEEL_LEFT_INDEX = 0;

	private const int WHEEL_RIGHT_INDEX = 1;

	private const float wheelWidth = 0.085f;

	public Vector3 centerOfMass = Vector3.zero;

	[Header("Engine")]
	[Tooltip("Y - Desired vehicle speed (km/h). X - Time (seconds)")]
	public AnimationCurve accelerationCurve = AnimationCurve.Linear(0f, 0f, 5f, 100f);

	[Tooltip("Y - Desired vehicle speed (km/h). X - Time (seconds)")]
	public AnimationCurve accelerationCurveReverse = AnimationCurve.Linear(0f, 0f, 5f, 20f);

	[Tooltip("Number of times to iterate reverse evaluation of Acceleration Curve. May need to increase with higher max vehicle speed. ")]
	public int reverseEvaluationAccuracy = 25;

	[Header("Steering")]
	[Tooltip("Y - Steereing angle limit (deg). X - Vehicle speed (km/h)")]
	public AnimationCurve steerAngleLimit = AnimationCurve.Linear(0f, 35f, 100f, 5f);

	[Tooltip("Y - Steereing reset speed (deg/sec). X - Vehicle speed (km/h)")]
	public AnimationCurve steeringResetSpeed = AnimationCurve.EaseInOut(0f, 30f, 100f, 10f);

	[Tooltip("Y - Steereing speed (deg/sec). X - Vehicle speed (km/h)")]
	public AnimationCurve steeringSpeed = AnimationCurve.Linear(0f, 2f, 100f, 0.5f);

	[Header("Debug")]
	public bool debugDraw = true;

	[Header("Other")]
	[Tooltip("Stabilization in flight (torque)")]
	public float flightStabilizationForce = 8f;

	[Tooltip("Stabilization in flight (Ang velocity damping)")]
	public float flightStabilizationDamping;

	[Tooltip("Hand brake slippery time in seconds")]
	public float handBrakeSlipperyTime = 2.2f;

	public bool controllable = true;

	[Tooltip("Y - Downforce (percentage 0%..100%). X - Vehicle speed (km/h)")]
	public AnimationCurve downForceCurve = AnimationCurve.Linear(0f, 0f, 200f, 100f);

	[Tooltip("Downforce")]
	public float downForce = 5f;

	[Header("Axles")]
	public Axle[] axles = new Axle[2];

	private float afterFlightSlipperyTiresTime;

	private float brakeSlipperyTiresTime;

	private float handBrakeSlipperyTiresTime;

	private bool isBrake;

	private bool isHandBrake;

	private bool isAcceleration;

	private bool isReverseAcceleration;

	private float accelerationForceMagnitude;

	private Rigidbody rb;

	private static GUIStyle style = new GUIStyle();

	private Ray wheelRay = default(Ray);

	private RaycastHit[] wheelRayHits = new RaycastHit[16];

	private void Reset(Vector3 position)
	{
		position += new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		float num = base.transform.eulerAngles.y + UnityEngine.Random.Range(-10f, 10f);
		base.transform.position = position;
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, num, 0f));
		rb.velocity = new Vector3(0f, 0f, 0f);
		rb.angularVelocity = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < axles.Length; i++)
		{
			axles[i].steerAngle = 0f;
		}
		Debug.Log(string.Format("Reset {0}, {1}, {2}, Rot {3}", position.x, position.y, position.z, num));
	}

	private void Start()
	{
		style.normal.textColor = Color.red;
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMass;
	}

	private void OnValidate()
	{
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		ApplyVisual();
		CalculateAckermannSteering();
	}

	private float GetHandBrakeK()
	{
		float num = handBrakeSlipperyTiresTime / Math.Max(0.1f, handBrakeSlipperyTime);
		return num * num * num * (num * (num * 6f - 15f) + 10f);
	}

	private float GetSteeringHandBrakeK()
	{
		return Mathf.Clamp01(0.4f + (1f - GetHandBrakeK()) * 0.6f);
	}

	private float GetAccelerationForceMagnitude(AnimationCurve accCurve, float speedMetersPerSec, float dt)
	{
		float num = speedMetersPerSec * 3.6f;
		float mass = rb.mass;
		int length = accCurve.length;
		switch (length)
		{
		case 0:
			return 0f;
		case 1:
		{
			float value2 = accCurve.keys[0].value;
			float num12 = value2 - num;
			num12 /= 3.6f;
			float a3 = num12 * mass;
			return Mathf.Max(a3, 0f);
		}
		default:
		{
			float time = accCurve.keys[0].time;
			float time2 = accCurve.keys[length - 1].time;
			float num2 = time2 - time;
			float num3 = time;
			bool flag = false;
			if (num < accCurve.keys[length - 1].value)
			{
				for (int i = 0; i < reverseEvaluationAccuracy; i++)
				{
					float num4 = accCurve.Evaluate(num3);
					float num5 = Math.Abs(num - num4);
					float num6 = num3 + num2;
					float num7 = accCurve.Evaluate(num6);
					float num8 = Math.Abs(num - num7);
					if (num8 < num5)
					{
						num3 = num6;
						num4 = num7;
					}
					num2 = Math.Abs(num2 / 2f) * Mathf.Sign(num - num4);
				}
				flag = true;
			}
			if (flag)
			{
				float num9 = accCurve.Evaluate(num3 + dt);
				float num10 = num9 - num;
				num10 /= 3.6f;
				float a = num10 * mass;
				return Mathf.Max(a, 0f);
			}
			if (debugDraw)
			{
				Debug.Log("Max speed reached!");
			}
			float value = accCurve.keys[length - 1].value;
			float num11 = value - num;
			num11 /= 3.6f;
			float a2 = num11 * mass;
			return Mathf.Max(a2, 0f);
		}
		}
	}

	public float GetSpeed()
	{
		Vector3 velocity = rb.velocity;
		Vector3 vector = rb.transform.rotation * Vector3.forward;
		float num = Vector3.Dot(velocity, vector);
		return (num * vector).magnitude * Mathf.Sign(num);
	}

	private float CalcAccelerationForceMagnitude()
	{
		if (!isAcceleration && !isReverseAcceleration)
		{
			return 0f;
		}
		float speed = GetSpeed();
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (isAcceleration)
		{
			return GetAccelerationForceMagnitude(accelerationCurve, speed, fixedDeltaTime);
		}
		float num = GetAccelerationForceMagnitude(accelerationCurveReverse, 0f - speed, fixedDeltaTime);
		return 0f - num;
	}

	private float GetSteerAngleLimitInDeg(float speedMetersPerSec)
	{
		float num = speedMetersPerSec * 3.6f;
		num *= GetSteeringHandBrakeK();
		return steerAngleLimit.Evaluate(num);
	}

	private void UpdateInput()
	{
		float num = Input.GetAxis("Vertical");
		float num2 = Input.GetAxis("Horizontal");
		if (!controllable)
		{
			num = 0f;
			num2 = 0f;
		}
		if (Input.GetKey(KeyCode.R) && controllable)
		{
			Debug.Log("Reset pressed");
			Ray ray = new Ray
			{
				origin = base.transform.position + new Vector3(0f, 100f, 0f),
				direction = new Vector3(0f, -1f, 0f)
			};
			RaycastHit[] array = new RaycastHit[16];
			int num3 = Physics.RaycastNonAlloc(ray, array, 250f);
			if (num3 > 0)
			{
				float num4 = float.MaxValue;
				for (int i = 0; i < num3; i++)
				{
					if ((!(array[i].collider != null) || !array[i].collider.isTrigger) && !(array[i].rigidbody == rb) && array[i].distance < num4)
					{
						num4 = array[i].distance;
					}
				}
				num4 -= 4f;
				Vector3 position = ray.origin + ray.direction * num4;
				Reset(position);
			}
			else
			{
				Reset(new Vector3(-69.48f, 5.25f, 132.71f));
			}
		}
		bool flag = false;
		bool flag2 = Input.GetKey(KeyCode.Space) && controllable;
		float speed = GetSpeed();
		isAcceleration = false;
		isReverseAcceleration = false;
		if (num > 0.4f)
		{
			if (speed < -0.5f)
			{
				flag = true;
			}
			else
			{
				isAcceleration = true;
			}
		}
		else if (num < -0.4f)
		{
			if (speed > 0.5f)
			{
				flag = true;
			}
			else
			{
				isReverseAcceleration = true;
			}
		}
		if (flag && !isBrake)
		{
			brakeSlipperyTiresTime = 1f;
		}
		if (flag2)
		{
			handBrakeSlipperyTiresTime = Math.Max(0.1f, handBrakeSlipperyTime);
		}
		isBrake = flag;
		isHandBrake = flag2 && !isAcceleration && !isReverseAcceleration;
		axles[0].brakeLeft = isBrake;
		axles[0].brakeRight = isBrake;
		axles[1].brakeLeft = isBrake;
		axles[1].brakeRight = isBrake;
		axles[0].handBrakeLeft = isHandBrake;
		axles[0].handBrakeRight = isHandBrake;
		axles[1].handBrakeLeft = isHandBrake;
		axles[1].handBrakeRight = isHandBrake;
		if (Mathf.Abs(num2) > 0.001f)
		{
			float num5 = Mathf.Abs(speed) * 3.6f;
			num5 *= GetSteeringHandBrakeK();
			float num6 = steeringSpeed.Evaluate(num5);
			float num7 = axles[0].steerAngle + num2 * num6;
			float num8 = Mathf.Sign(num7);
			float steerAngleLimitInDeg = GetSteerAngleLimitInDeg(speed);
			num7 = Mathf.Min(Math.Abs(num7), steerAngleLimitInDeg) * num8;
			axles[0].steerAngle = num7;
		}
		else
		{
			float num9 = Mathf.Abs(speed) * 3.6f;
			float b = steeringResetSpeed.Evaluate(num9);
			b = Mathf.Lerp(0f, b, Mathf.Clamp01(num9 / 2f));
			float steerAngle = axles[0].steerAngle;
			float num10 = Mathf.Sign(steerAngle);
			steerAngle = Mathf.Abs(steerAngle);
			steerAngle -= b * Time.fixedDeltaTime;
			steerAngle = Mathf.Max(steerAngle, 0f) * num10;
			axles[0].steerAngle = steerAngle;
		}
	}

	private void Update()
	{
		ApplyVisual();
	}

	private void FixedUpdate()
	{
		UpdateInput();
		accelerationForceMagnitude = CalcAccelerationForceMagnitude();
		float num = Mathf.Clamp01(0.8f + (1f - GetHandBrakeK()) * 0.2f);
		accelerationForceMagnitude *= num;
		CalculateAckermannSteering();
		int num2 = 0;
		for (int i = 0; i < axles.Length; i++)
		{
			if (axles[i].isPowered)
			{
				num2 += 2;
			}
		}
		int totalWheelsCount = axles.Length * 2;
		for (int j = 0; j < axles.Length; j++)
		{
			CalculateAxleForces(axles[j], totalWheelsCount, num2);
		}
		bool flag = true;
		for (int k = 0; k < axles.Length; k++)
		{
			if (axles[k].wheelDataL.isOnGround || axles[k].wheelDataR.isOnGround)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			afterFlightSlipperyTiresTime = 1f;
			Vector3 lhs = base.transform.TransformDirection(new Vector3(0f, 1f, 0f));
			Vector3 rhs = new Vector3(0f, 1f, 0f);
			Vector3 vector = Vector3.Cross(lhs, rhs);
			float mass = rb.mass;
			Vector3 angularVelocity = rb.angularVelocity;
			Vector3 vector2 = angularVelocity;
			vector2.y = 0f;
			vector2 *= Mathf.Clamp01(flightStabilizationDamping * Time.fixedDeltaTime);
			rb.angularVelocity = angularVelocity - vector2;
			rb.AddTorque(vector * flightStabilizationForce * mass);
		}
		else
		{
			Vector3 vector3 = base.transform.TransformDirection(new Vector3(0f, -1f, 0f));
			float speed = GetSpeed();
			float time = Mathf.Abs(speed) * 3.6f;
			float num3 = downForceCurve.Evaluate(time) / 100f;
			float mass2 = rb.mass;
			rb.AddForce(vector3 * mass2 * num3 * downForce);
		}
		if (afterFlightSlipperyTiresTime > 0f)
		{
			afterFlightSlipperyTiresTime -= Time.fixedDeltaTime;
		}
		else
		{
			afterFlightSlipperyTiresTime = 0f;
		}
		if (brakeSlipperyTiresTime > 0f)
		{
			brakeSlipperyTiresTime -= Time.fixedDeltaTime;
		}
		else
		{
			brakeSlipperyTiresTime = 0f;
		}
		if (handBrakeSlipperyTiresTime > 0f)
		{
			handBrakeSlipperyTiresTime -= Time.fixedDeltaTime;
		}
		else
		{
			handBrakeSlipperyTiresTime = 0f;
		}
	}

	private void OnGUI()
	{
		if (!controllable)
		{
			return;
		}
		float speed = GetSpeed();
		float num = speed * 3.6f;
		GUI.Label(new Rect(30f, 20f, 150f, 130f), string.Format("{0:F2} km/h", num), style);
		GUI.Label(new Rect(30f, 40f, 150f, 130f), string.Format("{0:F2} {1:F2} {2:F2}", afterFlightSlipperyTiresTime, brakeSlipperyTiresTime, handBrakeSlipperyTiresTime), style);
		float num2 = 60f;
		for (int i = 0; i < axles.Length; i++)
		{
			GUI.Label(new Rect(30f, num2, 150f, 130f), string.Format("Axle {0}, steering angle {1:F2}", i, axles[i].steerAngle), style);
			num2 += 18f;
		}
		Camera current = Camera.current;
		if (current == null || !debugDraw)
		{
			return;
		}
		Axle[] array = axles;
		foreach (Axle axle in array)
		{
			Vector3 position = new Vector3(axle.width * -0.5f, axle.offset.y, axle.offset.x);
			Vector3 position2 = new Vector3(axle.width * 0.5f, axle.offset.y, axle.offset.x);
			Vector3 vector = base.transform.TransformPoint(position);
			Vector3 vector2 = base.transform.TransformPoint(position2);
			for (int k = 0; k < 2; k++)
			{
				WheelData wheelData = ((k != 0) ? axle.wheelDataR : axle.wheelDataL);
				Vector3 position3 = ((k != 0) ? vector2 : vector);
				Vector3 vector3 = current.WorldToScreenPoint(position3);
				GUI.Label(new Rect(vector3.x, (float)Screen.height - vector3.y, 150f, 130f), wheelData.debugText, style);
			}
		}
	}

	private void AddForceAtPosition(Vector3 force, Vector3 position)
	{
		rb.AddForceAtPosition(force, position);
	}

	private bool RayCast(Ray ray, float maxDistance, ref RaycastHit nearestHit)
	{
		int num = Physics.RaycastNonAlloc(wheelRay, wheelRayHits, maxDistance);
		if (num == 0)
		{
			return false;
		}
		nearestHit.distance = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			if ((!(wheelRayHits[i].collider != null) || !wheelRayHits[i].collider.isTrigger) && !(wheelRayHits[i].rigidbody == rb) && !(Vector3.Dot(wheelRayHits[i].normal, new Vector3(0f, 1f, 0f)) < 0.6f) && wheelRayHits[i].distance < nearestHit.distance)
			{
				nearestHit = wheelRayHits[i];
			}
		}
		return nearestHit.distance <= maxDistance;
	}

	private void CalculateWheelForces(Axle axle, Vector3 wsDownDirection, WheelData wheelData, Vector3 wsAttachPoint, int wheelIndex, int totalWheelsCount, int numberOfPoweredWheels)
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		wheelData.debugText = "--";
		Quaternion quaternion = Quaternion.Euler(new Vector3(0f, wheelData.yawRad * 57.29578f, 0f));
		Quaternion quaternion2 = base.transform.rotation * quaternion;
		Vector3 vector = quaternion2 * Vector3.left;
		wheelData.isOnGround = false;
		wheelRay.direction = wsDownDirection;
		float maxDistance = axle.lengthRelaxed + axle.radius;
		wheelRay.origin = wsAttachPoint + vector * 0.085f;
		RaycastHit nearestHit = default(RaycastHit);
		bool flag = RayCast(wheelRay, maxDistance, ref nearestHit);
		wheelRay.origin = wsAttachPoint - vector * 0.085f;
		RaycastHit nearestHit2 = default(RaycastHit);
		bool flag2 = RayCast(wheelRay, maxDistance, ref nearestHit2);
		wheelRay.origin = wsAttachPoint;
		if (!RayCast(wheelRay, maxDistance, ref wheelData.touchPoint) || !flag || !flag2)
		{
			float num = 1f;
			wheelData.compressionPrev = wheelData.compression;
			wheelData.compression = Mathf.Clamp01(wheelData.compression - fixedDeltaTime * num);
			return;
		}
		float num2 = wheelData.touchPoint.distance - axle.radius;
		wheelData.isOnGround = true;
		float num3 = 0f;
		wheelData.compression = 1f - Mathf.Clamp01(num2 / axle.lengthRelaxed);
		wheelData.debugText = wheelData.compression.ToString("F2");
		float num4 = wheelData.compression * (0f - axle.stiffness);
		num3 += num4;
		float num5 = (wheelData.compression - wheelData.compressionPrev) / fixedDeltaTime;
		wheelData.compressionPrev = wheelData.compression;
		float num6 = (0f - num5) * axle.damping;
		num3 += num6;
		float num7 = Vector3.Dot(wheelData.touchPoint.normal, -wsDownDirection);
		num3 *= num7;
		Vector3 force = wsDownDirection * num3;
		AddForceAtPosition(force, wheelData.touchPoint.point);
		Vector3 pointVelocity = rb.GetPointVelocity(wheelData.touchPoint.point);
		Vector3 normal = wheelData.touchPoint.normal;
		Vector3 normalized = (nearestHit.point - nearestHit2.point).normalized;
		Vector3 vector2 = Vector3.Cross(normal, normalized);
		Vector3 vector3 = Vector3.Dot(pointVelocity, normalized) * normalized;
		Vector3 vector4 = Vector3.Dot(pointVelocity, vector2) * vector2;
		Vector3 vector5 = (vector3 + vector4) * 0.5f;
		Vector3 vector6 = vector5 * rb.mass / fixedDeltaTime / totalWheelsCount;
		if (debugDraw)
		{
			Debug.DrawRay(wheelData.touchPoint.point, vector5, Color.red);
		}
		float num8 = Mathf.Clamp01(axle.laterialFriction);
		float num9 = 1f;
		if (afterFlightSlipperyTiresTime > 0f)
		{
			float b = Mathf.Lerp(1f, axle.afterFlightSlipperyK, Mathf.Clamp01(afterFlightSlipperyTiresTime));
			num9 = Mathf.Min(num9, b);
		}
		if (brakeSlipperyTiresTime > 0f)
		{
			float b2 = Mathf.Lerp(1f, axle.brakeSlipperyK, Mathf.Clamp01(brakeSlipperyTiresTime));
			num9 = Mathf.Min(num9, b2);
		}
		float handBrakeK = GetHandBrakeK();
		if (handBrakeK > 0f)
		{
			float b3 = Mathf.Lerp(1f, axle.handBrakeSlipperyK, handBrakeK);
			num9 = Mathf.Min(num9, b3);
		}
		num8 *= num9;
		Vector3 vector7 = -vector6 * num8;
		Vector3 vector8 = Vector3.Dot(vector7, vector2) * vector2;
		bool flag3 = ((wheelIndex != 0) ? axle.brakeRight : axle.brakeLeft);
		bool flag4 = ((wheelIndex != 0) ? axle.handBrakeRight : axle.handBrakeLeft);
		if (flag3 || flag4)
		{
			float num10 = Mathf.Clamp(axle.brakeForceMag * rb.mass, 0f, vector8.magnitude);
			Vector3 vector9 = vector8.normalized * num10;
			if (flag4)
			{
				vector9 *= 0.8f;
			}
			vector8 -= vector9;
		}
		else if (!isAcceleration && !isReverseAcceleration)
		{
			float num11 = 1f - Mathf.Clamp01(axle.rollingFriction);
			vector8 *= num11;
		}
		vector7 -= vector8;
		if (debugDraw)
		{
			Debug.DrawRay(wheelData.touchPoint.point, vector7, Color.red);
			Debug.DrawRay(wheelData.touchPoint.point, vector8, Color.white);
		}
		AddForceAtPosition(vector7, wheelData.touchPoint.point);
		if (!isBrake && axle.isPowered && Mathf.Abs(accelerationForceMagnitude) > 0.01f)
		{
			Vector3 vector10 = wheelData.touchPoint.point - wsDownDirection * 0.2f;
			Vector3 vector11 = vector2 * accelerationForceMagnitude / numberOfPoweredWheels / fixedDeltaTime;
			AddForceAtPosition(vector11, vector10);
			if (debugDraw)
			{
				Debug.DrawRay(vector10, vector11, Color.green);
			}
		}
	}

	private void CalculateAxleForces(Axle axle, int totalWheelsCount, int numberOfPoweredWheels)
	{
		Vector3 vector = base.transform.TransformDirection(Vector3.down);
		vector.Normalize();
		Vector3 position = new Vector3(axle.width * -0.5f, axle.offset.y, axle.offset.x);
		Vector3 position2 = new Vector3(axle.width * 0.5f, axle.offset.y, axle.offset.x);
		Vector3 vector2 = base.transform.TransformPoint(position);
		Vector3 vector3 = base.transform.TransformPoint(position2);
		for (int i = 0; i < 2; i++)
		{
			WheelData wheelData = ((i != 0) ? axle.wheelDataR : axle.wheelDataL);
			Vector3 wsAttachPoint = ((i != 0) ? vector3 : vector2);
			CalculateWheelForces(axle, vector, wheelData, wsAttachPoint, i, totalWheelsCount, numberOfPoweredWheels);
		}
		float num = 1f - Mathf.Clamp01(axle.wheelDataL.compression);
		float num2 = 1f - Mathf.Clamp01(axle.wheelDataR.compression);
		float num3 = (num - num2) * axle.antiRollForce;
		if (axle.wheelDataL.isOnGround)
		{
			AddForceAtPosition(vector * num3, axle.wheelDataL.touchPoint.point);
			if (debugDraw)
			{
				Debug.DrawRay(axle.wheelDataL.touchPoint.point, vector * num3, Color.magenta);
			}
		}
		if (axle.wheelDataR.isOnGround)
		{
			AddForceAtPosition(vector * (0f - num3), axle.wheelDataR.touchPoint.point);
			if (debugDraw)
			{
				Debug.DrawRay(axle.wheelDataR.touchPoint.point, vector * (0f - num3), Color.magenta);
			}
		}
	}

	private void CalculateAckermannSteering()
	{
		for (int i = 0; i < axles.Length; i++)
		{
			float yawRad = axles[i].steerAngle * ((float)Math.PI / 180f);
			axles[i].wheelDataL.yawRad = yawRad;
			axles[i].wheelDataR.yawRad = yawRad;
		}
		if (axles.Length != 2)
		{
			Debug.LogWarning("Ackermann work only for 2 axle vehicles.");
			return;
		}
		Axle axle = axles[0];
		Axle axle2 = axles[1];
		if (Mathf.Abs(axle2.steerAngle) > 0.0001f)
		{
			Debug.LogWarning("Ackermann work only for vehicles with forward steering axle.");
			return;
		}
		float magnitude = (base.transform.TransformPoint(new Vector3(0f, axle.offset.y, axle.offset.x)) - base.transform.TransformPoint(new Vector3(0f, axle2.offset.y, axle2.offset.x))).magnitude;
		float magnitude2 = (base.transform.TransformPoint(new Vector3(axle.width * -0.5f, axle.offset.y, axle.offset.x)) - base.transform.TransformPoint(new Vector3(axle.width * 0.5f, axle.offset.y, axle.offset.x))).magnitude;
		float num = magnitude / Mathf.Tan(axle.steerAngle * ((float)Math.PI / 180f));
		float yawRad2 = Mathf.Atan(magnitude / (num + magnitude2 / 2f));
		float yawRad3 = Mathf.Atan(magnitude / (num - magnitude2 / 2f));
		axle.wheelDataL.yawRad = yawRad2;
		axle.wheelDataR.yawRad = yawRad3;
	}

	private void CalculateWheelVisualTransform(Vector3 wsAttachPoint, Vector3 wsDownDirection, Axle axle, WheelData data, int wheelIndex, float visualRotationRad, out Vector3 pos, out Quaternion rot)
	{
		float num = Mathf.Clamp01(1f - data.compression) * axle.lengthRelaxed;
		pos = wsAttachPoint + wsDownDirection * num;
		float num2 = 0f;
		float num3 = 57.29578f;
		if (wheelIndex == 0)
		{
			num2 = 180f;
			num3 = -57.29578f;
		}
		Quaternion quaternion = Quaternion.Euler(new Vector3(data.visualRotationRad * num3, num2 + data.yawRad * 57.29578f, 0f));
		rot = base.transform.rotation * quaternion;
	}

	private void CalculateWheelRotationFromSpeed(Axle axle, WheelData data, Vector3 wsPos)
	{
		if (rb == null)
		{
			data.visualRotationRad = 0f;
			return;
		}
		Quaternion quaternion = Quaternion.Euler(new Vector3(0f, data.yawRad * 57.29578f, 0f));
		Quaternion quaternion2 = base.transform.rotation * quaternion;
		Vector3 rhs = quaternion2 * Vector3.forward;
		Vector3 worldPoint = ((!data.isOnGround) ? wsPos : data.touchPoint.point);
		Vector3 pointVelocity = rb.GetPointVelocity(worldPoint);
		float num = Vector3.Dot(pointVelocity, rhs);
		float num2 = (float)Math.PI * 2f * axle.radius;
		float num3 = num / num2;
		float num4 = (float)Math.PI * 2f * num3 * Time.deltaTime;
		data.visualRotationRad += num4;
	}

	private void ApplyVisual()
	{
		Vector3 wsDownDirection = base.transform.TransformDirection(Vector3.down);
		wsDownDirection.Normalize();
		for (int i = 0; i < axles.Length; i++)
		{
			Axle axle = axles[i];
			Vector3 position = new Vector3(axle.width * -0.5f, axle.offset.y, axle.offset.x);
			Vector3 position2 = new Vector3(axle.width * 0.5f, axle.offset.y, axle.offset.x);
			Vector3 wsAttachPoint = base.transform.TransformPoint(position);
			Vector3 wsAttachPoint2 = base.transform.TransformPoint(position2);
			Vector3 pos;
			Quaternion rot;
			if (axle.wheelVisualLeft != null)
			{
				CalculateWheelVisualTransform(wsAttachPoint, wsDownDirection, axle, axle.wheelDataL, 0, axle.wheelDataL.visualRotationRad, out pos, out rot);
				axle.wheelVisualLeft.transform.position = pos;
				axle.wheelVisualLeft.transform.rotation = rot;
				axle.wheelVisualLeft.transform.localScale = new Vector3(axle.radius, axle.radius, axle.radius) * axle.visualScale;
				if (!isBrake)
				{
					CalculateWheelRotationFromSpeed(axle, axle.wheelDataL, pos);
				}
			}
			if (axle.wheelVisualRight != null)
			{
				CalculateWheelVisualTransform(wsAttachPoint2, wsDownDirection, axle, axle.wheelDataR, 1, axle.wheelDataR.visualRotationRad, out pos, out rot);
				axle.wheelVisualRight.transform.position = pos;
				axle.wheelVisualRight.transform.rotation = rot;
				axle.wheelVisualRight.transform.localScale = new Vector3(axle.radius, axle.radius, axle.radius) * axle.visualScale;
				if (!isBrake)
				{
					CalculateWheelRotationFromSpeed(axle, axle.wheelDataR, pos);
				}
			}
		}
	}
}
