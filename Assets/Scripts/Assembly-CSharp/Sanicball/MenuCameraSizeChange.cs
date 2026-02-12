using UnityEngine;

namespace Sanicball
{
	public class MenuCameraSizeChange : MonoBehaviour
	{
		public float time = 0.5f;

		public float menuWidth = 400f;

		public Canvas canvas;

		private bool resized;

		private float pos;

		public void Resize()
		{
			resized = true;
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (resized)
			{
				pos = Mathf.Min(1f, pos + Time.deltaTime / time);
				float num = Mathf.SmoothStep(0f, 1f, pos);
				float num2 = menuWidth * canvas.scaleFactor;
				Camera.main.rect = new Rect(0f, 0f, 1f - num * num2 / (float)Screen.width, 1f);
			}
		}
	}
}
