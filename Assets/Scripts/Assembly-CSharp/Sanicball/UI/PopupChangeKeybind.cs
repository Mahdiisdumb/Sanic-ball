using System;
using System.Linq;
using UnityEngine;

namespace Sanicball.UI
{
	[RequireComponent(typeof(Popup))]
	public class PopupChangeKeybind : MonoBehaviour
	{
		private ControlsPanel panel;

		private KeyCode[] validKeyCodes;

		private void Start()
		{
			panel = UnityEngine.Object.FindObjectOfType<ControlsPanel>();
			if (!panel)
			{
				GetComponent<Popup>().Close();
			}
			validKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
			validKeyCodes = validKeyCodes.Where((KeyCode a) => !a.ToString().Contains("Mouse") && !a.ToString().Contains("Joystick")).ToArray();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GetComponent<Popup>().Close();
				return;
			}
			KeyCode[] array = validKeyCodes;
			foreach (KeyCode keyCode in array)
			{
				if (Input.GetKeyDown(keyCode) && keyCode != KeyCode.Escape)
				{
					panel.ChangeKeybind(keyCode);
					GetComponent<Popup>().Close();
					break;
				}
			}
		}
	}
}
