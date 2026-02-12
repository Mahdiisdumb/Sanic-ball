using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(Text))]
	public class TextSlideshow : MonoBehaviour
	{
		private Text t;

		[SerializeField]
		private string[] lines;

		[SerializeField]
		private float fadeTime = 0.2f;

		[SerializeField]
		private float waitTime = 3f;

		private int currentLine;

		private bool fadingOut;

		private float a;

		private float timer;

		private void Start()
		{
			timer = waitTime;
			t = GetComponent<Text>();
			t.text = lines[currentLine];
		}

		private void Update()
		{
			if (PauseMenu.GamePaused)
			{
				return;
			}
			if (fadingOut)
			{
				a -= Time.deltaTime / fadeTime;
				t.color = new Color(1f, 1f, 1f, a);
				if (a <= 0f)
				{
					fadingOut = false;
					currentLine++;
					if (currentLine >= lines.Length)
					{
						currentLine = 0;
					}
					t.text = lines[currentLine];
				}
			}
			else if (a < 1f)
			{
				a += Time.deltaTime / fadeTime;
				t.color = new Color(1f, 1f, 1f, a);
			}
			else
			{
				timer -= Time.deltaTime;
				if (timer < 0f && lines.Length > 1)
				{
					fadingOut = true;
					timer = waitTime;
				}
			}
		}
	}
}
