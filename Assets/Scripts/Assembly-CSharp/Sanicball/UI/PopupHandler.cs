using System;
using UnityEngine;

namespace Sanicball.UI
{
	public class PopupHandler : MonoBehaviour
	{
		public CanvasGroup groupDisabledOnPopup;

		public Transform targetParent;

		private Popup activePopup;

		public void OpenPopup(Popup popupPrefab)
		{
			if (activePopup != null)
			{
				activePopup.Close();
				groupDisabledOnPopup.interactable = true;
			}
			activePopup = UnityEngine.Object.Instantiate(popupPrefab);
			activePopup.transform.SetParent(targetParent, false);
			Popup popup = activePopup;
			popup.onClose = (Action)Delegate.Combine(popup.onClose, (Action)delegate
			{
				groupDisabledOnPopup.interactable = true;
			});
			groupDisabledOnPopup.interactable = false;
		}

		public void CloseActivePopup()
		{
			activePopup.Close();
			activePopup = null;
			groupDisabledOnPopup.interactable = true;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
