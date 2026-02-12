using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class MusicPlayerCanvas : MonoBehaviour
	{
		public CanvasGroup panel;

		public Text label;

		public bool lobbyOffset;

		private float showTimer;

		private void Start()
		{
			panel.alpha = 0f;
			if (lobbyOffset)
			{
				RectTransform component = panel.GetComponent<RectTransform>();
				Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.y += 48f;
				component.anchoredPosition = anchoredPosition;
			}
		}

		private void Update()
		{
			if (showTimer > 0f)
			{
				panel.alpha = Mathf.Lerp(panel.alpha, 1f, Time.deltaTime * 20f);
				showTimer -= Time.deltaTime;
			}
			else
			{
				panel.alpha = Mathf.Lerp(panel.alpha, 0f, Time.deltaTime * 20f);
			}
		}

		public void Show(string text)
		{
			label.text = text;
			showTimer = 5f;
		}
	}
}
