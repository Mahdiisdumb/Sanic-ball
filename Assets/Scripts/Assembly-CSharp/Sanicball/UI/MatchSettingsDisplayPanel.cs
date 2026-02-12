using System;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class MatchSettingsDisplayPanel : MonoBehaviour
	{
		[Header("Fields")]
		public Text stageName;

		public Image stageImage;

		public Text lapCount;

		public Text aiOpponents;

		public Text aiSkill;

		private Vector3 targetStageCamPos;

		[SerializeField]
		private Animation settingsChangedAnimation;

		[SerializeField]
		private Camera stageLayoutCamera;

		private MatchManager manager;

		private void Start()
		{
			manager = UnityEngine.Object.FindObjectOfType<MatchManager>();
			manager.MatchSettingsChanged += Manager_MatchSettingsChanged;
			Manager_MatchSettingsChanged(this, EventArgs.Empty);
		}

		private void Manager_MatchSettingsChanged(object sender, EventArgs e)
		{
			MatchSettings currentSettings = manager.CurrentSettings;
			targetStageCamPos = new Vector3(currentSettings.StageId * 50, stageLayoutCamera.transform.position.y, stageLayoutCamera.transform.position.z);
			stageName.text = ActiveData.Stages[currentSettings.StageId].name;
			stageImage.sprite = ActiveData.Stages[currentSettings.StageId].picture;
			lapCount.text = currentSettings.Laps + ((currentSettings.Laps != 1) ? " laps" : " lap");
			aiOpponents.text = string.Empty;
			aiSkill.text = "AI Skill: " + currentSettings.AISkill;
			settingsChangedAnimation.Rewind();
			settingsChangedAnimation.Play();
		}

		private void Update()
		{
			if (Vector3.Distance(stageLayoutCamera.transform.position, targetStageCamPos) > 0.1f)
			{
				stageLayoutCamera.transform.position = Vector3.Lerp(stageLayoutCamera.transform.position, targetStageCamPos, Time.deltaTime * 10f);
				if (Vector3.Distance(stageLayoutCamera.transform.position, targetStageCamPos) <= 0.1f)
				{
					stageLayoutCamera.transform.position = targetStageCamPos;
				}
			}
		}

		private void OnDestroy()
		{
			manager.MatchSettingsChanged -= Manager_MatchSettingsChanged;
		}
	}
}
