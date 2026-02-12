using System.Net;
using Sanicball.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class ServerListItem : MonoBehaviour
	{
		[SerializeField]
		private Text serverNameText;

		[SerializeField]
		private Text serverStatusText;

		[SerializeField]
		private Text playerCountText;

		[SerializeField]
		private Text pingText;

		private ServerInfo info;

		private IPEndPoint endpoint;

		public void Init(ServerInfo info, IPEndPoint endpoint, int pingMs, bool isLocal)
		{
			serverNameText.text = info.Config.ServerName;
			serverStatusText.text = ((!info.InRace) ? "In lobby" : "In race");
			if (isLocal)
			{
				serverStatusText.text += " - LAN server";
			}
			playerCountText.text = info.Players + "/" + info.Config.MaxPlayers;
			pingText.text = pingMs + "ms";
			this.endpoint = endpoint;
		}

		public void Join()
		{
			MatchStarter matchStarter = Object.FindObjectOfType<MatchStarter>();
			if ((bool)matchStarter)
			{
				matchStarter.JoinOnlineGame(endpoint);
			}
			else
			{
				Debug.LogError("No match starter found");
			}
		}
	}
}
