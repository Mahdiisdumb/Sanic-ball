using System;
using System.Net;
using Lidgren.Network;
using Newtonsoft.Json;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
	public class MatchStarter : MonoBehaviour
	{
		public const string APP_ID = "Sanicball";

		[SerializeField]
		private MatchManager matchManagerPrefab;

		[SerializeField]
		private Popup connectingPopupPrefab;

		[SerializeField]
		private PopupHandler popupHandler;

		private PopupConnecting activeConnectingPopup;

		private NetClient joiningClient;

		private void Update()
		{
			if (joiningClient == null)
			{
				return;
			}
			NetIncomingMessage netIncomingMessage;
			while ((netIncomingMessage = joiningClient.ReadMessage()) != null)
			{
				switch (netIncomingMessage.MessageType)
				{
				case NetIncomingMessageType.VerboseDebugMessage:
				case NetIncomingMessageType.DebugMessage:
					Debug.Log(netIncomingMessage.ReadString());
					break;
				case NetIncomingMessageType.WarningMessage:
					Debug.LogWarning(netIncomingMessage.ReadString());
					break;
				case NetIncomingMessageType.ErrorMessage:
					Debug.LogError(netIncomingMessage.ReadString());
					break;
				case NetIncomingMessageType.StatusChanged:
				{
					NetConnectionStatus netConnectionStatus = (NetConnectionStatus)netIncomingMessage.ReadByte();
					switch (netConnectionStatus)
					{
					case NetConnectionStatus.Connected:
						Debug.Log("Connected! Now waiting for match state");
						activeConnectingPopup.ShowMessage("Receiving match state...");
						break;
					case NetConnectionStatus.Disconnected:
						activeConnectingPopup.ShowMessage(netIncomingMessage.ReadString());
						break;
					default:
					{
						string text = netIncomingMessage.ReadString();
						Debug.Log(string.Concat("Status change received: ", netConnectionStatus, " - Message: ", text));
						break;
					}
					}
					break;
				}
				case NetIncomingMessageType.Data:
				{
					byte b = netIncomingMessage.ReadByte();
					if (b == 1)
					{
						try
						{
							MatchState matchState = MatchState.ReadFromMessage(netIncomingMessage);
							BeginOnlineGame(matchState);
						}
						catch (Exception ex)
						{
							activeConnectingPopup.ShowMessage("Failed to read match message - cannot join server!");
							Debug.LogError("Could not read match state, error: " + ex.Message);
						}
					}
					break;
				}
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				popupHandler.CloseActivePopup();
				joiningClient.Disconnect("Cancelled");
				joiningClient = null;
			}
		}

		public void BeginLocalGame()
		{
			MatchManager matchManager = UnityEngine.Object.Instantiate(matchManagerPrefab);
			matchManager.InitLocalMatch();
		}

		public void JoinOnlineGame(string ip = "127.0.0.1", int port = 25000)
		{
			JoinOnlineGame(new IPEndPoint(IPAddress.Parse(ip), port));
		}

		public void JoinOnlineGame(IPEndPoint endpoint)
		{
			NetPeerConfiguration config = new NetPeerConfiguration("Sanicball");
			joiningClient = new NetClient(config);
			joiningClient.Start();
			NetOutgoingMessage netOutgoingMessage = joiningClient.CreateMessage();
			ClientInfo value = new ClientInfo(0.82f, false);
			netOutgoingMessage.Write(JsonConvert.SerializeObject(value));
			joiningClient.Connect(endpoint, netOutgoingMessage);
			popupHandler.OpenPopup(connectingPopupPrefab);
			activeConnectingPopup = UnityEngine.Object.FindObjectOfType<PopupConnecting>();
		}

		private void BeginOnlineGame(MatchState matchState)
		{
			MatchManager matchManager = UnityEngine.Object.Instantiate(matchManagerPrefab);
			matchManager.InitOnlineMatch(joiningClient, matchState);
		}
	}
}
