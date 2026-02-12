using System;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(ToggleCanvasGroup))]
	public class MatchSettingsPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject firstActive;

		[SerializeField]
		[Header("Fields")]
		private Text stage;

		[SerializeField]
		private Text laps;

		[SerializeField]
		private Text aiCount;

		[SerializeField]
		private Text aiSkill;

		[SerializeField]
		private Text[] aiCharacters;

		private MatchSettings tempSettings = MatchSettings.CreateDefault();

		public void Show()
		{
			ToggleCanvasGroup component = GetComponent<ToggleCanvasGroup>();
			component.Show();
			EventSystem.current.SetSelectedGameObject(firstActive);
		}

		public void Hide()
		{
			ToggleCanvasGroup component = GetComponent<ToggleCanvasGroup>();
			component.Hide();
		}

		public void RevertSettings()
		{
			MatchManager matchManager = UnityEngine.Object.FindObjectOfType<MatchManager>();
			if ((bool)matchManager)
			{
				tempSettings = matchManager.CurrentSettings;
			}
			UpdateUiFields();
		}

		public void SaveSettings()
		{
			MatchManager matchManager = UnityEngine.Object.FindObjectOfType<MatchManager>();
			if ((bool)matchManager)
			{
				matchManager.RequestSettingsChange(tempSettings);
				ActiveData.MatchSettings = tempSettings;
			}
		}

		public void DefaultSettings()
		{
			tempSettings = MatchSettings.CreateDefault();
			UpdateUiFields();
		}

		public void IncrementLaps()
		{
			if (tempSettings.Laps < 6)
			{
				tempSettings.Laps++;
			}
			else
			{
				tempSettings.Laps = 1;
			}
			UpdateUiFields();
		}

		public void DecrementLaps()
		{
			if (tempSettings.Laps > 1)
			{
				tempSettings.Laps--;
			}
			else
			{
				tempSettings.Laps = 6;
			}
			UpdateUiFields();
		}

		public void IncrementStage()
		{
			if (tempSettings.StageId < ActiveData.Stages.Length - 1)
			{
				tempSettings.StageId++;
			}
			else
			{
				tempSettings.StageId = 0;
			}
			UpdateUiFields();
		}

		public void DecrementStage()
		{
			if (tempSettings.StageId > 0)
			{
				tempSettings.StageId--;
			}
			else
			{
				tempSettings.StageId = ActiveData.Stages.Length - 1;
			}
			UpdateUiFields();
		}

		public void IncrementAICount()
		{
			if (tempSettings.AICount < 12)
			{
				tempSettings.AICount++;
			}
			else
			{
				tempSettings.AICount = 0;
			}
			UpdateUiFields();
		}

		public void DecrementAICount()
		{
			if (tempSettings.AICount > 0)
			{
				tempSettings.AICount--;
			}
			else
			{
				tempSettings.AICount = 12;
			}
			UpdateUiFields();
		}

		public void IncrementAISkill()
		{
			if ((int)tempSettings.AISkill < Enum.GetNames(typeof(AISkillLevel)).Length - 1)
			{
				tempSettings.AISkill++;
			}
			UpdateUiFields();
		}

		public void DecrementAISkill()
		{
			if (tempSettings.AISkill > AISkillLevel.Retarded)
			{
				tempSettings.AISkill--;
			}
			UpdateUiFields();
		}

		public void IncrementAICharacter(int pos)
		{
			int num = tempSettings.GetAICharacter(pos);
			do
			{
				num++;
				if (num >= ActiveData.Characters.Length)
				{
					num = 0;
				}
			}
			while (ActiveData.Characters[num].hidden);
			tempSettings.SetAICharacter(pos, num);
			UpdateUiFields();
		}

		public void DecrementAICharacter(int pos)
		{
			int num = tempSettings.GetAICharacter(pos);
			do
			{
				num--;
				if (num < 0)
				{
					num = ActiveData.Characters.Length - 1;
				}
			}
			while (ActiveData.Characters[num].hidden);
			tempSettings.SetAICharacter(pos, num);
			UpdateUiFields();
		}

		private void UpdateUiFields()
		{
			stage.text = ActiveData.Stages[tempSettings.StageId].name;
			laps.text = tempSettings.Laps.ToString();
			aiCount.text = ((tempSettings.AICount != 0) ? tempSettings.AICount.ToString() : "None");
			aiSkill.text = tempSettings.AISkill.ToString();
			for (int i = 0; i < aiCharacters.Length; i++)
			{
				aiCharacters[i].text = ActiveData.Characters[tempSettings.GetAICharacter(i)].name;
			}
		}

		private void Start()
		{
			RevertSettings();
			UpdateUiFields();
			EventSystem.current.SetSelectedGameObject(firstActive);
		}
	}
}
