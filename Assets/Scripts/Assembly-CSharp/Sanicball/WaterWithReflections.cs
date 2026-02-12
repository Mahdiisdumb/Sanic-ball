using Sanicball.Data;
using UnityEngine;

namespace Sanicball
{
	[RequireComponent(typeof(MeshRenderer))]
	public class WaterWithReflections : MonoBehaviour
	{
		[SerializeField]
		private MirrorReflection reflection;

		[SerializeField]
		private float materialAlphaWithReflections = 0.8f;

		private void Start()
		{
			ReflectionQuality reflectionQuality = ActiveData.GameSettings.reflectionQuality;
			if (reflectionQuality == ReflectionQuality.Off)
			{
				reflection.enabled = false;
				return;
			}
			reflection.enabled = true;
			Color color = GetComponent<MeshRenderer>().material.color;
			color.a = materialAlphaWithReflections;
			GetComponent<MeshRenderer>().material.color = color;
			switch (reflectionQuality)
			{
			case ReflectionQuality.Low:
				reflection.m_TextureSize = 256;
				break;
			case ReflectionQuality.Medium:
				reflection.m_TextureSize = 512;
				break;
			case ReflectionQuality.High:
				reflection.m_TextureSize = 1024;
				break;
			}
		}

		private void Update()
		{
		}
	}
}
