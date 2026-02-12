using System.Collections.Generic;
using System.Linq;
using Sanicball.Logic;
using Sanicball.UI;
using UnityEngine;

namespace Sanicball
{
	public class EndOfMatch : MonoBehaviour
	{
		[SerializeField]
		private Transform[] topPositionSpawnpoints;

		[SerializeField]
		private Transform lowerPositionsSpawnpoint;

		[SerializeField]
		private Scoreboard scoreboardPrefab;

		[SerializeField]
		private Camera cam;

		[SerializeField]
		private Rotate camRotate;

		private Scoreboard activeScoreboard;

		private bool hasActivatedOnce;

		private List<RacePlayer> movedPlayers = new List<RacePlayer>();

		public void Activate(RaceManager manager)
		{
			if (!hasActivatedOnce)
			{
				CameraFade.StartAlphaFade(Color.black, false, 1f, 0f, delegate
				{
					CameraFade.StartAlphaFade(Color.black, true, 1f);
					ActivateInner(manager);
				});
			}
			else
			{
				ActivateInner(manager);
			}
		}

		private void ActivateInner(RaceManager manager)
		{
			if (!hasActivatedOnce)
			{
				hasActivatedOnce = true;
				activeScoreboard = Object.Instantiate(scoreboardPrefab);
				for (int i = 0; i < manager.PlayerCount; i++)
				{
					RacePlayer racePlayer = manager[i];
					if (racePlayer.Camera != null)
					{
						racePlayer.Camera.Remove();
					}
				}
				RaceUI[] array = Object.FindObjectsOfType<RaceUI>();
				foreach (RaceUI raceUI in array)
				{
					Object.Destroy(raceUI.gameObject);
				}
				PlayerUI[] array2 = Object.FindObjectsOfType<PlayerUI>();
				foreach (PlayerUI playerUI in array2)
				{
					Object.Destroy(playerUI.gameObject);
				}
				cam.gameObject.SetActive(true);
				camRotate.angle = new Vector3(0f, 1f, 0f);
			}
			activeScoreboard.DisplayResults(manager);
			RacePlayer[] array3 = (from a in manager.Players
				where a.RaceFinished && !a.FinishReport.Disqualified
				orderby a.FinishReport.Position
				select a).ToArray();
			for (int num = 0; num < array3.Length; num++)
			{
				Vector3 position = lowerPositionsSpawnpoint.position;
				if (num < topPositionSpawnpoints.Length)
				{
					position = topPositionSpawnpoints[num].position;
				}
				RacePlayer racePlayer2 = array3[num];
				if (!movedPlayers.Contains(racePlayer2))
				{
					racePlayer2.Ball.transform.position = position;
					racePlayer2.Ball.transform.rotation = base.transform.rotation;
					racePlayer2.Ball.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere * 0.5f;
					racePlayer2.Ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, Random.Range(-50f, 50f));
					racePlayer2.Ball.CanMove = false;
					racePlayer2.Ball.gameObject.layer = LayerMask.NameToLayer("Racer");
					movedPlayers.Add(racePlayer2);
				}
			}
		}
	}
}
