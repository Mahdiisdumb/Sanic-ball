using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(Popup))]
	public class PopupSetServerListURL : MonoBehaviour
	{
		public InputField url;

		private OptionsPanel optionsPanel;

		public void Validate()
		{
			string serverListURL = url.text.Trim();
			optionsPanel.SetServerListURL(serverListURL);
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
				url.text = optionsPanel.GetServerListURL() ?? string.Empty;
			}
		}
	}
}
