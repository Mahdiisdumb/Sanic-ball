using UnityEngine;

namespace Sanicball.Gameplay
{
	public class BoostPad : MonoBehaviour
	{
		[SerializeField]
		private float speed = 1f;

		[SerializeField]
		private float speedLimit = 200f;

		[SerializeField]
		private LayerMask placementLayers;

		private float offset;

		private void Start()
		{
			PosRot posRot = CalcTargetPlacement();
			base.transform.position = posRot.Position;
			base.transform.rotation = posRot.Rotation;
		}

		private void Update()
		{
			offset -= 5f * Time.deltaTime;
			if (offset <= 0f)
			{
				offset += 1f;
			}
			GetComponent<Renderer>().materials[1].SetTextureOffset("_MainTex", new Vector2(0f, offset));
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			PosRot posRot = CalcTargetPlacement();
			Gizmos.DrawLine(base.transform.position, posRot.Position);
			Gizmos.matrix = Matrix4x4.TRS(posRot.Position, posRot.Rotation, Vector3.one);
			Gizmos.DrawCube(Vector3.zero, new Vector3(5f, 1f, 10f));
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void OnTriggerEnter(Collider other)
		{
			Ball component = other.GetComponent<Ball>();
			if (!(component != null))
			{
				return;
			}
			Rigidbody component2 = other.GetComponent<Rigidbody>();
			if ((bool)component2)
			{
				float magnitude = component2.velocity.magnitude;
				magnitude = Mathf.Min(magnitude + speed, speedLimit);
				component2.velocity = base.transform.rotation * Vector3.forward * magnitude;
				AudioSource component3 = GetComponent<AudioSource>();
				if ((bool)component3)
				{
					component3.Play();
				}
			}
		}

		private PosRot CalcTargetPlacement()
		{
			Ray ray = new Ray(base.transform.position, base.transform.rotation * Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, placementLayers.value))
			{
				PosRot result = new PosRot
				{
					Position = hitInfo.point
				};
				Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
				float y = base.transform.rotation.eulerAngles.y;
				result.Rotation = Quaternion.AngleAxis(y, hitInfo.normal) * quaternion;
				return result;
			}
			return new PosRot(base.transform.position, base.transform.rotation);
		}
	}
}
