using System;
using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class PlayerUI : MonoBehaviour
	{
		[SerializeField]
		private RectTransform fieldContainer;

		[SerializeField]
		private Text speedField;

		[SerializeField]
		private Text speedFieldLabel;

		[SerializeField]
		private Text lapField;

		[SerializeField]
		private Text timeField;

		[SerializeField]
		private Text checkpointTimeField;

		[SerializeField]
		private Text checkpointTimeDiffField;

		[SerializeField]
		private AudioClip checkpointSound;

		[SerializeField]
		private AudioClip respawnSound;

		[SerializeField]
		private Marker markerPrefab;

		[SerializeField]
		private RectTransform markerContainer;

		private Marker checkpointMarker;

		private List<Marker> playerMarkers = new List<Marker>();

		private RacePlayer targetPlayer;

		private RaceManager targetManager;

		private readonly Color finishedColor = new Color(0f, 0.5f, 1f);

		public RacePlayer TargetPlayer
		{
			get
			{
				return targetPlayer;
			}
			set
			{
				if (targetPlayer != null)
				{
					targetPlayer.NextCheckpointPassed -= TargetPlayer_NextCheckpointPassed;
					targetPlayer.Respawned -= TargetPlayer_Respawned;
					UnityEngine.Object.Destroy(checkpointMarker.gameObject);
					foreach (Marker playerMarker in playerMarkers)
					{
						UnityEngine.Object.Destroy(playerMarker.gameObject);
					}
				}
				targetPlayer = value;
				targetPlayer.NextCheckpointPassed += TargetPlayer_NextCheckpointPassed;
				targetPlayer.Respawned += TargetPlayer_Respawned;
				checkpointMarker = UnityEngine.Object.Instantiate(markerPrefab);
				checkpointMarker.transform.SetParent(markerContainer, false);
				checkpointMarker.Text = "Checkpoint";
				checkpointMarker.Clamp = true;
				for (int i = 0; i < TargetManager.PlayerCount; i++)
				{
					RacePlayer racePlayer = TargetManager[i];
					if (racePlayer != TargetPlayer)
					{
						Marker marker = UnityEngine.Object.Instantiate(markerPrefab);
						marker.transform.SetParent(markerContainer, false);
						marker.Text = racePlayer.Name;
						marker.Target = racePlayer.Transform;
						marker.Clamp = false;
						Sanicball.Data.CharacterInfo characterInfo = ActiveData.Characters[racePlayer.Character];
						Color color = characterInfo.color;
						color.a = 0.2f;
						marker.Color = color;
						playerMarkers.Add(marker);
					}
				}
			}
		}

		public RaceManager TargetManager
		{
			get
			{
				return targetManager;
			}
			set
			{
				targetManager = value;
			}
		}

		public Camera TargetCamera { get; set; }

		private void TargetPlayer_Respawned(object sender, EventArgs e)
		{
			UISound.Play(respawnSound);
			checkpointTimeField.text = "Respawn lap time penalty";
			checkpointTimeField.GetComponent<ToggleCanvasGroup>().ShowTemporarily(2f);
			checkpointTimeDiffField.color = Color.red;
			checkpointTimeDiffField.text = "+" + Utils.GetTimeString(TimeSpan.FromSeconds(5.0));
			checkpointTimeDiffField.GetComponent<ToggleCanvasGroup>().ShowTemporarily(2f);
		}

		private void TargetPlayer_NextCheckpointPassed(object sender, NextCheckpointPassArgs e)
		{
			UISound.Play(checkpointSound);
			checkpointTimeField.text = Utils.GetTimeString(e.CurrentLapTime);
			checkpointTimeField.GetComponent<ToggleCanvasGroup>().ShowTemporarily(2f);
			if (!TargetPlayer.LapRecordsEnabled)
			{
				return;
			}
			CharacterTier tier = ActiveData.Characters[targetPlayer.Character].tier;
			string sceneName = SceneManager.GetActiveScene().name;
			int stage = ActiveData.Stages.Where((StageInfo a) => a.sceneName == sceneName).First().id;
			float num = (float)e.CurrentLapTime.TotalSeconds;
			RaceRecord raceRecord = (from a in ActiveData.RaceRecords
				where a.Tier == tier && a.Stage == stage && a.GameVersion == 0.82f && !a.WasTesting
				orderby a.Time
				select a).FirstOrDefault();
			if (raceRecord != null)
			{
				float num2 = num - raceRecord.CheckpointTimes[e.IndexOfPreviousCheckpoint];
				bool flag = num2 < 0f;
				TimeSpan timeToUse = TimeSpan.FromSeconds(Mathf.Abs(num2));
				checkpointTimeDiffField.text = ((!flag) ? "+" : "-") + Utils.GetTimeString(timeToUse);
				checkpointTimeDiffField.color = ((!flag) ? Color.red : Color.blue);
				checkpointTimeDiffField.GetComponent<ToggleCanvasGroup>().ShowTemporarily(2f);
				if (e.IndexOfPreviousCheckpoint == StageReferences.Active.checkpoints.Length - 1 && flag)
				{
					checkpointTimeDiffField.text = "New lap record!";
				}
			}
			else if (e.IndexOfPreviousCheckpoint == StageReferences.Active.checkpoints.Length - 1)
			{
				checkpointTimeDiffField.text = "Lap record set!";
				checkpointTimeDiffField.color = Color.blue;
				checkpointTimeDiffField.GetComponent<ToggleCanvasGroup>().ShowTemporarily(2f);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if ((bool)TargetCamera)
			{
				fieldContainer.anchorMin = TargetCamera.rect.min;
				fieldContainer.anchorMax = TargetCamera.rect.max;
			}
			if (TargetPlayer == null || TargetManager == null)
			{
				return;
			}
			float num = TargetPlayer.Speed;
			string text = " ";
			if (!ActiveData.GameSettings.useImperial)
			{
				text += ((Mathf.Floor(num) != 1f) ? "fasts/h" : "fast/h");
			}
			else
			{
				num *= 0.62f;
				text += ((Mathf.Floor(num) != 1f) ? "lightspeeds" : "lightspeed");
				speedFieldLabel.fontSize = 62;
			}
			int num2 = 96;
			int num3 = 150;
			float num4 = (float)num3 - (float)(num3 - num2) * Mathf.Exp((0f - num) * 0.02f);
			speedField.fontSize = (int)num4;
			speedField.text = Mathf.Floor(num).ToString();
			speedFieldLabel.text = text;
			if (!TargetPlayer.RaceFinished)
			{
				lapField.text = "Lap " + TargetPlayer.Lap + "/" + TargetManager.Settings.Laps;
			}
			else if (TargetPlayer.FinishReport.Disqualified)
			{
				lapField.text = "Disqualified";
				lapField.color = Color.red;
			}
			else
			{
				lapField.text = "Race finished";
				lapField.color = finishedColor;
			}
			TimeSpan timeToUse = TargetManager.RaceTime;
			if (TargetPlayer.FinishReport != null)
			{
				timeToUse = TargetPlayer.FinishReport.Time;
				timeField.color = finishedColor;
			}
			timeField.text = Utils.GetTimeString(timeToUse);
			if (TargetPlayer.Timeout > 0f)
			{
				Text text2 = timeField;
				text2.text = text2.text + Environment.NewLine + "<b>Timeout</b> " + Utils.GetTimeString(TimeSpan.FromSeconds(TargetPlayer.Timeout));
			}
			if (TargetPlayer.NextCheckpoint != null)
			{
				checkpointMarker.Target = TargetPlayer.NextCheckpoint.transform;
			}
			else
			{
				checkpointMarker.Target = null;
			}
			checkpointMarker.CameraToUse = TargetCamera;
			playerMarkers.RemoveAll((Marker a) => a == null);
			foreach (Marker item in playerMarkers.ToList())
			{
				item.CameraToUse = TargetCamera;
			}
		}
	}
}
