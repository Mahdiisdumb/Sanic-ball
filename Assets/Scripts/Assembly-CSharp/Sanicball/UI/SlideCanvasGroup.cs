using UnityEngine;
using UnityEngine.Events;

namespace Sanicball.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class SlideCanvasGroup : MonoBehaviour
	{
		public bool isOpen;

		public Vector2 closedPosition;

		public float time = 1f;

		public UnityEvent onOpen;

		public UnityEvent onClose;

		private CanvasGroup cg;

		private float pos;

		private Vector2 startPosition;

		private RectTransform rectTransform;

		public void Open()
		{
			isOpen = true;
			base.gameObject.SetActive(true);
			cg.alpha = 1f;
			cg.interactable = true;
			onOpen.Invoke();
		}

		public void Close()
		{
			isOpen = false;
			cg.interactable = false;
			onClose.Invoke();
		}

		private void Start()
		{
			rectTransform = GetComponent<RectTransform>();
			cg = GetComponent<CanvasGroup>();
			startPosition = rectTransform.anchoredPosition;
			cg.interactable = isOpen;
			cg.alpha = ((!isOpen) ? 0f : 1f);
			if (isOpen)
			{
				pos = 1f;
			}
			else
			{
				base.gameObject.SetActive(false);
			}
			UpdatePosition();
		}

		private void Update()
		{
			if (isOpen && pos < 1f)
			{
				pos = Mathf.Min(1f, pos + Time.deltaTime / time);
			}
			if (!isOpen && pos > 0f)
			{
				pos = Mathf.Max(0f, pos - Time.deltaTime / time);
				if (pos <= 0f)
				{
					cg.alpha = 0f;
					base.gameObject.SetActive(false);
				}
			}
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			float t = Mathf.SmoothStep(0f, 1f, pos);
			(base.transform as RectTransform).anchoredPosition = Vector2.Lerp(startPosition + closedPosition, startPosition, t);
		}
	}
}
