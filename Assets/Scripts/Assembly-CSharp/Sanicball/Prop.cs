using System;
using SanicballCore;
using UnityEngine;

namespace Sanicball
{
	public class Prop : MonoBehaviour
	{
		[SerializeField]
		private Vector3 maxRandomRotation = new Vector3(20f, 360f, 20f);

		[SerializeField]
		private float maxRandomScale = 0.4f;

		private void Start()
		{
			int instanceID = GetInstanceID();
			System.Random rand = new System.Random(instanceID);
			float x = rand.NextFloatUniform() * maxRandomRotation.x;
			float y = rand.NextFloatUniform() * maxRandomRotation.y;
			float z = rand.NextFloatUniform() * maxRandomRotation.z;
			base.transform.rotation = Quaternion.Euler(x, y, z);
			Vector3 localScale = base.transform.localScale;
			float num = rand.NextFloatUniform() * maxRandomScale;
			base.transform.localScale = new Vector3(localScale.x + num, localScale.y + num, localScale.x + num);
		}
	}
}
