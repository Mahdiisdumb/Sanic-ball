using System;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class RecordTypeControl : MonoBehaviour
	{
		public Text titleField;

		public Text timeField;

		public Text characterField;

		public Text dateField;

		public void SetRecord(RaceRecord r)
		{
			if (r != null)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(r.Time);
				timeField.color = new Color(10f / 51f, 10f / 51f, 10f / 51f);
				timeField.text = string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
				characterField.text = "Set with " + ActiveData.Characters[r.Character].name;
				dateField.text = r.Date.ToString();
			}
			else
			{
				timeField.text = "No records found";
				timeField.color = new Color(0.5f, 0.5f, 0.5f);
				characterField.text = string.Empty;
				dateField.text = string.Empty;
			}
		}
	}
}
