using System;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

namespace Sanicball.UI
{
    [Serializable]
    public class ServerInfo
    {
        public string Name;
        public int MaxPlayers;
        public int CurrentPlayers;
    }

    public class OnlinePanel : MonoBehaviour
    {
        [Header("UI")]
        public Transform targetServerListContainer;
        public Text errorField;
        public Text serverCountField;
        public ServerListItem serverListItemPrefab;
        public Selectable aboveList;
        public Selectable belowList;

        private List<ServerListItem> servers = new List<ServerListItem>();
        private NetClient discoveryClient;
        private DateTime latestLocalRefreshTime;

        // Placeholder "online" list
        private bool placeholderOnlineEnabled = true;

        private void Awake()
        {
            errorField.enabled = false;

            // LAN discovery
            var config = new NetPeerConfiguration("SANICBAL_MAHDIISDUMBVER_LAN");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            discoveryClient = new NetClient(config);
            discoveryClient.Start();
        }

        private void Update()
        {
            // Refresh on F5
            if (Input.GetKeyDown(KeyCode.F5))
            {
                RefreshServers();
            }

            // Check for LAN messages
            NetIncomingMessage msg;
            while ((msg = discoveryClient.ReadMessage()) != null)
            {
                if (msg.MessageType == NetIncomingMessageType.DiscoveryResponse)
                {
                    AddServerFromMessage(msg, true);
                }
                else
                {
                    Debug.Log("Unhandled NetMessage: " + msg.MessageType);
                }
            }

            // Placeholder "online servers"
            if (placeholderOnlineEnabled && servers.Count == 0)
            {
                AddPlaceholderServer("Placeholder Online Server", "127.0.0.1", 25001);
                placeholderOnlineEnabled = false;
            }
        }

        public void RefreshServers()
        {
            // Clear previous
            foreach (var serv in servers)
                Destroy(serv.gameObject);

            servers.Clear();
            errorField.enabled = false;
            serverCountField.text = "0 servers";

            // Start LAN discovery
            latestLocalRefreshTime = DateTime.Now;
            discoveryClient.DiscoverLocalPeers(25000);

            // Reset placeholder
            placeholderOnlineEnabled = true;
        }

        private void AddServerFromMessage(NetIncomingMessage msg, bool isLocal)
        {
            ServerInfo info;
            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerInfo>(msg.ReadString());
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to deserialize server info: " + ex.Message);
                return;
            }

            var server = Instantiate(serverListItemPrefab);
            server.transform.SetParent(targetServerListContainer, false);
            server.Init(info, msg.SenderEndPoint, 0, isLocal);
            servers.Add(server);

            UpdateServerCountText();
            RefreshNavigation();
        }

        private void AddPlaceholderServer(string name, string ip, int port)
        {
            var info = new ServerInfo
            {
                Name = name,
                MaxPlayers = 8,
                CurrentPlayers = 0
            };

            var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var server = Instantiate(serverListItemPrefab);
            server.transform.SetParent(targetServerListContainer, false);
            server.Init(info, endpoint, 0, false);
            servers.Add(server);

            serverCountField.text = servers.Count + " servers (placeholder)";
            RefreshNavigation();
        }

        private void RefreshNavigation()
        {
            for (int i = 0; i < servers.Count; i++)
            {
                var button = servers[i].GetComponent<Button>();
                if (!button) continue;

                var nav = new Navigation() { mode = Navigation.Mode.Explicit };

                // Up
                if (i == 0)
                {
                    nav.selectOnUp = aboveList;
                    if (aboveList)
                    {
                        var nav2 = aboveList.navigation;
                        nav2.selectOnDown = button;
                        aboveList.navigation = nav2;
                    }
                }
                else
                {
                    nav.selectOnUp = servers[i - 1].GetComponent<Button>();
                }

                // Down
                if (i == servers.Count - 1)
                {
                    nav.selectOnDown = belowList;
                    if (belowList)
                    {
                        var nav2 = belowList.navigation;
                        nav2.selectOnUp = button;
                        belowList.navigation = nav2;
                    }
                }
                else
                {
                    nav.selectOnDown = servers[i + 1].GetComponent<Button>();
                }

                button.navigation = nav;
            }
        }

        private void UpdateServerCountText()
        {
            serverCountField.text = servers.Count + (servers.Count == 1 ? " server" : " servers");
        }

        private void OnDestroy()
        {
            discoveryClient?.Shutdown("Panel destroyed");
        }
    }
}