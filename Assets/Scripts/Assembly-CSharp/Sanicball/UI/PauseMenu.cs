using Sanicball.Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class PauseMenu : MonoBehaviour
	{
		private const string pauseTag = "Pause";

		[SerializeField]
		private GameObject firstSelected;

		[SerializeField]
		private Button contextSensitiveButton;

		[SerializeField]
		private Text contextSensitiveButtonLabel;

		private bool mouseWasLocked;

		public static bool GamePaused
		{
			get
			{
				return GameObject.FindWithTag("Pause");
			}
		}

		public bool OnlineMode { get; set; }

		private void Awake()
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				mouseWasLocked = true;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		private void Start()
		{
			EventSystem.current.SetSelectedGameObject(firstSelected);
			if (!OnlineMode)
			{
				Time.timeScale = 0f;
				AudioListener.pause = true;
			}
			if (SceneManager.GetActiveScene().name == "Lobby")
			{
				contextSensitiveButtonLabel.text = "Change match settings";
				contextSensitiveButton.onClick.AddListener(MatchSettings);
				if (OnlineMode)
				{
					contextSensitiveButton.interactable = false;
				}
			}
			else
			{
				contextSensitiveButtonLabel.text = "Return to lobby";
				contextSensitiveButton.onClick.AddListener(BackToLobby);
			}
		}

		public void Close()
		{
			if (mouseWasLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			if (!OnlineMode)
			{
				Time.timeScale = 1f;
				AudioListener.pause = false;
			}
		}

		public void MatchSettings()
		{
			LobbyReferences.Active.MatchSettingsPanel.Show();
			Close();
		}

		public void BackToLobby()
		{
			MatchManager matchManager = Object.FindObjectOfType<MatchManager>();
			if ((bool)matchManager)
			{
				matchManager.RequestLoadLobby();
				Close();
			}
			else
			{
				Debug.LogError("Cannot return to lobby: no match manager found to handle the request. Something is broken!");
			}
		}

		public void QuitMatch()
		{
			MatchManager matchManager = Object.FindObjectOfType<MatchManager>();
			if ((bool)matchManager)
			{
				matchManager.QuitMatch();
			}
			else
			{
				SceneManager.LoadScene("Menu");
			}
		}
	}
}
