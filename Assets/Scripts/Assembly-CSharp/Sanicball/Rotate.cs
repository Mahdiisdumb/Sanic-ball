using UnityEngine;

namespace Sanicball
{
	public class Rotate : MonoBehaviour
	{
		public Vector3 angle;

		private void Start()
		{
		}

		private void Update()
		{
			base.transform.Rotate(angle * Time.deltaTime * 10f);
		}
	}
}
