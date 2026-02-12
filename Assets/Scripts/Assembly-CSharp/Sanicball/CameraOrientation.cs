using UnityEngine;

namespace Sanicball
{
	public class CameraOrientation : MonoBehaviour
	{
		public Quaternion CameraRotation
		{
			get
			{
				return Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, 0f);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, CameraRotation, Vector3.one);
			Gizmos.DrawFrustum(base.transform.position, 72f, 1000f, 1f, 1.7777778f);
		}
	}
}
