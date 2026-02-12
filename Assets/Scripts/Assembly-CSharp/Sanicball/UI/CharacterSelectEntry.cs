using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
	public class CharacterSelectEntry : MonoBehaviour
	{
		[SerializeField]
		private Image iconImage;

		private RectTransform rt;

		private LayoutElement le;

		private float size;

		public Sanicball.Data.CharacterInfo Character { get; private set; }

		public Image IconImage
		{
			get
			{
				return iconImage;
			}
		}

		public RectTransform RectTransform
		{
			get
			{
				return rt;
			}
		}

		public float Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				le.preferredWidth = size;
				le.preferredHeight = size;
			}
		}

		private void Start()
		{
			rt = GetComponent<RectTransform>();
			le = GetComponent<LayoutElement>();
			size = le.preferredWidth;
		}

		public void Init(Sanicball.Data.CharacterInfo character)
		{
			Character = character;
			iconImage.sprite = character.icon;
		}
	}
}
