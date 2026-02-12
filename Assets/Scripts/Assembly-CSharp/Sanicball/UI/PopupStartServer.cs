using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class PopupStartServer : MonoBehaviour
	{
		public InputField serverNameInput;

		public Text serverNameOutput;

		public InputField portInput;

		public Text portOutput;

		public Text maxPlayersText;

		public Text showOnServerListText;

		public Text enableNatPunchingText;

		public CanvasRenderer enableNatPunchingPanel;

		public Slideshow slideshow;

		private int maxPlayers = 12;

		private bool showOnList = true;

		private bool useNat;

		public void MaxPlayersUp()
		{
			if (maxPlayers < 64)
			{
				maxPlayers++;
			}
			else
			{
				maxPlayers = 1;
			}
			UpdateFields();
		}

		public void MaxPlayersDown()
		{
			if (maxPlayers > 1)
			{
				maxPlayers--;
			}
			else
			{
				maxPlayers = 64;
			}
			UpdateFields();
		}

		public void ToggleShowOnList()
		{
			showOnList = !showOnList;
			showOnServerListText.text = ((!showOnList) ? "No" : "Yes");
			enableNatPunchingPanel.gameObject.SetActive(showOnList);
		}

		public void ToggleNatPunching()
		{
			useNat = !useNat;
			enableNatPunchingText.text = ((!useNat) ? "No" : "Yes");
		}

		public void ValidateServerName()
		{
			serverNameOutput.text = string.Empty;
			string text = serverNameInput.text;
			bool flag = true;
			if (text.Length <= 0)
			{
				serverNameOutput.text = "Server name can't be empty!";
				flag = false;
			}
			if (flag)
			{
				slideshow.NextSlide();
			}
		}

		public void ValidatePort()
		{
			portOutput.text = string.Empty;
			int result;
			if (int.TryParse(portInput.text, out result))
			{
				if (result <= 0 || result >= 49152)
				{
					portOutput.text = "Port number must be between 1 and 49151.";
				}
			}
			else
			{
				portOutput.text = "Failed to parse port as an integer!";
			}
		}

		private void Start()
		{
			UpdateFields();
			showOnServerListText.text = "Yes";
			enableNatPunchingText.text = "No";
		}

		private void UpdateFields()
		{
			maxPlayersText.text = maxPlayers.ToString();
		}
	}
}
