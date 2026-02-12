using Sanicball.Logic;
using UnityEngine;

namespace Sanicball.UI
{
	public class RaceUI : MonoBehaviour
	{
		[SerializeField]
		private PlayerPortrait portraitPrefab;

		[SerializeField]
		private Transform portraitContainer;

		public RaceManager TargetManager { get; set; }

		private void Start()
		{
			for (int i = 0; i < TargetManager.PlayerCount; i++)
			{
				PlayerPortrait playerPortrait = Object.Instantiate(portraitPrefab);
				playerPortrait.transform.SetParent(portraitContainer, false);
				playerPortrait.TargetPlayer = TargetManager[i];
			}
		}
	}
}
