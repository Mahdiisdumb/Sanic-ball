using Sanicball.Data;
using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
	public class LobbyBallSpawner : BallSpawner
	{
		[SerializeField]
		private LobbyPlatform lobbyPlatform;

		public Ball SpawnBall(PlayerType playerType, ControlType ctrlType, int character, string nickname)
		{
			if ((bool)lobbyPlatform)
			{
				lobbyPlatform.Activate();
			}
			else
			{
				Debug.LogError("LobbyBallSpawner has no lobby platform assigned");
			}
			return SpawnBall(base.transform.position, base.transform.rotation, BallType.LobbyPlayer, ctrlType, character, nickname);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, 0.5f);
			Gizmos.DrawWireSphere(base.transform.position, 1f);
		}
	}
}
