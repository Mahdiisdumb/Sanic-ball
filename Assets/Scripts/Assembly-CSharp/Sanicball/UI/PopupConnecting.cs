using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class PopupConnecting : MonoBehaviour
	{
		[SerializeField]
		private Text titleField;

		[SerializeField]
		private Image spinner;

		public void ShowMessage(string text)
		{
			titleField.text = text;
			spinner.enabled = false;
		}
	}
}
