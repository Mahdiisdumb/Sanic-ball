using System.Collections.Generic;
using System.Linq;
using Sanicball.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class ClientListEntry : MonoBehaviour
	{
		[SerializeField]
		private Text nameField;

		[SerializeField]
		private Text playerCountField;

		public void FillFields(MatchClient client, MatchManager manager)
		{
			nameField.text = client.Name;
			List<MatchPlayer> source = manager.Players.Where((MatchPlayer a) => a.ClientGuid == client.Guid).ToList();
			int num = source.Count();
			int num2 = source.Count((MatchPlayer a) => a.ReadyToRace);
			if (num == 0)
			{
				playerCountField.text = "Spectating";
				return;
			}
			playerCountField.text = num2 + "/" + num + " ready";
		}
	}
}
