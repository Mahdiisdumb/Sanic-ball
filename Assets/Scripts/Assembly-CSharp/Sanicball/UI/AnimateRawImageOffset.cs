using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(RawImage))]
	public class AnimateRawImageOffset : MonoBehaviour
	{
		public Vector2 speed;

		private Vector2 offset;

		private RawImage img;

		private void Start()
		{
			img = GetComponent<RawImage>();
		}

		private void Update()
		{
			offset += new Vector2(speed.x * Time.deltaTime, speed.y * Time.deltaTime);
			if (offset.x >= 1f)
			{
				offset += new Vector2(-1f, 0f);
			}
			if (offset.y >= 1f)
			{
				offset += new Vector2(0f, -1f);
			}
			img.uvRect = new Rect(offset.x, offset.y, img.uvRect.width, img.uvRect.height);
		}
	}
}
