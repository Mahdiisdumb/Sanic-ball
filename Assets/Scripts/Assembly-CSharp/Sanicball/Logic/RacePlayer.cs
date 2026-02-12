using System;
using System.Linq;
using Sanicball.Data;
using Sanicball.Gameplay;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sanicball.Logic
{
	[Serializable]
	public class RacePlayer
	{
		private Ball ball;

		private IBallCamera ballCamera;

		private RaceFinishReport finishReport;

		private int lap;

		private int currentCheckpointIndex;

		private Vector3 currentCheckpointPos;

		private Checkpoint nextCheckpoint;

		private float lapTime;

		private float[] checkpointTimes;

		private float timeout;

		private StageReferences sr;

		private MatchMessenger matchMessenger;

		private MatchPlayer associatedMatchPlayer;

		private bool waitingForCheckpointMessage;

		public Ball Ball
		{
			get
			{
				return ball;
			}
		}

		public bool IsPlayer
		{
			get
			{
				return ball.Type == BallType.Player;
			}
		}

		public string Name
		{
			get
			{
				return ball.Nickname;
			}
		}

		public ControlType CtrlType
		{
			get
			{
				return ball.CtrlType;
			}
		}

		public int Character
		{
			get
			{
				return ball.CharacterId;
			}
		}

		public Transform Transform
		{
			get
			{
				return ball.transform;
			}
		}

		public float Speed
		{
			get
			{
				return ball.GetComponent<Rigidbody>().velocity.magnitude;
			}
		}

		public IBallCamera Camera
		{
			get
			{
				return ballCamera;
			}
		}

		public int Lap
		{
			get
			{
				return lap;
			}
		}

		public bool RaceFinished
		{
			get
			{
				return finishReport != null;
			}
		}

		public RaceFinishReport FinishReport
		{
			get
			{
				return finishReport;
			}
		}

		public float Timeout
		{
			get
			{
				return timeout;
			}
		}

		public MatchPlayer AssociatedMatchPlayer
		{
			get
			{
				return associatedMatchPlayer;
			}
		}

		public bool LapRecordsEnabled { get; set; }

		public int Position { get; set; }

		public Checkpoint NextCheckpoint
		{
			get
			{
				return nextCheckpoint;
			}
		}

		public event EventHandler<NextCheckpointPassArgs> NextCheckpointPassed;

		public event EventHandler Respawned;

		public event EventHandler FinishLinePassed;

		public event EventHandler Destroyed;

		public RacePlayer(Ball ball, MatchMessenger matchMessenger, MatchPlayer associatedMatchPlayer)
		{
			sr = StageReferences.Active;
			this.matchMessenger = matchMessenger;
			this.associatedMatchPlayer = associatedMatchPlayer;
			matchMessenger.CreateListener<CheckpointPassedMessage>(CheckpointPassedHandler);
			matchMessenger.CreateListener<RaceTimeoutMessage>(RaceTimeoutHandler);
			lap = 1;
			ball.CanMove = false;
			ball.AutoBrake = true;
			ball.CheckpointPassed += Ball_CheckpointPassed;
			ball.RespawnRequested += Ball_RespawnRequested;
			currentCheckpointPos = sr.checkpoints[0].transform.position;
			this.ball = ball;
			ball.CameraCreated += delegate(object sender, CameraCreationArgs e)
			{
				ballCamera = e.CameraCreated;
				ballCamera.SetDirection(sr.checkpoints[0].transform.rotation);
			};
			checkpointTimes = new float[StageReferences.Active.checkpoints.Length];
			SetNextCheckpoint();
		}

		public void StartRace()
		{
			ball.CanMove = true;
			ball.AutoBrake = false;
		}

		public void FinishRace(RaceFinishReport report)
		{
			if (finishReport == null)
			{
				finishReport = report;
				if (ball.Type == BallType.AI)
				{
					ball.CanMove = false;
				}
				ball.gameObject.layer = LayerMask.NameToLayer("Racer Ghost");
				return;
			}
			throw new InvalidOperationException("RacePlayer tried to finish a race twice for some reason");
		}

		public float CalculateRaceProgress()
		{
			if (RaceFinished)
			{
				return Lap;
			}
			float num = 1f / (float)sr.checkpoints.Length;
			Vector3 position = nextCheckpoint.transform.position;
			float num2 = Vector3.Distance(ball.transform.position, position);
			float num3 = Vector3.Distance(currentCheckpointPos, position);
			float num4 = 1f - Mathf.Clamp(num2 / num3, 0f, 1f);
			float num5 = num4 * num;
			float num6 = (float)currentCheckpointIndex * num + num5;
			return (float)Lap + num6;
		}

		private void Ball_CheckpointPassed(object sender, CheckpointPassArgs e)
		{
			if (!(e.CheckpointPassed == nextCheckpoint))
			{
				return;
			}
			if (ball.Type == BallType.Player && ball.CtrlType != ControlType.None)
			{
				if (!waitingForCheckpointMessage)
				{
					waitingForCheckpointMessage = true;
					matchMessenger.SendMessage(new CheckpointPassedMessage(associatedMatchPlayer.ClientGuid, ball.CtrlType, lapTime));
				}
			}
			else if (ball.Type == BallType.AI)
			{
				PassNextCheckpoint(lapTime);
			}
		}

		private void CheckpointPassedHandler(CheckpointPassedMessage msg, float travelTime)
		{
			if (associatedMatchPlayer != null && msg.ClientGuid == associatedMatchPlayer.ClientGuid && msg.CtrlType == associatedMatchPlayer.CtrlType)
			{
				PassNextCheckpoint(msg.LapTime);
				waitingForCheckpointMessage = false;
			}
		}

		private void RaceTimeoutHandler(RaceTimeoutMessage msg, float travelTime)
		{
			if (associatedMatchPlayer != null && msg.ClientGuid == associatedMatchPlayer.ClientGuid && msg.CtrlType == associatedMatchPlayer.CtrlType)
			{
				timeout = msg.Time - travelTime;
			}
		}

		private void PassNextCheckpoint(float lapTime)
		{
			checkpointTimes[currentCheckpointIndex] = lapTime;
			if (this.NextCheckpointPassed != null)
			{
				this.NextCheckpointPassed(this, new NextCheckpointPassArgs(currentCheckpointIndex, TimeSpan.FromSeconds(lapTime)));
			}
			currentCheckpointIndex = (currentCheckpointIndex + 1) % sr.checkpoints.Length;
			currentCheckpointPos = nextCheckpoint.transform.position;
			if (currentCheckpointIndex == 0)
			{
				lap++;
				if (this.FinishLinePassed != null)
				{
					this.FinishLinePassed(this, EventArgs.Empty);
				}
				if (LapRecordsEnabled)
				{
					CharacterTier tier = ActiveData.Characters[Character].tier;
					string sceneName = SceneManager.GetActiveScene().name;
					int id = ActiveData.Stages.Where((StageInfo a) => a.sceneName == sceneName).First().id;
					ActiveData.RaceRecords.Add(new RaceRecord(tier, lapTime, DateTime.Now, id, Character, checkpointTimes, 0.82f, false));
					Debug.Log(string.Concat("Saved lap record (", TimeSpan.FromSeconds(lapTime), ")"));
				}
				this.lapTime = 0f + (this.lapTime - lapTime);
				checkpointTimes = new float[StageReferences.Active.checkpoints.Length];
			}
			SetNextCheckpoint();
			TrySetAITarget();
		}

		private void Ball_RespawnRequested(object sender, EventArgs e)
		{
			if (this.Respawned != null)
			{
				this.Respawned(this, EventArgs.Empty);
			}
			ball.transform.position = sr.checkpoints[currentCheckpointIndex].GetRespawnPoint() + Vector3.up * ball.transform.localScale.x * 0.5f;
			ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			if (ballCamera != null)
			{
				ballCamera.SetDirection(sr.checkpoints[currentCheckpointIndex].transform.rotation);
			}
			lapTime += 5f;
			TrySetAITarget();
		}

		private void TrySetAITarget()
		{
			BallControlAI component = ball.GetComponent<BallControlAI>();
			if ((bool)component)
			{
				Checkpoint checkpoint = StageReferences.Active.checkpoints[currentCheckpointIndex];
				component.Target = checkpoint.FirstAINode;
			}
		}

		private void SetNextCheckpoint()
		{
			if (RaceFinished)
			{
				nextCheckpoint = null;
			}
			else if (currentCheckpointIndex == sr.checkpoints.Length - 1)
			{
				nextCheckpoint = sr.checkpoints[0];
			}
			else
			{
				nextCheckpoint = sr.checkpoints[currentCheckpointIndex + 1];
			}
		}

		public void UpdateTimer(float dt)
		{
			lapTime += dt;
			if (timeout > 0f)
			{
				timeout = Mathf.Max(0f, timeout - Time.deltaTime);
			}
		}

		public void Destroy()
		{
			matchMessenger.RemoveListener<CheckpointPassedMessage>(CheckpointPassedHandler);
			if (this.Destroyed != null)
			{
				this.Destroyed(this, EventArgs.Empty);
			}
		}
	}
}
