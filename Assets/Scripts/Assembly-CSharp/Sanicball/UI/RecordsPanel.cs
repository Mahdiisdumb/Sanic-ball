using System;
using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class RecordsPanel : MonoBehaviour
	{
		public Text stageNameField;

		public RecordTypeControl lapRecord;

		public RecordTypeControl hyperspeedLapRecord;

		public RecordTypeControl recordTypeControlPrefab;

		public RectTransform sectionHeaderPrefab;

		public RectTransform recordTypeContainer;

		private List<RecordTypeControl> recordTypes = new List<RecordTypeControl>();

		private int selectedStage;

		public void IncrementStage()
		{
			selectedStage++;
			if (selectedStage >= ActiveData.Stages.Length)
			{
				selectedStage = 0;
			}
			UpdateStageName();
		}

		public void DecrementStage()
		{
			selectedStage--;
			if (selectedStage < 0)
			{
				selectedStage = ActiveData.Stages.Length - 1;
			}
			UpdateStageName();
		}

		private void Start()
		{
			foreach (CharacterTier item in Enum.GetValues(typeof(CharacterTier)).Cast<CharacterTier>())
			{
				RectTransform rectTransform = UnityEngine.Object.Instantiate(sectionHeaderPrefab);
				rectTransform.GetComponentInChildren<Text>().text = item.ToString() + " balls";
				rectTransform.SetParent(recordTypeContainer, false);
				RecordTypeControl recordTypeControl = UnityEngine.Object.Instantiate(recordTypeControlPrefab);
				recordTypeControl.transform.SetParent(recordTypeContainer, false);
				recordTypeControl.titleField.text = "Lap record";
				recordTypes.Add(recordTypeControl);
			}
			UpdateFields();
			UpdateStageName();
		}

		private void UpdateFields()
		{
			IOrderedEnumerable<RaceRecord> source = from a in ActiveData.RaceRecords
				where a.Stage == selectedStage && a.GameVersion == 0.82f && !a.WasTesting
				orderby a.Time
				select a;
			int i;
			for (i = 0; i < recordTypes.Count(); i++)
			{
				RecordTypeControl recordTypeControl = recordTypes[i];
				RaceRecord record = source.Where((RaceRecord a) => a.Tier == (CharacterTier)i).FirstOrDefault();
				recordTypeControl.SetRecord(record);
			}
		}

		private void UpdateStageName()
		{
			stageNameField.text = ActiveData.Stages[selectedStage].name;
			UpdateFields();
		}
	}
}
