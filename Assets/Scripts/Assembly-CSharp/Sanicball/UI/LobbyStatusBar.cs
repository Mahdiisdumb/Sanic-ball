using System;
using System.Collections.Generic;
using Sanicball.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class LobbyStatusBar : MonoBehaviour
	{
		[SerializeField]
		private Text leftText;

		[SerializeField]
		private Text rightText;

		[SerializeField]
		private RectTransform clientList;

		[SerializeField]
		private ClientListEntry clientListEntryPrefab;

		private List<ClientListEntry> curClientListEntries = new List<ClientListEntry>();

		private MatchManager manager;

		private void Start()
		{
			manager = UnityEngine.Object.FindObjectOfType<MatchManager>();
			if (!manager.OnlineMode)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				UpdateText();
			}
		}

		private void UpdateText()
		{
			if (!manager)
			{
				return;
			}
			int count = manager.Clients.Count;
			int count2 = manager.Players.Count;
			if (manager.AutoStartTimerOn)
			{
				leftText.text = "Match will start in " + GetTimeString(TimeSpan.FromSeconds(manager.AutoStartTimer)) + ", or when all players are ready.";
			}
			else if (manager.Players.Count > 0)
			{
				leftText.text = "Match starts when all players are ready.";
			}
			else
			{
				leftText.text = "Match will not start without players.";
			}
			rightText.text = count + " " + ((count == 1) ? "client" : "clients") + " - " + count2 + " " + ((count2 == 1) ? "player" : "players");
			foreach (ClientListEntry curClientListEntry in curClientListEntries)
			{
				UnityEngine.Object.Destroy(curClientListEntry.gameObject);
			}
			curClientListEntries.Clear();
			foreach (MatchClient client in manager.Clients)
			{
				ClientListEntry clientListEntry = UnityEngine.Object.Instantiate(clientListEntryPrefab);
				clientListEntry.transform.SetParent(clientList, false);
				clientListEntry.FillFields(client, manager);
				curClientListEntries.Add(clientListEntry);
			}
		}

		private void Update()
		{
			UpdateText();
		}

		private string GetTimeString(TimeSpan timeToUse)
		{
			return string.Format("{0:00}:{1:00}", timeToUse.Minutes, timeToUse.Seconds);
		}
	}
}
