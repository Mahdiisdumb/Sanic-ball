using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sanicball.Data;
using Sanicball.Gameplay;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;

namespace Sanicball.Logic
{
	public class RaceManager : MonoBehaviour
	{
		[SerializeField]
		private WaitingCamera waitingCamPrefab;

		[SerializeField]
		private WaitingUI waitingUIPrefab;

		[SerializeField]
		private RaceCountdown raceCountdownPrefab;

		[SerializeField]
		private PlayerUI playerUIPrefab;

		[SerializeField]
		private RaceUI raceUIPrefab;

		[SerializeField]
		private SpectatorView spectatorViewPrefab;

		private List<RacePlayer> players = new List<RacePlayer>();

		private RaceState currentState;

		private MatchSettings settings;

		private MatchManager matchManager;

		private MatchMessenger messenger;

		private WaitingCamera activeWaitingCam;

		private WaitingUI activeWaitingUI;

		private double raceTimer;

		private bool raceTimerOn;

		private RaceUI raceUI;

		private float countdownOffset;

		private bool joinedWhileRaceInProgress;

		public TimeSpan RaceTime
		{
			get
			{
				return TimeSpan.FromSeconds(raceTimer);
			}
		}

		public MatchSettings Settings
		{
			get
			{
				return settings;
			}
		}

		public int PlayerCount
		{
			get
			{
				return players.Count;
			}
		}

		public RacePlayer this[int playerIndex]
		{
			get
			{
				return players[playerIndex];
			}
		}

		public ReadOnlyCollection<RacePlayer> Players
		{
			get
			{
				return new ReadOnlyCollection<RacePlayer>(players);
			}
		}

		private RaceState CurrentState
		{
			get
			{
				return currentState;
			}
			set
			{
				RaceState raceState = currentState;
				if (raceState == RaceState.Waiting)
				{
					if ((bool)activeWaitingCam)
					{
						UnityEngine.Object.Destroy(activeWaitingCam.gameObject);
					}
					if ((bool)activeWaitingUI)
					{
						UnityEngine.Object.Destroy(activeWaitingUI.gameObject);
					}
				}
				switch (value)
				{
				case RaceState.Waiting:
					activeWaitingCam = UnityEngine.Object.Instantiate(waitingCamPrefab);
					activeWaitingUI = UnityEngine.Object.Instantiate(waitingUIPrefab);
					activeWaitingUI.StageNameToShow = ActiveData.Stages[settings.StageId].name;
					if (matchManager.OnlineMode)
					{
						if (joinedWhileRaceInProgress)
						{
							activeWaitingUI.InfoToShow = "Waiting for race to end.";
						}
						else
						{
							activeWaitingUI.InfoToShow = "Waiting for other players...";
						}
					}
					break;
				case RaceState.Countdown:
				{
					RaceCountdown raceCountdown = UnityEngine.Object.Instantiate(raceCountdownPrefab);
					raceCountdown.ApplyOffset(countdownOffset);
					raceCountdown.OnCountdownFinished += Countdown_OnCountdownFinished;
					raceUI = UnityEngine.Object.Instantiate(raceUIPrefab);
					raceUI.TargetManager = this;
					CreateBallObjects();
					if (!matchManager.Players.Any((MatchPlayer a) => a.ClientGuid == matchManager.LocalClientGuid))
					{
						SpectatorView spectatorView = UnityEngine.Object.Instantiate(spectatorViewPrefab);
						spectatorView.TargetManager = this;
						spectatorView.Target = players[0];
					}
					break;
				}
				case RaceState.Racing:
				{
					raceTimerOn = true;
					MusicPlayer musicPlayer = UnityEngine.Object.FindObjectOfType<MusicPlayer>();
					if ((bool)musicPlayer)
					{
						musicPlayer.Play();
					}
					foreach (RacePlayer player in players)
					{
						player.StartRace();
					}
					break;
				}
				}
				currentState = value;
			}
		}

		private void Countdown_OnCountdownFinished(object sender, EventArgs e)
		{
			CurrentState = RaceState.Racing;
		}

		public void Init(MatchSettings settings, MatchManager matchManager, MatchMessenger messenger, bool raceIsInProgress)
		{
			this.settings = settings;
			this.matchManager = matchManager;
			this.messenger = messenger;
			messenger.CreateListener<StartRaceMessage>(StartRaceCallback);
			messenger.CreateListener<ClientLeftMessage>(ClientLeftCallback);
			messenger.CreateListener<DoneRacingMessage>(DoneRacingCallback);
			if (raceIsInProgress)
			{
				Debug.Log("Starting race in progress");
				joinedWhileRaceInProgress = true;
				CreateBallObjects();
			}
		}

		private void StartRaceCallback(StartRaceMessage msg, float travelTime)
		{
			countdownOffset = travelTime;
			CurrentState = RaceState.Countdown;
		}

		private void ClientLeftCallback(ClientLeftMessage msg, float travelTime)
		{
			foreach (RacePlayer item in players.ToList())
			{
				if (item.AssociatedMatchPlayer != null && item.AssociatedMatchPlayer.ClientGuid == msg.ClientGuid)
				{
					item.Destroy();
					players.Remove(item);
				}
			}
		}

