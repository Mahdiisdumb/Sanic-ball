using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Sanicball
{
	[RequireComponent(typeof(BlurOptimized))]
	public class ToggleBlur : MonoBehaviour
	{
		public float speed = 1f;

		private bool isOn;

		private float t;

		private BlurOptimized blur;

		public void Toggle()
		{
			isOn = !isOn;
		}

		private void Start()
		{
			blur = GetComponent<BlurOptimized>();
		}

		private void Update()
		{
			if (isOn && t < 1f)
			{
				t = Mathf.Lerp(t, 1f, speed * Time.deltaTime);
			}
			if (!isOn && t > 0f)
			{
				t = 0f;
			}
			blur.enabled = t > 0.01f;
			blur.blurSize = Mathf.Lerp(0f, 10f, t);
		}
	}
}
