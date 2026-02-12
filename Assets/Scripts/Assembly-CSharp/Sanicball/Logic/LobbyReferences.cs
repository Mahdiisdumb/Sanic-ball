using Sanicball.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.Logic
{
	public class LobbyReferences : MonoBehaviour
	{
		[SerializeField]
		private LobbyStatusBar statusBar;

		[SerializeField]
		private LocalPlayerManager localPlayerManager;

		[SerializeField]
		private MatchSettingsPanel matchSettingsPanel;

		[SerializeField]
		private LobbyBallSpawner ballSpawner;

		[SerializeField]
		private Text countdownField;

		[SerializeField]
		private RectTransform markerContainer;

		public static LobbyReferences Active { get; private set; }

		public LobbyStatusBar StatusBar
		{
			get
			{
				return statusBar;
			}
		}

		public LocalPlayerManager LocalPlayerManager
		{
			get
			{
				return localPlayerManager;
			}
		}

		public MatchSettingsPanel MatchSettingsPanel
		{
			get
			{
				return matchSettingsPanel;
			}
		}

		public LobbyBallSpawner BallSpawner
		{
			get
			{
				return ballSpawner;
			}
		}

		public Text CountdownField
		{
			get
			{
				return countdownField;
			}
		}

		public RectTransform MarkerContainer
		{
			get
			{
				return markerContainer;
			}
		}

		private void Awake()
		{
			Active = this;
			CameraFade.StartAlphaFade(Color.black, true, 1f);
		}
	}
}
