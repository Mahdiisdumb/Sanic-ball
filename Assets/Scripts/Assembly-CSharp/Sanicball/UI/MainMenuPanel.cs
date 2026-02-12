using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class MainMenuPanel : MonoBehaviour
	{
		[SerializeField]
		private Text versionNameField;

		[SerializeField]
		private Text taglineField;

		private SlideCanvasGroup activePanel;

		public void SetActivePanel(SlideCanvasGroup panel)
		{
			if (activePanel == null)
			{
				panel.Open();
				activePanel = panel;
			}
			else if (activePanel != panel)
			{
				CloseActivePanel();
				panel.Open();
				activePanel = panel;
			}
			else
			{
				CloseActivePanel();
			}
		}

		public void CloseActivePanel()
		{
			activePanel.Close();
			activePanel = null;
		}

		private void Start()
		{
			versionNameField.text = "alpha v0.8.2";
			taglineField.text = "The best failed early access game";
		}

		private void Update()
		{
			if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)) && Object.FindObjectsOfType<Popup>().Length <= 0)
			{
				if (activePanel != null)
				{
					CloseActivePanel();
				}
				else
				{
					Application.Quit();
				}
			}
		}
	}
}
