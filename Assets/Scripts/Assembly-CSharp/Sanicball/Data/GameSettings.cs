using System;
using UnityEngine;

namespace Sanicball.Data
{
	[Serializable]
	public class GameSettings
	{
		[Header("Online")]
		public string nickname = string.Empty;

		public string serverListURL = "https://sanicball.bdgr.zone/servers/";

		public string gameJoltUsername;

		public string gameJoltToken;

		[Header("Display")]
		public int resolution;

		public bool fullscreen = true;

		public bool vsync;

		public bool useImperial;

		public bool showControlsWhileWaiting = true;

		[Header("Graphics")]
		public int aa;

		public bool trails = true;

		public bool shadows = true;

		public bool motionBlur;

		public bool bloom;

		public ReflectionQuality reflectionQuality;

		public bool eSportsReady;

		[Header("Gameplay")]
		public bool useOldControls;

		public float oldControlsMouseSpeed = 3f;

		public float oldControlsKbSpeed = 10f;

		[Header("Audio")]
		public float soundVolume = 1f;

		public bool music = true;

		public bool fastMusic = true;

		public void CopyValues(GameSettings original)
		{
			nickname = original.nickname;
			serverListURL = original.serverListURL;
			gameJoltUsername = original.gameJoltUsername;
			gameJoltToken = original.gameJoltToken;
			resolution = original.resolution;
			fullscreen = original.fullscreen;
			vsync = original.vsync;
			useImperial = original.useImperial;
			showControlsWhileWaiting = original.showControlsWhileWaiting;
			aa = original.aa;
			trails = original.trails;
			shadows = original.shadows;
			motionBlur = original.motionBlur;
			bloom = original.bloom;
			reflectionQuality = original.reflectionQuality;
			eSportsReady = original.eSportsReady;
			useOldControls = original.useOldControls;
			oldControlsMouseSpeed = original.oldControlsMouseSpeed;
			oldControlsKbSpeed = original.oldControlsKbSpeed;
			soundVolume = original.soundVolume;
			music = original.music;
			fastMusic = original.fastMusic;
		}

		public void Validate()
		{
			if (resolution >= Screen.resolutions.Length)
			{
				resolution = Screen.resolutions.Length - 1;
			}
			if (resolution < 0)
			{
				resolution = 0;
			}
			if (aa != 0 && aa != 2 && aa != 4 && aa != 8)
			{
				aa = 0;
			}
			oldControlsMouseSpeed = Mathf.Clamp(oldControlsMouseSpeed, 0.5f, 10f);
			oldControlsKbSpeed = Mathf.Clamp(oldControlsKbSpeed, 0.5f, 10f);
			soundVolume = Mathf.Clamp(soundVolume, 0f, 1f);
		}

		public void Apply(bool changeWindow)
		{
			if (changeWindow && this.resolution < Screen.resolutions.Length)
			{
				Resolution resolution = Screen.resolutions[this.resolution];
				if (Screen.width != resolution.width || Screen.height != resolution.height || fullscreen != Screen.fullScreen)
				{
					Screen.SetResolution(resolution.width, resolution.height, fullscreen);
				}
			}
			QualitySettings.antiAliasing = aa;
			if (vsync)
			{
				QualitySettings.vSyncCount = 1;
			}
			else
			{
				QualitySettings.vSyncCount = 0;
			}
			GameObject gameObject = GameObject.Find("Directional light");
			if (gameObject != null)
			{
				LightShadows lightShadows = (shadows ? LightShadows.Hard : LightShadows.None);
				gameObject.GetComponent<Light>().shadows = lightShadows;
			}
			AudioListener.volume = soundVolume;
			MusicPlayer musicPlayer = UnityEngine.Object.FindObjectOfType<MusicPlayer>();
			if ((bool)musicPlayer)
			{
				musicPlayer.GetComponent<AudioSource>().mute = !musicPlayer;
			}
			CameraEffects[] array = UnityEngine.Object.FindObjectsOfType<CameraEffects>();
			foreach (CameraEffects cameraEffects in array)
			{
				cameraEffects.EnableEffects();
			}
		}
	}
}
