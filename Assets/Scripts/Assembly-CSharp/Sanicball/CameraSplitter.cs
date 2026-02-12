using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sanicball
{
	[RequireComponent(typeof(Camera))]
	public class CameraSplitter : MonoBehaviour
	{
		private Camera cam;

		private AudioListener listener;

		public int SplitscreenIndex { get; set; }

		private void Start()
		{
			cam = GetComponent<Camera>();
			listener = GetComponent<AudioListener>();
			List<CameraSplitter> list = Object.FindObjectsOfType<CameraSplitter>().ToList();
			int count = list.Count;
			int splitscreenIndex = SplitscreenIndex;
			switch (count)
			{
			case 2:
				cam.rect = new Rect(0f, (splitscreenIndex != 0) ? 0f : 0.5f, 1f, 0.5f);
				break;
			case 3:
				switch (splitscreenIndex)
				{
				case 0:
					cam.rect = new Rect(0f, 0.5f, 1f, 0.5f);
					break;
				case 1:
					cam.rect = new Rect(0f, 0f, 0.5f, 0.5f);
					break;
				case 2:
					cam.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
					break;
				}
				break;
			case 4:
				switch (splitscreenIndex)
				{
				case 0:
					cam.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
					break;
				case 1:
					cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
					break;
				case 2:
					cam.rect = new Rect(0f, 0f, 0.5f, 0.5f);
					break;
				case 3:
					cam.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
					break;
				}
				break;
			default:
				cam.rect = new Rect(0f, 0f, 1f, 1f);
				break;
			}
			if ((bool)listener)
			{
				listener.enabled = splitscreenIndex == 0;
			}
		}
	}
}
