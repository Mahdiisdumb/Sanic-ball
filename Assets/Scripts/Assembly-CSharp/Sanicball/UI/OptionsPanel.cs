using System;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class OptionsPanel : MonoBehaviour
	{
		[Header("Online")]
		public Text nickname;

		public Text serverListURL;

		public Text gameJoltAccount;

		[Header("Display")]
		public Text resolution;

		public Text fullscreen;

		public Text vsync;

		public Text speedUnit;

		[Header("Graphics")]
		public Text aa;

		public Text trails;

		public Text shadows;

		public Text motionBlur;

		public Text bloom;

		public Text reflectionQuality;

		public Text eSportsReady;

		[Header("Gameplay")]
		public Text controlMode;

		public Text cameraSpeedMouse;

		public Text cameraSpeedKeyboard;

		[Header("Audio")]
		public Text soundVolume;

		public Text music;

		public Text fast;

		private GameSettings tempSettings = new GameSettings();

		public void Apply()
		{
			ActiveData.GameSettings.CopyValues(tempSettings);
			ActiveData.GameSettings.Apply(true);
		}

		public void RevertToCurrent()
		{
			tempSettings.CopyValues(ActiveData.GameSettings);
			UpdateFields();
		}

		public void ResetToDefault()
		{
			string text = tempSettings.nickname;
			tempSettings = new GameSettings();
			tempSettings.nickname = text;
			UpdateFields();
		}

		public void UpdateFields()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			nickname.text = tempSettings.nickname;
			serverListURL.text = tempSettings.serverListURL;
			gameJoltAccount.text = (string.IsNullOrEmpty(tempSettings.gameJoltToken) ? "Not linked" : ("Linked as " + tempSettings.gameJoltUsername));
			if (Screen.resolutions.Length > 0)
			{
				if (tempSettings.resolution >= Screen.resolutions.Length)
				{
					tempSettings.resolution = 0;
				}
				Resolution resolution = Screen.resolutions[tempSettings.resolution];
				this.resolution.text = resolution.width + " x " + resolution.height;
			}
			else
			{
				this.resolution.text = "None found!";
			}
			fullscreen.text = ((!tempSettings.fullscreen) ? "Windowed" : "Fullscreen");
			vsync.text = ((!tempSettings.vsync) ? "Off" : "On");
			speedUnit.text = ((!tempSettings.useImperial) ? "Metric" : "Imperial");
			aa.text = ((tempSettings.aa != 0) ? ("x" + tempSettings.aa) : "Off");
			trails.text = ((!tempSettings.trails) ? "Off" : "On");
			shadows.text = ((!tempSettings.shadows) ? "Off" : "On");
			motionBlur.text = ((!tempSettings.motionBlur) ? "Off" : "On");
			bloom.text = ((!tempSettings.bloom) ? "Off" : "On");
			reflectionQuality.text = tempSettings.reflectionQuality.ToString();
			eSportsReady.text = ((!tempSettings.eSportsReady) ? "No way" : "Born ready");
			controlMode.text = ((!tempSettings.useOldControls) ? "Follow velocity (Intuitive)" : "Rotate manually (Precise)");
			cameraSpeedMouse.text = tempSettings.oldControlsMouseSpeed.ToString("n1");
			cameraSpeedKeyboard.text = tempSettings.oldControlsKbSpeed.ToString("n1");
			soundVolume.text = ((!(tempSettings.soundVolume <= 0f)) ? (tempSettings.soundVolume * 10f).ToString("n0") : "Off");
			music.text = ((!tempSettings.music) ? "Off" : "On");
			fast.text = ((!tempSettings.fastMusic) ? "Off" : "On");
		}

		private void Start()
		{
			RevertToCurrent();
		}

		public void SetNickname(string nick)
		{
			tempSettings.nickname = nick;
			UpdateFields();
		}

		public void SetServerListURL(string url)
		{
			tempSettings.serverListURL = url;
			UpdateFields();
		}

		public void SetGameJoltInfo(string username, string token)
		{
			tempSettings.gameJoltUsername = username;
			tempSettings.gameJoltToken = token;
			UpdateFields();
		}

		public string GetNickname()
		{
			return tempSettings.nickname;
		}

		public string GetServerListURL()
		{
			return tempSettings.serverListURL;
		}

		public string GetGameJoltUsername()
		{
			return tempSettings.gameJoltUsername;
		}

		public string GetGameJoltToken()
		{
			return tempSettings.gameJoltToken;
		}

		public void ResolutionUp()
		{
			tempSettings.resolution++;
			if (tempSettings.resolution >= Screen.resolutions.Length)
			{
				tempSettings.resolution = 0;
			}
			UpdateFields();
		}

		public void ResolutionDown()
		{
			tempSettings.resolution--;
			if (tempSettings.resolution < 0)
			{
				tempSettings.resolution = Screen.resolutions.Length - 1;
			}
			UpdateFields();
		}

		public void FullscreenToggle()
		{
			tempSettings.fullscreen = !tempSettings.fullscreen;
			UpdateFields();
		}

		public void VsyncToggle()
		{
			tempSettings.vsync = !tempSettings.vsync;
			UpdateFields();
		}

		public void SpeedUnitToggle()
		{
			tempSettings.useImperial = !tempSettings.useImperial;
			UpdateFields();
		}

		public void AaUp()
		{
			switch (tempSettings.aa)
			{
			case 0:
				tempSettings.aa = 2;
				break;
			case 2:
				tempSettings.aa = 4;
				break;
			case 4:
				tempSettings.aa = 8;
				break;
			case 8:
				tempSettings.aa = 0;
				break;
			default:
				tempSettings.aa = 0;
				break;
			}
			UpdateFields();
		}

		public void AaDown()
		{
			switch (tempSettings.aa)
			{
			case 0:
				tempSettings.aa = 8;
				break;
			case 2:
				tempSettings.aa = 0;
				break;
			case 4:
				tempSettings.aa = 2;
				break;
			case 8:
				tempSettings.aa = 4;
				break;
			default:
				tempSettings.aa = 0;
				break;
			}
			UpdateFields();
		}

		public void TrailsToggle()
		{
			tempSettings.trails = !tempSettings.trails;
			UpdateFields();
		}

		public void ShadowsToggle()
		{
			tempSettings.shadows = !tempSettings.shadows;
			UpdateFields();
		}

		public void MotionBlurToggle()
		{
			tempSettings.motionBlur = !tempSettings.motionBlur;
			UpdateFields();
		}

		public void BloomToggle()
		{
			tempSettings.bloom = !tempSettings.bloom;
			UpdateFields();
		}

		public void ReflectionQualityUp()
		{
			int num = (int)tempSettings.reflectionQuality;
			num = Mathf.Min(num + 1, Enum.GetNames(typeof(ReflectionQuality)).Length - 1);
			tempSettings.reflectionQuality = (ReflectionQuality)num;
			UpdateFields();
		}

		public void ReflectionQualityDown()
		{
			int num = (int)tempSettings.reflectionQuality;
			num = Mathf.Max(num - 1, 0);
			tempSettings.reflectionQuality = (ReflectionQuality)num;
			UpdateFields();
		}

		public void ESportsToggle()
		{
			tempSettings.eSportsReady = !tempSettings.eSportsReady;
			UpdateFields();
		}

		public void UseOldControlsToggle()
		{
			tempSettings.useOldControls = !tempSettings.useOldControls;
			UpdateFields();
		}

		public void CameraSpeedMouseUp()
		{
			if (tempSettings.oldControlsMouseSpeed < 10f)
			{
				tempSettings.oldControlsMouseSpeed += 0.5f;
			}
			else
			{
				tempSettings.oldControlsMouseSpeed = 10f;
			}
			UpdateFields();
		}

		public void CameraSpeedMouseDown()
		{
			if (tempSettings.oldControlsMouseSpeed > 0.5f)
			{
				tempSettings.oldControlsMouseSpeed -= 0.5f;
			}
			else
			{
				tempSettings.oldControlsMouseSpeed = 0.5f;
			}
			UpdateFields();
		}

		public void CameraSpeedKbUp()
		{
			if (tempSettings.oldControlsKbSpeed < 10f)
			{
				tempSettings.oldControlsKbSpeed += 0.5f;
			}
			else
			{
				tempSettings.oldControlsKbSpeed = 10f;
			}
			UpdateFields();
		}

		public void CameraSpeedKbDown()
		{
			if (tempSettings.oldControlsKbSpeed > 0.5f)
			{
				tempSettings.oldControlsKbSpeed -= 0.5f;
			}
			else
			{
				tempSettings.oldControlsKbSpeed = 0.5f;
			}
			UpdateFields();
		}

		public void SoundVolumeUp()
		{
			if (tempSettings.soundVolume < 1f)
			{
				tempSettings.soundVolume += 0.1f;
			}
			else
			{
				tempSettings.soundVolume = 0f;
			}
			UpdateFields();
		}

		public void SoundVolumeDown()
		{
			if (tempSettings.soundVolume > 0f)
			{
				tempSettings.soundVolume -= 0.1f;
			}
			else
			{
				tempSettings.soundVolume = 1f;
			}
			UpdateFields();
		}

		public void MusicToggle()
		{
			tempSettings.music = !tempSettings.music;
			UpdateFields();
		}

		public void FastMusicToggle()
		{
			tempSettings.fastMusic = !tempSettings.fastMusic;
			UpdateFields();
		}
	}
}
