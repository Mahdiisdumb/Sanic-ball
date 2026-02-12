using System.Collections.Generic;
using Sanicball.Gameplay;
using UnityEngine;

namespace Sanicball
{
	public class ESportMode : MonoBehaviour
	{
		private const float COLOR_TIME = 0.54545456f;

		public Texture2D screenOverlay;

		public Texture2D solidWhite;

		public Texture2D snoop;

		private bool screenOverlayEnabled;

		private bool timerOn;

		private bool bass;

		private float timer = 1f;

		private bool started;

		private Color currentColor = new Color(1f, 0f, 0f, 0.2f);

		private float colorTimer = 0.54545456f;

		private Vector2 snoopPos = new Vector2(0f, 0f);

		private Vector2 snoopTarget = new Vector2(0f, 0f);

		private int qSamples = 1024;

		private float refValue = 0.1f;

		private float rmsValue;

		private float dbValue;

		private float[] samples;

		private float RMSmin;

		private float RMSmax;

		private List<Camera> cameras = new List<Camera>();

		private AudioSource music;

		private void Start()
		{
			samples = new float[qSamples];
		}

		public void StartTheShit()
		{
			timerOn = true;
		}

		private void Start4Real()
		{
			started = true;
			screenOverlayEnabled = true;
			CameraEffects[] array = Object.FindObjectsOfType<CameraEffects>();
			foreach (CameraEffects cameraEffects in array)
			{
				cameras.Add(cameraEffects.GetComponent<Camera>());
				cameraEffects.bloom.bloomIntensity = 2f;
				cameraEffects.bloom.bloomThreshold = 0.6f;
				cameraEffects.blur.velocityScale = 4f;
			}
			music = Object.FindObjectOfType<MusicPlayer>().GetComponent<AudioSource>();
		}

		private void GetVolume()
		{
			music.GetOutputData(samples, 0);
			float num = 0f;
			for (int i = 0; i < qSamples; i++)
			{
				num += samples[i] * samples[i];
			}
			rmsValue = Mathf.Sqrt(num / (float)qSamples);
			dbValue = 20f * Mathf.Log10(rmsValue / refValue);
			if (dbValue < -160f)
			{
				dbValue = -160f;
			}
		}

		private void Update()
		{
			if (Camera.main != null)
			{
				base.transform.position = Camera.main.transform.position;
				base.transform.rotation = Camera.main.transform.rotation;
			}
			if (timerOn)
			{
				timer -= Time.deltaTime;
				bass = timer < 0.2f && timer > 0.02f;
				if (timer <= 0f)
				{
					timerOn = false;
					Start4Real();
				}
			}
			if (screenOverlayEnabled)
			{
				snoopPos = Vector2.MoveTowards(snoopPos, snoopTarget, Time.deltaTime * 32f);
				if (snoopPos == snoopTarget)
				{
					snoopTarget = new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
				}
				colorTimer -= Time.deltaTime;
				if (colorTimer <= 0f)
				{
					currentColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0.2f);
					colorTimer += 0.54545456f;
				}
			}
			if (!started)
			{
				return;
			}
			GetVolume();
			RMSmin = Mathf.Min(RMSmin, rmsValue);
			RMSmax = Mathf.Max(RMSmax, rmsValue);
			Camera.main.backgroundColor = Color.Lerp(Color.magenta, Color.blue, rmsValue);
			float num = 20f - rmsValue * 80f;
			foreach (Camera camera in cameras)
			{
				OmniCamera component = camera.GetComponent<OmniCamera>();
				if ((bool)component)
				{
					component.fovOffset = num;
				}
				else
				{
					camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 72f + num, Time.deltaTime * 20f);
				}
			}
		}

		private void OnGUI()
		{
			Rect position = new Rect(0f, 0f, Screen.width, Screen.height);
			if (screenOverlayEnabled)
			{
				GUIStyle gUIStyle = new GUIStyle();
				gUIStyle.normal.background = solidWhite;
				gUIStyle.stretchWidth = true;
				gUIStyle.stretchHeight = true;
				GUI.backgroundColor = currentColor;
				GUI.Box(position, string.Empty, gUIStyle);
				GUI.backgroundColor = Color.white;
				GUIStyle gUIStyle2 = new GUIStyle();
				gUIStyle2.normal.background = screenOverlay;
				gUIStyle2.stretchWidth = true;
				gUIStyle2.stretchHeight = true;
				GUI.Box(position, string.Empty, gUIStyle2);
				SpriteSheetGUI component = GetComponent<SpriteSheetGUI>();
				Texture2D texture2D = snoop;
				Rect rect = new Rect(snoopPos.x, snoopPos.y, 8f, 16f);
				GUI.BeginGroup(new Rect(rect.x, rect.y, (float)texture2D.width * rect.width * component.Size.x, (float)texture2D.height * rect.height * component.Size.y));
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
				GUI.DrawTexture(new Rect((float)(-texture2D.width) * rect.width * component.Offset.x, (float)(-texture2D.height) * rect.height * component.Offset.y, (float)texture2D.width * rect.width, (float)texture2D.height * rect.height), texture2D);
				GUI.EndGroup();
			}
			if (bass)
			{
				GUIStyle gUIStyle3 = new GUIStyle();
				gUIStyle3.alignment = TextAnchor.MiddleCenter;
				gUIStyle3.fontSize = 600;
				gUIStyle3.fontStyle = FontStyle.Bold;
				gUIStyle3.normal.textColor = new Color(0f, 1f, 0f, 0.5f);
				GUI.Label(position, "BASS", gUIStyle3);
			}
		}
	}
}
