using System;
using Sanicball.Data;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Sanicball
{
	public class CameraEffects : MonoBehaviour
	{
		public bool isOmniCam;

		public Texture2D bloomLensFlareVignetteMask;

		public Shader bloomLensFlareShader;

		public Shader bloomScreenBlendShader;

		public Shader bloomBlurAndFlaresShader;

		public Shader bloomBrightPassFilter;

		public Shader motionBlurShader;

		public Shader motionBlurDX11Shader;

		public Shader motionBlurReplacementClearShader;

		public Texture2D motionBlurNoiseTexture;

		[NonSerialized]
		public Bloom bloom;

		[NonSerialized]
		public CameraMotionBlur blur;

		private void Start()
		{
			bloom = base.gameObject.AddComponent<Bloom>();
			bloom.bloomIntensity = 0.8f;
			bloom.bloomThreshold = 0.8f;
			bloom.lensFlareVignetteMask = bloomLensFlareVignetteMask;
			bloom.lensFlareShader = bloomLensFlareShader;
			bloom.screenBlendShader = bloomScreenBlendShader;
			bloom.blurAndFlaresShader = bloomBlurAndFlaresShader;
			bloom.brightPassFilterShader = bloomBrightPassFilter;
			blur = base.gameObject.AddComponent<CameraMotionBlur>();
			blur.filterType = CameraMotionBlur.MotionBlurFilter.LocalBlur;
			blur.velocityScale = 1f;
			blur.maxVelocity = 1000f;
			blur.minVelocity = 0.1f;
			blur.shader = motionBlurShader;
			blur.dx11MotionBlurShader = motionBlurDX11Shader;
			blur.replacementClear = motionBlurReplacementClearShader;
			blur.noiseTexture = motionBlurNoiseTexture;
			if (ActiveData.ESportsFullyReady)
			{
			}
			EnableEffects();
		}

		private void Update()
		{
		}

		public void EnableEffects()
		{
			bloom.enabled = ActiveData.GameSettings.bloom;
			blur.enabled = ActiveData.GameSettings.motionBlur;
		}
	}
}
