using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(RectTransform), typeof(Image))]
	public class Marker : MonoBehaviour
	{
		private const float clampMin = 0.2f;

		private const float clampMax = 0.8f;

		[SerializeField]
		private Transform target;

		[SerializeField]
		private Text textField;

		private RectTransform rectTransform;

		private Image image;

		private float colorAlpha;

		public Transform Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public string Text
		{
			get
			{
				return textField.text;
			}
			set
			{
				textField.text = value;
			}
		}

		public Sprite Sprite
		{
			set
			{
				image.sprite = value;
			}
		}

		public Color Color
		{
			set
			{
				image.color = value;
				colorAlpha = value.a;
			}
		}

		public Camera CameraToUse { get; set; }

		public bool Clamp { get; set; }

		public bool HideImageWhenInSight { get; set; }

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			image = GetComponent<Image>();
			colorAlpha = image.color.a;
		}

		private void Update()
		{
			Camera camera = Camera.main;
			if ((bool)CameraToUse)
			{
				camera = CameraToUse;
			}
			if (!target || !camera)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			Vector3 position = camera.transform.InverseTransformPoint(target.position);
			position.z = Mathf.Max(position.z, 1f);
			Vector3 vector = camera.WorldToViewportPoint(camera.transform.TransformPoint(position));
			if (Clamp)
			{
				vector = new Vector3(Mathf.Clamp(vector.x, 0.2f, 0.8f), Mathf.Clamp(vector.y, 0.2f, 0.8f), vector.z);
			}
			rectTransform.anchorMin = vector;
			rectTransform.anchorMax = vector;
			if (HideImageWhenInSight)
			{
				Ray ray = camera.ViewportPointToRay(vector);
				bool flag = true;
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 400f))
				{
					flag = hitInfo.transform == Target;
				}
				image.color = new Color(image.color.r, image.color.g, image.color.b, (!flag) ? 0f : colorAlpha);
			}
		}
	}
}
