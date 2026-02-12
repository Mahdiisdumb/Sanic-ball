using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sanicball.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class Popup : MonoBehaviour
	{
		public GameObject firstSelectedOnOpen;

		public Action onClose;

		private CanvasGroup cg;

		private float alpha;

		private float fadeTime = 0.2f;

		private bool closing;

		public void Close()
		{
			closing = true;
			if (onClose != null)
			{
				onClose();
			}
		}

		private void Start()
		{
			cg = GetComponent<CanvasGroup>();
			cg.alpha = 0f;
			if ((bool)firstSelectedOnOpen)
			{
				EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
				if ((bool)eventSystem)
				{
					eventSystem.SetSelectedGameObject(firstSelectedOnOpen);
				}
			}
		}

		private void Update()
		{
			if (!closing && alpha < 1f)
			{
				alpha = Mathf.Min(alpha + Time.deltaTime / fadeTime, 1f);
				cg.alpha = alpha;
			}
			if (closing && alpha > 0f)
			{
				alpha = Mathf.Max(alpha - Time.deltaTime / fadeTime, 0f);
				cg.alpha = alpha;
				if (alpha <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