		private void CreateBallObjects()
		{
			int num = 0;
			RaceBallSpawner spawnPoint = StageReferences.Active.spawnPoint;
			bool flag = matchManager.Players.Count((MatchPlayer a) => a.ClientGuid == matchManager.LocalClientGuid) == 1;
			for (int num2 = 0; num2 < matchManager.Players.Count; num2++)
			{
				MatchPlayer matchPlayer = matchManager.Players[num2];
				bool flag2 = matchPlayer.ClientGuid == matchManager.LocalClientGuid;
				string text = matchManager.Clients.FirstOrDefault((MatchClient a) => a.Guid == matchPlayer.ClientGuid).Name;
				matchPlayer.BallObject = spawnPoint.SpawnBall(num, BallType.Player, (!flag2) ? ControlType.None : matchPlayer.CtrlType, matchPlayer.CharacterId, text + " (" + GameInput.GetControlTypeName(matchPlayer.CtrlType) + ")");
				RacePlayer racePlayer = new RacePlayer(matchPlayer.BallObject, messenger, matchPlayer);
				players.Add(racePlayer);
				racePlayer.LapRecordsEnabled = flag && flag2;
				racePlayer.FinishLinePassed += RacePlayer_FinishLinePassed;
				num++;
			}
			if (!matchManager.OnlineMode)
			{
				for (int num3 = 0; num3 < settings.AICount; num3++)
				{
					Ball ball = spawnPoint.SpawnBall(num, BallType.AI, ControlType.None, settings.GetAICharacter(num3), "AI #" + num3);
					ball.CanMove = false;
					RacePlayer racePlayer2 = new RacePlayer(ball, messenger, null);
					players.Add(racePlayer2);
					racePlayer2.FinishLinePassed += RacePlayer_FinishLinePassed;
					num++;
				}
			}
			int num4 = 0;
			foreach (RacePlayer item in players.Where((RacePlayer a) => a.CtrlType != ControlType.None))
			{
				PlayerUI playerUI = UnityEngine.Object.Instantiate(playerUIPrefab);
				playerUI.TargetManager = this;
				playerUI.TargetPlayer = item;
				int persistentIndex = num4;
				item.AssociatedMatchPlayer.BallObject.CameraCreated += delegate(object sender, CameraCreationArgs e)
				{
					playerUI.TargetCamera = e.CameraCreated.AttachedCamera;
					CameraSplitter component = e.CameraCreated.AttachedCamera.GetComponent<CameraSplitter>();
					if ((bool)component)
					{
						component.SplitscreenIndex = persistentIndex;
					}
				};
				num4++;
			}
		}

		private void RacePlayer_FinishLinePassed(object sender, EventArgs e)
		{
			RacePlayer racePlayer = (RacePlayer)sender;
			if (racePlayer.FinishReport != null || racePlayer.Lap <= settings.Laps)
			{
				return;
			}
			if (racePlayer.AssociatedMatchPlayer != null)
			{
				if (racePlayer.AssociatedMatchPlayer.ClientGuid == matchManager.LocalClientGuid)
				{
					messenger.SendMessage(new DoneRacingMessage(racePlayer.AssociatedMatchPlayer.ClientGuid, racePlayer.AssociatedMatchPlayer.CtrlType, raceTimer, false));
				}
			}
			else
			{
				DoneRacingInner(racePlayer, raceTimer, false);
			}
		}

		private void DoneRacingCallback(DoneRacingMessage msg, float travelTime)
		{
			RacePlayer rp = players.FirstOrDefault((RacePlayer a) => a.AssociatedMatchPlayer != null && a.AssociatedMatchPlayer.ClientGuid == msg.ClientGuid && a.AssociatedMatchPlayer.CtrlType == msg.CtrlType);
			DoneRacingInner(rp, msg.RaceTime, msg.Disqualified);
		}

		private void DoneRacingInner(RacePlayer rp, double raceTime, bool disqualified)
		{
			int position = players.IndexOf(rp) + 1;
			if (disqualified)
			{
				position = -1;
			}
			rp.FinishRace(new RaceFinishReport(position, TimeSpan.FromSeconds(raceTime)));
			if (!players.Any((RacePlayer a) => a.IsPlayer && !a.RaceFinished))
			{
				StageReferences.Active.endOfMatchHandler.Activate(this);
			}
		}

		private void Start()
		{
			if (joinedWhileRaceInProgress)
			{
				CurrentState = RaceState.Racing;
				SpectatorView spectatorView = UnityEngine.Object.Instantiate(spectatorViewPrefab);
				spectatorView.TargetManager = this;
				spectatorView.Target = players[0];
			}
			else
			{
				CurrentState = RaceState.Waiting;
			}
			if (matchManager.OnlineMode)
			{
				messenger.SendMessage(new StartRaceMessage());
			}
		}

		private void Update()
		{
			if (!matchManager.OnlineMode && CurrentState == RaceState.Waiting && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)))
			{
				messenger.SendMessage(new StartRaceMessage());
			}
			if (raceTimerOn)
			{
				raceTimer += Time.deltaTime;
				foreach (RacePlayer player in players)
				{
					player.UpdateTimer(Time.deltaTime);
				}
			}
			players = players.OrderByDescending((RacePlayer a) => a.CalculateRaceProgress()).ToList();
			for (int num = 0; num < players.Count; num++)
			{
				players[num].Position = num + 1;
			}
		}

		private void OnDestroy()
		{
			messenger.RemoveListener<StartRaceMessage>(StartRaceCallback);
			messenger.RemoveListener<ClientLeftMessage>(ClientLeftCallback);
			messenger.RemoveListener<DoneRacingMessage>(DoneRacingCallback);
			foreach (RacePlayer player in players)
			{
				player.Destroy();
			}
		}
	}
}
