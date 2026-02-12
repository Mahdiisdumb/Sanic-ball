using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(LayoutElement))]
	public class StageImage : MonoBehaviour, IEventSystemHandler, ISelectHandler, IDeselectHandler
	{
		public delegate void StageImageEvent();

		public StageSelection stageSelectionObject;

		public float baseWidth = 350f;

		public float baseHeight = 233f;

		public float selectedScale = 0.8f;

		public float deselectedScale = 0.5f;

		public float animationSpeed = 5f;

		private bool selected;

		private LayoutElement le;

		private Button b;

		public event StageImageEvent onSelect;

		public event StageImageEvent onActivate;

		public void OnSelect(BaseEventData eventData)
		{
			selected = true;
			if (this.onSelect != null)
			{
				this.onSelect();
			}
			b.onClick.AddListener(Activate);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			selected = false;
			b.onClick.RemoveAllListeners();
		}

		private void Start()
		{
			le = GetComponent<LayoutElement>();
			b = GetComponent<Button>();
			le.preferredWidth = baseWidth * deselectedScale;
			le.preferredHeight = baseHeight * deselectedScale;
		}

		private void Update()
		{
			float num = ((!selected) ? (baseWidth * deselectedScale) : (baseWidth * selectedScale));
			float num2 = ((!selected) ? (baseHeight * deselectedScale) : (baseHeight * selectedScale));
			le.preferredWidth = Mathf.Lerp(le.preferredWidth, num, Time.deltaTime * animationSpeed);
			le.preferredHeight = Mathf.Lerp(le.preferredHeight, num2, Time.deltaTime * animationSpeed);
		}

		private void Activate()
		{
			if (this.onActivate != null)
			{
				this.onActivate();
			}
		}
	}
}
