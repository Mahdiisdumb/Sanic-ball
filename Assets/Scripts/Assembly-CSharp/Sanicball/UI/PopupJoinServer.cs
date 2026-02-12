using Sanicball.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class PopupJoinServer : MonoBehaviour
	{
		private const int LOWEST_PORT_NUM = 1024;

		private const int HIGHEST_PORT_NUM = 49151;

		[SerializeField]
		private InputField ipInput;

		[SerializeField]
		private InputField portInput;

		[SerializeField]
		private Text portOutput;

		public void Connect()
		{
			portOutput.text = string.Empty;
			int result;
			if (int.TryParse(portInput.text, out result))
			{
				if (result >= 1024 && result <= 49151)
				{
					MatchStarter matchStarter = Object.FindObjectOfType<MatchStarter>();
					matchStarter.JoinOnlineGame(ipInput.text, result);
					return;
				}
				portOutput.text = "Port number must be between " + 1024 + " and " + 49151 + ".";
			}
			else
			{
				portOutput.text = "Port must be a number!";
			}
		}
	}
}
