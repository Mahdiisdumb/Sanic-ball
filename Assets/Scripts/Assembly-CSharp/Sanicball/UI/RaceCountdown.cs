using System;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class RaceCountdown : MonoBehaviour
	{
		private int countdown = 5;

		private float timer = 4f;

		private float currentFontSize = 60f;

		private float targetFontSize = 60f;

		[SerializeField]
		private AudioClip countdown1;

		[SerializeField]
		private AudioClip countdown2;

		[SerializeField]
		private Text countdownLabel;

		private ESportMode esport;

		public event EventHandler OnCountdownFinished;

		private void Start()
		{
			if (ActiveData.ESportsFullyReady)
			{
				esport = UnityEngine.Object.Instantiate(ActiveData.ESportsPrefab);
			}
		}

		public void ApplyOffset(float time)
		{
			timer -= time;
		}

		private void Update()
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				string text = string.Empty;
				int num = 60;
				countdown--;
				switch (countdown)
				{
				case 4:
					text = "READY";
					num = 80;
					UISound.Play(countdown1);
					break;
				case 3:
					text = "STEADY";
					num = 100;
					UISound.Play(countdown1);
					break;
				case 2:
					text = "GET SET";
					num = 120;
					UISound.Play(countdown1);
					break;
				case 1:
					text = "GO FAST";
					num = 160;
					UISound.Play(countdown2);
					if (this.OnCountdownFinished != null)
					{
						this.OnCountdownFinished(this, new EventArgs());
					}
					if ((bool)esport)
					{
						esport.StartTheShit();
					}
					break;
				case 0:
					UnityEngine.Object.Destroy(base.gameObject);
					break;
				}
				countdownLabel.text = text;
				targetFontSize = num;
				timer = 1f;
				if (countdown == 1)
				{
					timer = 2f;
				}
			}
			currentFontSize = Mathf.Lerp(currentFontSize, targetFontSize, Time.deltaTime * 10f);
			countdownLabel.fontSize = (int)currentFontSize;
		}
	}
}
