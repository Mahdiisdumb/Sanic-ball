using UnityEngine;
using UnityEngine.Events;

namespace Sanicball.UI
{
	public class PressAnyKey : MonoBehaviour
	{
		public UnityEvent onAnyKeyPressed;

		public float timer = 10f;

		private float spin;

		private void Update()
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				if (spin == 0f)
				{
					spin = 0.01f;
				}
				if (spin < 1000000f)
				{
					spin += Time.deltaTime * spin;
				}
			}
			base.transform.Rotate(0f, 0f, spin * Time.deltaTime);
			if (Input.anyKeyDown)
			{
				onAnyKeyPressed.Invoke();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
