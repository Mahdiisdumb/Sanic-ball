using UnityEngine;

namespace Sanicball
{
	public class MouseUnlocker : MonoBehaviour
	{
		private void Start()
		{
			if (Object.FindObjectsOfType<MouseUnlocker>().Length > 1)
			{
				Object.Destroy(base.gameObject);
			}
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.LeftAlt))
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}
}
