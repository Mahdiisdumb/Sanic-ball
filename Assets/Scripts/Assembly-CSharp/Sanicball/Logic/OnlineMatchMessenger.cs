using System;
using System.Reflection;
using Lidgren.Network;
using Newtonsoft.Json;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
	public class OnlineMatchMessenger : MatchMessenger
	{
		public const string APP_ID = "Sanicball";

		private NetClient client;

		private JsonSerializerSettings serializerSettings;

		public event EventHandler<PlayerMovementArgs> OnPlayerMovement;

		public event EventHandler<DisconnectArgs> Disconnected;

		public OnlineMatchMessenger(NetClient client)
		{
			this.client = client;
			serializerSettings = new JsonSerializerSettings();
			serializerSettings.TypeNameHandling = TypeNameHandling.All;
		}

		public override void SendMessage<T>(T message)
		{
			NetOutgoingMessage netOutgoingMessage = client.CreateMessage();
			netOutgoingMessage.Write((byte)0);
			netOutgoingMessage.WriteTime(false);
			string source = JsonConvert.SerializeObject(message, serializerSettings);
			netOutgoingMessage.Write(source);
			client.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
		}

		public void SendPlayerMovement(MatchPlayer player)
		{
			NetOutgoingMessage netOutgoingMessage = client.CreateMessage();
			netOutgoingMessage.Write((byte)2);
			netOutgoingMessage.WriteTime(false);
			PlayerMovement playerMovement = PlayerMovement.CreateFromPlayer(player);
			playerMovement.WriteToMessage(netOutgoingMessage);
			client.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable);
		}

		public override void UpdateListeners()
		{
			NetIncomingMessage netIncomingMessage;
			while ((netIncomingMessage = client.ReadMessage()) != null)
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
					string text = netIncomingMessage.ReadString();
					NetConnectionStatus netConnectionStatus2 = netConnectionStatus;
					if (netConnectionStatus2 == NetConnectionStatus.Disconnected)
					{
						if (this.Disconnected != null)
						{
							this.Disconnected(this, new DisconnectArgs(text));
						}
					}
					else
					{
						Debug.Log(string.Concat("Status change received: ", netConnectionStatus, " - Message: ", text));
					}
					break;
				}
				case NetIncomingMessageType.Data:
					switch (netIncomingMessage.ReadByte())
					{
					case 0:
					{
						double num = netIncomingMessage.ReadTime(false);
						MatchMessage matchMessage = JsonConvert.DeserializeObject<MatchMessage>(netIncomingMessage.ReadString(), serializerSettings);
						MethodInfo method = typeof(OnlineMatchMessenger).GetMethod("ReceiveMessage", BindingFlags.Instance | BindingFlags.NonPublic);
						MethodInfo methodInfo = method.MakeGenericMethod(matchMessage.GetType());
						methodInfo.Invoke(this, new object[2] { matchMessage, num });
						break;
					}
					case 2:
					{
						double timestamp = netIncomingMessage.ReadTime(false);
						PlayerMovement movement = PlayerMovement.ReadFromMessage(netIncomingMessage);
						if (this.OnPlayerMovement != null)
						{
							this.OnPlayerMovement(this, new PlayerMovementArgs(timestamp, movement));
						}
						break;
					}
					}
					break;
				default:
					Debug.Log("Received unhandled message of type " + netIncomingMessage.MessageType);
					break;
				}
			}
		}

		public override void Close()
		{
			client.Disconnect("Client left the match");
		}

		private void ReceiveMessage<T>(T message, double timestamp) where T : MatchMessage
		{
			float travelTime = (float)(NetTime.Now - timestamp);
			for (int i = 0; i < listeners.Count; i++)
			{
				MatchMessageListener matchMessageListener = listeners[i];
				if (matchMessageListener.MessageType == message.GetType())
				{
					((MatchMessageHandler<T>)matchMessageListener.Handler)(message, travelTime);
				}
			}
		}
	}
}
