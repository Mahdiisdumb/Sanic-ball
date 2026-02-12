using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;
using Newtonsoft.Json;
using Sanicball.Data;
using Sanicball.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class OnlinePanel : MonoBehaviour
	{
		public Transform targetServerListContainer;

		public Text errorField;

		public Text serverCountField;

		public ServerListItem serverListItemPrefab;

		public Selectable aboveList;

		public Selectable belowList;

		private List<ServerListItem> servers = new List<ServerListItem>();

		private List<string> serverBrowserIPs = new List<string>();

		private NetClient discoveryClient;

		private WWW serverBrowserRequester;

		private DateTime latestLocalRefreshTime;

		private DateTime latestBrowserRefreshTime;

		public void RefreshServers()
		{
			serverBrowserIPs.Clear();
			discoveryClient.DiscoverLocalPeers(25000);
			latestLocalRefreshTime = DateTime.Now;
			serverBrowserRequester = new WWW(ActiveData.GameSettings.serverListURL);
			serverCountField.text = "Refreshing servers, hang on...";
			errorField.enabled = false;
			foreach (ServerListItem server in servers)
			{
				UnityEngine.Object.Destroy(server.gameObject);
			}
			servers.Clear();
		}

		private void Awake()
		{
			errorField.enabled = false;
			NetPeerConfiguration netPeerConfiguration = new NetPeerConfiguration("Sanicball");
			netPeerConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
			discoveryClient = new NetClient(netPeerConfiguration);
			discoveryClient.Start();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F5))
			{
				RefreshServers();
			}
			if (serverBrowserRequester != null && serverBrowserRequester.isDone)
			{
				if (string.IsNullOrEmpty(serverBrowserRequester.error))
				{
					latestBrowserRefreshTime = DateTime.Now;
					string text = serverBrowserRequester.text;
					string[] array = text.Split(new string[1] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						int num = text2.LastIndexOf(':');
						string ip = text2.Substring(0, num);
						string s = text2.Substring(num + 1, text2.Length - (num + 1));
						int portInt;
						if (int.TryParse(s, out portInt))
						{
							Thread thread = new Thread((ThreadStart)delegate
							{
								discoveryClient.DiscoverKnownPeer(ip, portInt);
							});
							thread.Start();
							serverBrowserIPs.Add(ip);
						}
					}
					serverCountField.text = "0 servers";
				}
				else
				{
					Debug.LogError("Failed to receive servers - " + serverBrowserRequester.error);
					serverCountField.text = "Cannot access server list URL!";
				}
				serverBrowserRequester = null;
			}
			NetIncomingMessage netIncomingMessage;
			while ((netIncomingMessage = discoveryClient.ReadMessage()) != null)
			{
				NetIncomingMessageType messageType = netIncomingMessage.MessageType;
				if (messageType == NetIncomingMessageType.DiscoveryResponse)
				{
					ServerInfo info;
					try
					{
						info = JsonConvert.DeserializeObject<ServerInfo>(netIncomingMessage.ReadString());
					}
					catch (JsonException ex)
					{
						Debug.LogError("Failed to deserialize info for a server: " + ex.Message);
						continue;
					}
					bool flag = !serverBrowserIPs.Contains(netIncomingMessage.SenderEndPoint.Address.ToString());
					DateTime dateTime = ((!flag) ? latestBrowserRefreshTime : latestLocalRefreshTime);
					double totalMilliseconds = (DateTime.Now - dateTime).TotalMilliseconds;
					ServerListItem serverListItem = UnityEngine.Object.Instantiate(serverListItemPrefab);
					serverListItem.transform.SetParent(targetServerListContainer, false);
					serverListItem.Init(info, netIncomingMessage.SenderEndPoint, (int)totalMilliseconds, flag);
					servers.Add(serverListItem);
					RefreshNavigation();
					serverCountField.text = servers.Count + ((servers.Count != 1) ? " servers" : " server");
				}
				else
				{
					Debug.Log(string.Concat("Server discovery client received an unhandled NetMessage (", netIncomingMessage.MessageType, ")"));
				}
			}
		}

		private void RefreshNavigation()
		{
			for (int i = 0; i < servers.Count; i++)
			{
				Button component = servers[i].GetComponent<Button>();
				if ((bool)component)
				{
					Navigation navigation = new Navigation
					{
						mode = Navigation.Mode.Explicit
					};
					if (i == 0)
					{
						navigation.selectOnUp = aboveList;
						Navigation navigation2 = aboveList.navigation;
						navigation2.selectOnDown = component;
						aboveList.navigation = navigation2;
					}
					else
					{
						navigation.selectOnUp = servers[i - 1].GetComponent<Button>();
					}
					if (i == servers.Count - 1)
					{
						navigation.selectOnDown = belowList;
						Navigation navigation3 = belowList.navigation;
						navigation3.selectOnUp = component;
						belowList.navigation = navigation3;
					}
					else
					{
						navigation.selectOnDown = servers[i + 1].GetComponent<Button>();
					}
					component.navigation = navigation;
				}
			}
		}
	}
}
