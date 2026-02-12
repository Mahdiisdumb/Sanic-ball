using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class LocalPlayerInfoBox : MonoBehaviour
	{
		private const float fadeTime = 0.2f;

		private const float timerMax = 3f;

		private string[] lines;

		private int currentLine;

		private bool fadingOut;

		private float a;

		[SerializeField]
		private Text text;

		[SerializeField]
		private Image image;

		private float timer = 3f;

		private void Awake()
		{
			text.color = new Color(1f, 1f, 1f, 0f);
			lines = new string[2] { "Nice", "Meme" };
			DisplayCurrentLine();
		}

		private void Update()
		{
			if (fadingOut)
			{
				a -= Time.deltaTime / 0.2f;
				text.color = new Color(1f, 1f, 1f, a);
				if (a <= 0f)
				{
					fadingOut = false;
					currentLine++;
					if (currentLine >= lines.Length)
					{
						currentLine = 0;
					}
					DisplayCurrentLine();
				}
			}
			else if (a < 1f)
			{
				a += Time.deltaTime / 0.2f;
				text.color = new Color(1f, 1f, 1f, a);
			}
			else
			{
				timer -= Time.deltaTime;
				if (timer < 0f && lines.Length > 1)
				{
					fadingOut = true;
					timer = 3f;
				}
			}
		}

		public void SetLines(params string[] newLines)
		{
			lines = newLines;
			currentLine = 0;
			timer = 3f;
			DisplayCurrentLine();
		}

		public void SetIcon(Sprite sprite)
		{
			image.sprite = sprite;
		}

		private void DisplayCurrentLine()
		{
			text.text = lines[currentLine];
		}
	}
}
