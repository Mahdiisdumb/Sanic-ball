using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class Intro : MonoBehaviour
	{
		public string menuSceneName = "Menu";

		public Image[] images;

		public float imgTime = 0.2f;

		public float fadeTime = 0.05f;

		private int curImg;

		private bool isHoldingImage;

		private float holdImageTimer;

		private bool isFadeOut;

		private void Start()
		{
			holdImageTimer = imgTime;
		}

		private void Update()
		{
			if (isHoldingImage)
			{
				holdImageTimer -= Time.deltaTime;
				if (holdImageTimer <= 0f)
				{
					isHoldingImage = false;
					isFadeOut = true;
				}
			}
			else if (isFadeOut)
			{
				float a = images[curImg].color.a;
				a -= fadeTime * Time.deltaTime;
				images[curImg].color = new Color(1f, 1f, 1f, a);
				if (a <= 0f)
				{
					NextImage();
					isFadeOut = false;
				}
			}
			else
			{
				float a2 = images[curImg].color.a;
				a2 += fadeTime * Time.deltaTime;
				images[curImg].color = new Color(1f, 1f, 1f, a2);
				if (a2 >= 1f)
				{
					isHoldingImage = true;
				}
			}
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
			{
				GoToMenu();
			}
		}

		private void NextImage()
		{
			if (curImg >= images.Length - 1)
			{
				GoToMenu();
				return;
			}
			images[curImg].enabled = false;
			curImg++;
			images[curImg].enabled = true;
			images[curImg].color = new Color(1f, 1f, 1f, 0f);
			holdImageTimer += imgTime;
		}

		private void GoToMenu()
		{
			SceneManager.LoadScene(menuSceneName);
			isHoldingImage = true;
		}
	}
}
