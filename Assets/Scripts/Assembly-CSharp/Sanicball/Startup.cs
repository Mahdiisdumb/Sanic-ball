using Sanicball.Data;
using Sanicball.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball
{
	public class Startup : MonoBehaviour
	{
		public Intro intro;

		public CanvasGroup setNicknameGroup;

		public InputField nicknameField;

		public void ValidateNickname()
		{
			if (nicknameField.text.Trim() != string.Empty)
			{
				setNicknameGroup.alpha = 0f;
				ActiveData.GameSettings.nickname = nicknameField.text;
				intro.enabled = true;
			}
		}

		private void Start()
		{
			if (string.IsNullOrEmpty(ActiveData.GameSettings.nickname) || ActiveData.GameSettings.nickname == "Player")
			{
				setNicknameGroup.alpha = 1f;
				return;
			}
			setNicknameGroup.alpha = 0f;
			intro.enabled = true;
		}
	}
}
