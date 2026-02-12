using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(ToggleCanvasGroup))]
	public class ScoreboardEntry : MonoBehaviour
	{
		[SerializeField]
		private Text positionField;

		[SerializeField]
		private Image iconField;

		[SerializeField]
		private Text nameField;

		[SerializeField]
		private Text timeField;

		public RacePlayer Player { get; private set; }

		public void Init(RacePlayer player)
		{
			Player = player;
			GetComponent<ToggleCanvasGroup>().Show();
			RaceFinishReport finishReport = player.FinishReport;
			if (finishReport != null)
			{
				if (finishReport.Position != -1)
				{
					positionField.text = Utils.GetPosString(finishReport.Position);
				}
				else
				{
					positionField.color = Color.red;
					positionField.text = "DSQ";
				}
				timeField.text = Utils.GetTimeString(finishReport.Time);
			}
			else
			{
				positionField.text = "???";
				timeField.text = "Still racing";
			}
			iconField.sprite = ActiveData.Characters[player.Character].icon;
			nameField.text = player.Name;
		}
	}
}
