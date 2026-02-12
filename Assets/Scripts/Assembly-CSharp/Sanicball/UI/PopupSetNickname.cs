using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(Popup))]
	public class PopupSetNickname : MonoBehaviour
	{
		public InputField nickname;

		public Text errorOutput;

		private OptionsPanel optionsPanel;

		public void Validate()
		{
			errorOutput.text = string.Empty;
			if (string.IsNullOrEmpty(nickname.text.Trim()))
			{
				errorOutput.text = "Nickname can't be empty!";
				return;
			}
			string text = nickname.text.Trim();
			optionsPanel.SetNickname(text);
			GetComponent<Popup>().Close();
		}

		private void Start()
		{
			optionsPanel = Object.FindObjectOfType<OptionsPanel>();
			if (!optionsPanel)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				nickname.text = optionsPanel.GetNickname() ?? string.Empty;
			}
		}
	}
}
