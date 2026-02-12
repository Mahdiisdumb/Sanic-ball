using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class StageSelection : MonoBehaviour
	{
		public Text title;

		public Transform stageList;

		public Vector3 stageOverviewSpawnPosition;

		public StageImage stageImagePrefab;

		private GameObject currentStageOverview;

		public void SetActiveStage(StageInfo s)
		{
			title.text = s.name;
			if (currentStageOverview != null)
			{
				Object.Destroy(currentStageOverview);
			}
			currentStageOverview = Object.Instantiate(s.overviewPrefab, stageOverviewSpawnPosition, Quaternion.Euler(270f, 0f, 0f)) as GameObject;
		}

		private void Start()
		{
			for (int i = 0; i < ActiveData.Stages.Length; i++)
			{
				StageInfo s = ActiveData.Stages[i];
				StageImage stageImage = Object.Instantiate(stageImagePrefab);
				stageImage.transform.SetParent(stageList, false);
				stageImage.GetComponent<Image>().sprite = s.picture;
				stageImage.onSelect += delegate
				{
					SetActiveStage(s);
				};
			}
		}
	}
}
