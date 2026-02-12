using UnityEngine;

namespace Sanicball
{
	public class ToggleGameobject : MonoBehaviour
	{
		public void Toggle()
		{
			base.gameObject.SetActive(!base.gameObject.activeSelf);
		}
	}
}
