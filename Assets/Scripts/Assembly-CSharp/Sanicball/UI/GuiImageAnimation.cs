using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(Image))]
	public class GuiImageAnimation : MonoBehaviour
	{
		public Sprite[] sprites;

		public float switchTime;

		public bool pickRandomly;

		public bool destroyOnEnd;

		private int currentSpr;

		private float timer;

		private Image image;

		private void Start()
		{
			timer = switchTime;
			image = GetComponent<Image>();
		}

		private void Update()
		{
			timer -= Time.deltaTime;
			if (!(timer <= 0f))
			{
				return;
			}
			if (!pickRandomly)
			{
				currentSpr++;
				if (currentSpr >= sprites.Length)
				{
					if (destroyOnEnd)
					{
						Object.Destroy(base.gameObject);
					}
					currentSpr = 0;
				}
				image.sprite = sprites[currentSpr];
			}
			else
			{
				image.sprite = sprites[Random.Range(0, sprites.Length)];
			}
			timer += switchTime;
		}
	}
}
