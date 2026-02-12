using System.Collections.Generic;
using System.Linq;
using Sanicball.Logic;
using UnityEngine;

namespace Sanicball.UI
{
	public class Scoreboard : MonoBehaviour
	{
		[SerializeField]
		private ScoreboardEntry entryPrefab;

		[SerializeField]
		private RectTransform entryContainer;

		[SerializeField]
		private SlideCanvasGroup slide;

		private bool slideShouldOpen;

		private List<ScoreboardEntry> activeEntries = new List<ScoreboardEntry>();

		private void Update()
		{
			if (slideShouldOpen && !slide.isOpen)
			{
				slide.Open();
			}
		}

		public void DisplayResults(RaceManager manager)
		{
			slideShouldOpen = true;
			for (int i = 0; i < manager.PlayerCount; i++)
			{
				if (!activeEntries.Any((ScoreboardEntry a) => a.Player == manager[i]) && manager[i].RaceFinished && !manager[i].FinishReport.Disqualified)
				{
					ScoreboardEntry scoreboardEntry = Object.Instantiate(entryPrefab);
					scoreboardEntry.transform.SetParent(entryContainer, false);
					scoreboardEntry.Init(manager[i]);
					activeEntries.Add(scoreboardEntry);
				}
			}
		}
	}
}
