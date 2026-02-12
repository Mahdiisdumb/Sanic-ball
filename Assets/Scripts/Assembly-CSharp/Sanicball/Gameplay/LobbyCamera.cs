using System.Collections.Generic;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[RequireComponent(typeof(Camera))]
	public class LobbyCamera : MonoBehaviour
	{
		public float rotationSpeed;

		private Quaternion startRotation;

		private Quaternion targetRotation;

		private List<Ball> balls = new List<Ball>();

		public void AddBall(Ball b)
		{
			balls.Add(b);
		}

		private void Start()
		{
			startRotation = base.transform.rotation;
		}

		private void Update()
		{
			if (balls.Count > 0)
			{
				Vector3 zero = Vector3.zero;
				List<Ball> list = new List<Ball>(balls);
				foreach (Ball item in list)
				{
					if (item == null)
					{
						balls.Remove(item);
						continue;
					}
					zero += item.transform.position;
					if ((bool)item.Input)
					{
						item.Input.LookDirection = base.transform.rotation;
					}
				}
				Vector3 vector = zero / list.Count;
				targetRotation = Quaternion.LookRotation(vector - base.transform.position);
			}
			else
			{
				targetRotation = startRotation;
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}
}
