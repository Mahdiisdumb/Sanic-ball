using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class Slideshow : MonoBehaviour
	{
		public Text activeSlideDisplay;

		public int firstActive;

		private SlideshowSlide[] slides;

		private int currentSlide = -1;

		public void NextSlide()
		{
			if (currentSlide < slides.Length - 1)
			{
				SetSlide(currentSlide + 1);
			}
			else
			{
				SetSlide(0);
			}
		}

		public void PrevSlide()
		{
			if (currentSlide > 0)
			{
				SetSlide(currentSlide - 1);
			}
			else
			{
				SetSlide(slides.Length - 1);
			}
		}

		public void SetSlide(int slide)
		{
			if (slide >= 0 && slide < slides.Length)
			{
				if (currentSlide >= 0 && currentSlide < slides.Length)
				{
					slides[currentSlide].gameObject.SetActive(false);
				}
				slides[slide].gameObject.SetActive(true);
				currentSlide = slide;
				if ((bool)activeSlideDisplay)
				{
					activeSlideDisplay.text = currentSlide + 1 + "/" + slides.Length;
				}
			}
		}

		public void TrySetSelectedGameObject(GameObject o)
		{
			EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
			if ((bool)eventSystem)
			{
				eventSystem.SetSelectedGameObject(o);
			}
		}

		private void Start()
		{
			slides = GetComponentsInChildren<SlideshowSlide>();
			SlideshowSlide[] array = slides;
			foreach (SlideshowSlide slideshowSlide in array)
			{
				slideshowSlide.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
				slideshowSlide.gameObject.SetActive(false);
			}
			SetSlide(firstActive);
		}

		private void Update()
		{
		}
	}
}
