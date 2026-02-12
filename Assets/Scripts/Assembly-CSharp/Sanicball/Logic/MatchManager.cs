using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lidgren.Network;
using Sanicball.Data;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sanicball.Logic
{
	public class MatchManager : MonoBehaviour
	{
		private const float lobbyTimerMax = 3f;

		private const int NET_UPDATES_PER_SECOND = 40;

		[SerializeField]
		private string lobbySceneName = "Lobby";

		[SerializeField]
		private PauseMenu pauseMenuPrefab;

		[SerializeField]
		private Chat chatPrefab;

		[SerializeField]
		private RaceManager raceManagerPrefab;

		[SerializeField]
		private Popup disconnectedPopupPrefab;

		[SerializeField]
		private Marker markerPrefab;

		private List<MatchClient> clients = new List<MatchClient>();

		private Guid myGuid;

		private List<MatchPlayer> players = new List<MatchPlayer>();

		private MatchSettings currentSettings;

		private bool lobbyTimerOn;

		private float lobbyTimer = 3f;

		private bool autoStartTimerOn;

		private float autoStartTimer;

		private bool inLobby;

		private bool loadingLobby;

		private bool loadingStage;

		private bool joiningRaceInProgress;

		private bool showSettingsOnLobbyLoad;

		private MatchMessenger messenger;

		private Chat activeChat;

		private float netUpdateTimer;

		public bool OnlineMode
		{
			get
			{
				return messenger is OnlineMatchMessenger;
			}
		}

		public ReadOnlyCollection<MatchClient> Clients
		{
			get
			{
				return clients.AsReadOnly();
			}
		}

		public ReadOnlyCollection<MatchPlayer> Players
		{
			get
			{
				return players.AsReadOnly();
			}
		}

		public MatchSettings CurrentSettings
		{
			get
			{
				return currentSettings;
			}
		}

		public Guid LocalClientGuid
		{
			get
			{
				return myGuid;
			}
		}

		public bool AutoStartTimerOn
		{
			get
			{
				return autoStartTimerOn;
			}
		}

		public float AutoStartTimer
		{
			get
			{
				return autoStartTimer;
			}
		}

		public bool InLobby
		{
			get
			{
				return inLobby;
			}
		}

		public event EventHandler<MatchPlayerEventArgs> MatchPlayerAdded;

		public event EventHandler<MatchPlayerEventArgs> MatchPlayerRemoved;

		public event EventHandler MatchSettingsChanged;

		public void RequestSettingsChange(MatchSettings newSettings)
		{
			messenger.SendMessage(new SettingsChangedMessage(newSettings));
		}

		public void RequestPlayerJoin(ControlType ctrlType, int initialCharacter)
		{
			messenger.SendMessage(new PlayerJoinedMessage(myGuid, ctrlType, initialCharacter));
		}

		public void RequestPlayerLeave(ControlType ctrlType)
		{
			messenger.SendMessage(new PlayerLeftMessage(myGuid, ctrlType));
		}

		public void RequestCharacterChange(ControlType ctrlType, int newCharacter)
		{
			messenger.SendMessage(new CharacterChangedMessage(myGuid, ctrlType, newCharacter));
		}

		public void RequestReadyChange(ControlType ctrlType, bool ready)
		{
			messenger.SendMessage(new ChangedReadyMessage(myGuid, ctrlType, ready));
		}

		public void RequestLoadLobby()
		{
			messenger.SendMessage(new LoadLobbyMessage());
		}

		private void SettingsChangedCallback(SettingsChangedMessage msg, float travelTime)
		{
			currentSettings = msg.NewMatchSettings;
			if (this.MatchSettingsChanged != null)
			{
				this.MatchSettingsChanged(this, EventArgs.Empty);
			}
		}

		private void ClientJoinedCallback(ClientJoinedMessage msg, float travelTime)
		{
			clients.Add(new MatchClient(msg.ClientGuid, msg.ClientName));
			Debug.Log("New client " + msg.ClientName);
		}

		private void ClientLeftCallback(ClientLeftMessage msg, float travelTime)
		{
			List<MatchPlayer> list = players.Where((MatchPlayer a) => a.ClientGuid == msg.ClientGuid).ToList();
			foreach (MatchPlayer item in list)
			{
				PlayerLeftCallback(new PlayerLeftMessage(item.ClientGuid, item.CtrlType), travelTime);
			}
			clients.RemoveAll((MatchClient a) => a.Guid == msg.ClientGuid);
		}

		private void PlayerJoinedCallback(PlayerJoinedMessage msg, float travelTime)
		{
			MatchPlayer matchPlayer = new MatchPlayer(msg.ClientGuid, msg.CtrlType, msg.InitialCharacter);
			players.Add(matchPlayer);
			if (inLobby)
			{
				SpawnLobbyBall(matchPlayer);
			}
			StopLobbyTimer();
			if (this.MatchPlayerAdded != null)
			{
				this.MatchPlayerAdded(this, new MatchPlayerEventArgs(matchPlayer, msg.ClientGuid == myGuid));
			}
		}

		private void PlayerLeftCallback(PlayerLeftMessage msg, float travelTime)
		{
			MatchPlayer matchPlayer = players.FirstOrDefault((MatchPlayer a) => a.ClientGuid == msg.ClientGuid && a.CtrlType == msg.CtrlType);
			if (matchPlayer != null)
			{
				players.Remove(matchPlayer);
				if ((bool)matchPlayer.BallObject)
				{
					matchPlayer.BallObject.CreateRemovalParticles();
					UnityEngine.Object.Destroy(matchPlayer.BallObject.gameObject);
				}
				if (this.MatchPlayerRemoved != null)
				{
					this.MatchPlayerRemoved(this, new MatchPlayerEventArgs(matchPlayer, msg.ClientGuid == myGuid));
				}
			}
		}

		private void CharacterChangedCallback(CharacterChangedMessage msg, float travelTime)
		{
			if (!inLobby)
			{
				Debug.LogError("Cannot set character outside of lobby!");
			}
			MatchPlayer matchPlayer = players.FirstOrDefault((MatchPlayer a) => a.ClientGuid == msg.ClientGuid && a.CtrlType == msg.CtrlType);
			if (matchPlayer != null)
			{
				matchPlayer.CharacterId = msg.NewCharacter;
				SpawnLobbyBall(matchPlayer);
			}
		}

		private void ChangedReadyCallback(ChangedReadyMessage msg, float travelTime)
		{
			MatchPlayer matchPlayer = players.FirstOrDefault((MatchPlayer a) => a.ClientGuid == msg.ClientGuid && a.CtrlType == msg.CtrlType);
			if (matchPlayer != null)
			{
				matchPlayer.ReadyToRace = !matchPlayer.ReadyToRace;
				bool flag = players.TrueForAll((MatchPlayer a) => a.ReadyToRace);
				if (flag && !lobbyTimerOn)
				{
					StartLobbyTimer(travelTime);
				}
				if (!flag && lobbyTimerOn)
				{
					StopLobbyTimer();
				}
			}
		}

		private void LoadRaceCallback(LoadRaceMessage msg, float travelTime)
		{
			StopLobbyTimer();
			CameraFade.StartAlphaFade(Color.black, false, 0.3f, 0.05f, delegate
			{
				GoToStage();
			});
		}

		private void ChatCallback(ChatMessage msg, float travelTime)
		{
			if ((bool)activeChat)
			{
				activeChat.ShowMessage(msg.Type, msg.From, msg.Text);
			}
		}

		private void LoadLobbyCallback(LoadLobbyMessage msg, float travelTime)
		{
			GoToLobby();
		}

		private void AutoStartTimerCallback(AutoStartTimerMessage msg, float travelTime)
		{
			autoStartTimerOn = msg.Enabled;
			autoStartTimer = (float)currentSettings.AutoStartTime - travelTime;
		}

		public void InitLocalMatch()
		{
			currentSettings = ActiveData.MatchSettings;
			messenger = new LocalMatchMessenger();
			showSettingsOnLobbyLoad = true;
			GoToLobby();
		}

		public void InitOnlineMatch(NetClient client, MatchState matchState)
		{
			foreach (MatchClientState client2 in matchState.Clients)
			{
				clients.Add(new MatchClient(client2.Guid, client2.Name));
			}
			foreach (MatchPlayerState player in matchState.Players)
			{
				MatchPlayer matchPlayer = new MatchPlayer(player.ClientGuid, player.CtrlType, player.CharacterId);
				matchPlayer.ReadyToRace = player.ReadyToRace;
				players.Add(matchPlayer);
				if (inLobby)
				{
					SpawnLobbyBall(matchPlayer);
				}
			}
			currentSettings = matchState.Settings;
			autoStartTimerOn = matchState.CurAutoStartTime != 0f;
			autoStartTimer = matchState.CurAutoStartTime;
			OnlineMatchMessenger onlineMatchMessenger = (OnlineMatchMessenger)(messenger = new OnlineMatchMessenger(client));
			onlineMatchMessenger.Disconnected += delegate(object sender, DisconnectArgs e)
			{
				QuitMatch(e.Reason);
			};
			onlineMatchMessenger.OnPlayerMovement += OnlinePlayerMovement;
			activeChat = UnityEngine.Object.Instantiate(chatPrefab);
			activeChat.MessageSent += LocalChatMessageSent;
			if (matchState.InRace)
			{
				joiningRaceInProgress = true;
				GoToStage();
			}
			else
			{
				GoToLobby();
			}
		}

		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			messenger.CreateListener<SettingsChangedMessage>(SettingsChangedCallback);
			messenger.CreateListener<ClientJoinedMessage>(ClientJoinedCallback);
			messenger.CreateListener<ClientLeftMessage>(ClientLeftCallback);
			messenger.CreateListener<PlayerJoinedMessage>(PlayerJoinedCallback);
			messenger.CreateListener<PlayerLeftMessage>(PlayerLeftCallback);
			messenger.CreateListener<CharacterChangedMessage>(CharacterChangedCallback);
			messenger.CreateListener<ChangedReadyMessage>(ChangedReadyCallback);
			messenger.CreateListener<LoadRaceMessage>(LoadRaceCallback);
			messenger.CreateListener<ChatMessage>(ChatCallback);
			messenger.CreateListener<LoadLobbyMessage>(LoadLobbyCallback);
			messenger.CreateListener<AutoStartTimerMessage>(AutoStartTimerCallback);
			myGuid = Guid.NewGuid();
			messenger.SendMessage(new ClientJoinedMessage(myGuid, ActiveData.GameSettings.nickname));
		}

		private void LocalChatMessageSent(object sender, ChatMessageArgs args)
		{
			MatchClient matchClient = clients.FirstOrDefault((MatchClient a) => a.Guid == myGuid);
			messenger.SendMessage(new ChatMessage(matchClient.Name, ChatMessageType.User, args.Text));
		}

		private void OnlinePlayerMovement(object sender, PlayerMovementArgs e)
		{
			MatchPlayer matchPlayer = players.FirstOrDefault((MatchPlayer a) => a.ClientGuid == e.Movement.ClientGuid && a.CtrlType == e.Movement.CtrlType);
			if (matchPlayer != null && matchPlayer.BallObject != null)
			{
				matchPlayer.ProcessMovement(e.Timestamp, e.Movement);
			}
		}

		private void Update()
		{
			messenger.UpdateListeners();
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
			{
				if (!PauseMenu.GamePaused)
				{
					PauseMenu pauseMenu = UnityEngine.Object.Instantiate(pauseMenuPrefab);
					pauseMenu.OnlineMode = OnlineMode;
				}
				else
				{
					PauseMenu pauseMenu2 = UnityEngine.Object.FindObjectOfType<PauseMenu>();
					if ((bool)pauseMenu2)
					{
						UnityEngine.Object.Destroy(pauseMenu2.gameObject);
					}
				}
			}
			if (lobbyTimerOn && inLobby)
			{
				lobbyTimer -= Time.deltaTime;
				LobbyReferences.Active.CountdownField.text = "Match starts in " + Mathf.Max(1f, Mathf.Ceil(lobbyTimer));
				if (lobbyTimer <= 0f && !OnlineMode)
				{
					messenger.SendMessage(new LoadRaceMessage());
				}
			}
			if (autoStartTimerOn && inLobby)
			{
				autoStartTimer = Mathf.Max(0f, autoStartTimer - Time.deltaTime);
			}
			if (!OnlineMode)
			{
				return;
			}
			netUpdateTimer -= Time.deltaTime;
			if (!(netUpdateTimer <= 0f))
			{
				return;
			}
			netUpdateTimer = 0.025f;
			foreach (MatchPlayer player in players)
			{
				if (player.ClientGuid == myGuid && (bool)player.BallObject)
				{
					((OnlineMatchMessenger)messenger).SendPlayerMovement(player);
				}
			}
		}

		public void OnDestroy()
		{
			messenger.Close();
			if ((bool)activeChat)
			{
				UnityEngine.Object.Destroy(activeChat.gameObject);
			}
		}

		private void StartLobbyTimer(float offset = 0f)
		{
			lobbyTimerOn = true;
			lobbyTimer -= offset;
			LobbyReferences.Active.CountdownField.enabled = true;
		}

		private void StopLobbyTimer()
		{
			lobbyTimerOn = false;
			lobbyTimer = 3f;
			LobbyReferences.Active.CountdownField.enabled = false;
		}

		private void GoToLobby()
		{
			if (!inLobby)
			{
				loadingStage = false;
				loadingLobby = true;
				SceneManager.LoadScene(lobbySceneName);
			}
		}

		private void GoToStage()
		{
			StageInfo stageInfo = ActiveData.Stages[currentSettings.StageId];
			loadingStage = true;
			loadingLobby = false;
			foreach (MatchPlayer player in Players)
			{
				player.ReadyToRace = false;
			}
			SceneManager.LoadScene(stageInfo.sceneName);
		}

		private void OnLevelWasLoaded(int level)
		{
			if (loadingLobby)
			{
				InitLobby();
				loadingLobby = false;
			}
			if (loadingStage)
			{
				InitRace();
				loadingStage = false;
			}
		}

		private void InitLobby()
		{
			inLobby = true;
			foreach (MatchPlayer player in Players)
			{
				SpawnLobbyBall(player);
			}
			if (showSettingsOnLobbyLoad)
			{
				LobbyReferences.Active.MatchSettingsPanel.Show();
				showSettingsOnLobbyLoad = false;
			}
		}

		private void InitRace()
		{
			inLobby = false;
			RaceManager raceManager = UnityEngine.Object.Instantiate(raceManagerPrefab);
			raceManager.Init(currentSettings, this, messenger, joiningRaceInProgress);
			joiningRaceInProgress = false;
		}

		public void QuitMatch(string reason = null)
		{
			StartCoroutine(QuitMatchInternal(reason));
		}

		private IEnumerator QuitMatchInternal(string reason)
		{
			SceneManager.LoadScene("Menu");
			if (reason != null)
			{
				yield return null;
				UnityEngine.Object.FindObjectOfType<PopupHandler>().OpenPopup(disconnectedPopupPrefab);
				UnityEngine.Object.FindObjectOfType<PopupDisconnected>().Reason = reason;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void SpawnLobbyBall(MatchPlayer player)
		{
			LobbyBallSpawner ballSpawner = LobbyReferences.Active.BallSpawner;
			if (player.BallObject != null)
			{
				player.BallObject.CreateRemovalParticles();
				UnityEngine.Object.Destroy(player.BallObject.gameObject);
			}
			string text = clients.First((MatchClient a) => a.Guid == player.ClientGuid).Name + " (" + GameInput.GetControlTypeName(player.CtrlType) + ")";
			player.BallObject = ballSpawner.SpawnBall(PlayerType.Normal, (!(player.ClientGuid == myGuid)) ? ControlType.None : player.CtrlType, player.CharacterId, text);
			if (player.ClientGuid != myGuid)
			{
				Marker marker = UnityEngine.Object.Instantiate(markerPrefab);
				marker.transform.SetParent(LobbyReferences.Active.MarkerContainer, false);
				marker.Color = Color.clear;
				marker.Text = text;
				marker.Target = player.BallObject.transform;
			}
		}
	}
}
